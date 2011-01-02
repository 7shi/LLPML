using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.LLPML.Struct;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Call : NodeBase
    {
        private NodeBase target;
        protected List<NodeBase> args = new List<NodeBase>();

        private NodeBase val;
        private CallType callType = CallType.CDecl;

        public Call(BlockBase parent, string name)
        {
            Parent = parent;
            this.name = name;
        }

        public Call(BlockBase parent, string name, NodeBase target, params NodeBase[] args)
            : this(parent, name)
        {
            this.target = target;
            this.args.AddRange(args);
            if (string.IsNullOrEmpty(name) && target is Member)
                this.name = (target as Member).GetName();
        }

        public Call(BlockBase parent, NodeBase val, NodeBase target, params NodeBase[] args)
        {
            Parent = parent;
            this.val = val;
            this.target = target;
            this.args.AddRange(args);
        }

        public NodeBase GetFunction(OpModule codes, NodeBase target, out List<NodeBase> args)
        {
            if (val == null && target is Member)
            {
                var mem = target as Member;
                var memf = mem.GetFunction();
                if (memf == null)
                    memf = Parent.GetFunction(mem.GetName());
                if (memf == null)
                {
                    if (!string.IsNullOrEmpty(mem.TargetType))
                        throw Abort("call: undefined symbol: {0}", mem.TargetType);
                    else
                        throw Abort("call: undefined function: {0}", mem.FullName);
                }
                var memt = mem.GetTarget();
                args = new List<NodeBase>();
                if (memt != null && !memf.IsStatic)
                    args.Add(memt);
                args.AddRange(this.args);
                return memf;
            }
            else if (string.IsNullOrEmpty(name))
            {
                args = new List<NodeBase>();
                if (target != null) args.Add(target);
                args.AddRange(this.args);
                if (val is Function)
                    return val;
                else if (val is Variant)
                    return (val as Variant).GetFunction();
                return null;
            }

            Function ret = null;
            if (target == null)
            {
                ret = Parent.GetFunction(name);
                if (ret == null)
                    throw Abort("undefined function: {0}", name);
                else if (ret.HasThis)
                    return GetFunction(codes, This.New(Parent), out args);
                args = this.args;
                return ret;
            }

            var st = Types.GetStruct(target.Type);
            if (st != null)
            {
                ret = st.GetFunction(name);
                if (ret == null)
                {
                    var mem = st.GetMemberDecl(name);
                    if (mem != null)
                    {
                        args = this.args;
                        var mem2 = new Member(Parent, name);
                        if (target is Member)
                        {
                            var mem3 = (target as Member).Duplicate();
                            mem3.Append(mem2);
                            return mem3;
                        }
                        mem2.Target = target;
                        return mem2;
                    }
                }
            }
            if (ret == null) ret = Parent.GetFunction(name);
            if (ret == null)
            {
                if (st == null)
                    throw Abort("undefined function: {0}", name);
                else
                    throw Abort("undefined function: {0}", st.GetFullName(name));
            }
            args = new List<NodeBase>();
            args.Add(target);
            args.AddRange(this.args);
            return ret;
        }

        public override void AddCodes(OpModule codes)
        {
            var args = new List<NodeBase>();
            if (this.val == null && target is Member)
                args.Add((target as Member).GetTarget());
            else if (target != null)
                args.Add(target);
            args.AddRange(this.args);
            if (name != null && name.StartsWith("__"))
            {
                if (AddIntrinsicCodes(codes, args)) return;
                if (AddSIMDCodes(codes, args)) return;
            }

            var f = GetFunction(codes, target, out args);
            var args_array = args.ToArray();
            if (f is Function)
            {
                (f.Type as TypeFunction).CheckArgs(this, args_array);
                AddCallCodes(codes, f as Function, args_array);
                return;
            }

            var val = this.val;
            if (f != null) val = f;
            var t = val.Type;
            if (t is TypeFunction)
                (t as TypeFunction).CheckArgs(this, args_array);
            bool cleanup = NeedsDtor(val);
            if (cleanup)
                codes.Add(I386.SubR(Reg32.ESP, Val32.New(4)));
            AddCallCodes2(codes, args_array, callType, delegate
            {
                if (!cleanup)
                {
                    val.AddCodesV(codes, "mov", null);
                    codes.Add(I386.Call(Reg32.EAX));
                }
                else
                {
                    var ad = Addr32.NewRO(Reg32.ESP, args_array.Length * 4);
                    val.AddCodesV(codes, "mov", ad);
                    codes.Add(I386.CallA(ad));
                    codes.Add(I386.Push(Reg32.EAX));
                    var ad2 = Addr32.NewRO(Reg32.ESP, 4);
                    if (callType == CallType.CDecl)
                        ad2.Add(args_array.Length * 4);
                    val.Type.AddDestructor(codes, ad2);
                    codes.Add(I386.Pop(Reg32.EAX));
                }
            });
            if (cleanup)
                codes.Add(I386.AddR(Reg32.ESP, Val32.New(4)));
        }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            AddCodes(codes);
            codes.AddCodes(op, dest);
        }

        public static void AddCallCodes(OpModule codes, Function f, NodeBase[] args)
        {
            AddCallCodes2(codes, args, f.CallType, delegate
            {
                codes.Add(I386.CallD(f.First));
            });
        }

        public static bool NeedsDtor(NodeBase arg)
        {
            if (arg is Call || arg is Delegate)
            {
                var t = arg.Type;
                if (t != null) return t.NeedsDtor;
            }
            else if (arg is Member)
            {
                var t = arg.Type;
                if (t is TypeDelegate) return t.NeedsDtor;
            }
            return false;
        }

        public static void AddCallCodes2(
            OpModule codes, NodeBase[] args, CallType type, Action delg)
        {
            var args2 = args.Clone() as NodeBase[];
            Array.Reverse(args2);
            foreach (NodeBase arg in args2)
                arg.AddCodesV(codes, "push", null);
            delg();
            if (type == CallType.CDecl && args2.Length > 0)
            {
                int p = 4;
                bool pop = false;
                foreach (var arg in args)
                {
                    if (OpModule.NeedsDtor(arg))
                    {
                        if (!pop)
                        {
                            codes.Add(I386.Push(Reg32.EAX));
                            pop = true;
                        }
                        arg.Type.AddDestructor(codes, Addr32.NewRO(Reg32.ESP, p));
                    }
                    p += 4;
                }
                if (pop) codes.Add(I386.Pop(Reg32.EAX));
                codes.Add(I386.AddR(Reg32.ESP, Val32.New((byte)(args2.Length * 4))));
            }
        }

        protected TypeBase type;
        protected bool doneInferType = false;

        public override TypeBase Type
        {
            get
            {
                if (doneInferType || !root.IsCompiling)
                    return type;

                doneInferType = true;
                if (name == null)
                {
                    var t = val.Type;
                    if (t is TypeFunction)
                        type = (t as TypeFunction).RetType;
                    else
                        type = TypeVar.Instance;
                }
                else if (AddIntrinsicCodes(null, this.args)
                    || AddSIMDCodes(null, this.args))
                {
                    return null;
                }
                else
                {
                    List<NodeBase> args;
                    var f = GetFunction(null, target, out args);
                    if (f is Function)
                        type = (f as Function).ReturnType;
                    else
                        type = TypeVar.Instance;
                }
                return type;
            }
        }

        public void PipeForward(NodeBase arg)
        {
            args.Add(arg);
        }

        public void PipeBack(NodeBase arg)
        {
            //if (target != null) args.Insert(0, target);
            //target = arg;
            if (target == null)
                target = arg;
            else
                args.Insert(0, arg);
        }
    }
}
