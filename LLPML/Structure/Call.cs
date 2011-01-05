using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.LLPML.Struct;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Call : NodeBase
    {
        private NodeBase target;
        protected ArrayList args = new ArrayList();

        private NodeBase val;
        private CallType callType = CallType.CDecl;

        public static Call NewName(BlockBase parent, string name)
        {
            var ret = new Call();
            ret.Parent = parent;
            ret.name = name;
            return ret;
        }

        public static Call New(BlockBase parent, string name, NodeBase target, NodeBase[] args)
        {
            var ret = NewName(parent, name);
            ret.target = target;
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                    ret.args.Add(args[i]);
            }
            if (string.IsNullOrEmpty(name) && target is Member)
                ret.name = (target as Member).GetName();
            return ret;
        }

        public static Call NewV(BlockBase parent, NodeBase val, NodeBase target, NodeBase[] args)
        {
            var ret = new Call();
            ret.Parent = parent;
            ret.val = val;
            ret.target = target;
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                    ret.args.Add(args[i]);
            }
            return ret;
        }

        private void AddArgs(ArrayList list)
        {
            for (int i = 0; i < args.Count; i++)
                list.Add(args[i]);
        }

        public NodeBase GetFunction(OpModule codes, NodeBase target, ArrayList[] args)
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
                args[0] = new ArrayList();
                if (memt != null && !memf.IsStatic)
                    args[0].Add(memt);
                AddArgs(args[0]);
                return memf;
            }
            else if (string.IsNullOrEmpty(name))
            {
                args[0] = new ArrayList();
                if (target != null) args[0].Add(target);
                AddArgs(args[0]);
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
                    return GetFunction(codes, This.New(Parent), args);
                args[0] = this.args;
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
                        args[0] = this.args;
                        var mem2 = Member.New(Parent, name);
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
            args[0] = new ArrayList();
            args[0].Add(target);
            AddArgs(args[0]);
            return ret;
        }

        public override void AddCodes(OpModule codes)
        {
            var args = new ArrayList[1];
            args[0] = new ArrayList();
            if (this.val == null && target is Member)
                args[0].Add((target as Member).GetTarget());
            else if (target != null)
                args[0].Add(target);
            AddArgs(args[0]);
            if (name != null && name.StartsWith("__"))
            {
                if (AddIntrinsicCodes(codes, args[0])) return;
                if (AddSIMDCodes(codes, args[0])) return;
            }

            var f = GetFunction(codes, target, args);
            var args_array = new NodeBase[args[0].Count];
            for (int i = 0; i < args_array.Length; i++)
                args_array[i] = args[0][i] as NodeBase;
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
                    val.Type.AddDestructorA(codes, ad2);
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
            for (int i = args.Length - 1; i >= 0; i--)
                args[i].AddCodesV(codes, "push", null);
            delg();
            if (type == CallType.CDecl && args.Length > 0)
            {
                int p = 4;
                bool pop = false;
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    if (OpModule.NeedsDtor(arg))
                    {
                        if (!pop)
                        {
                            codes.Add(I386.Push(Reg32.EAX));
                            pop = true;
                        }
                        arg.Type.AddDestructorA(codes, Addr32.NewRO(Reg32.ESP, p));
                    }
                    p += 4;
                }
                if (pop) codes.Add(I386.Pop(Reg32.EAX));
                codes.Add(I386.AddR(Reg32.ESP, Val32.New((byte)(args.Length * 4))));
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
                    var args = new ArrayList[1];
                    var f = GetFunction(null, target, args);
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
