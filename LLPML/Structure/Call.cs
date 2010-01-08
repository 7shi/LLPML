using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Call : NodeBase, IIntValue
    {
        private IIntValue target;
        protected List<IIntValue> args = new List<IIntValue>();

        private IIntValue val;
        private CallType callType = CallType.CDecl;

        public Call(BlockBase parent, string name)
            : base(parent, name)
        {
        }

        public Call(BlockBase parent, string name, IIntValue target, params IIntValue[] args)
            : this(parent, name)
        {
            this.target = target;
            this.args.AddRange(args);
        }

        public Call(BlockBase parent, IIntValue val, IIntValue target, params IIntValue[] args)
            : base(parent)
        {
            this.val = val;
            this.target = target;
            this.args.AddRange(args);
        }

        public Call(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            bool invoke = xr.Name == "invoke";
            string name = xr["name"];
            string var = xr["var"];
            if (name != null && var == null)
            {
                this.name = name;
            }
            else if (name == null && var != null)
            {
                this.val = new Var(parent, var);
                if (xr["type"] == "std") callType = CallType.Std;
            }
            else
            {
                throw Abort(xr, "either name or var required");
            }

            Parse(xr, delegate
            {
                var vs = IntValue.Read(parent, xr);
                if (vs == null) return;
                foreach (var v in vs)
                {
                    if (invoke && target == null)
                        target = v;
                    else
                        args.Add(v);
                }
            });
        }

        public IIntValue GetFunction(OpCodes codes, IIntValue target, out List<IIntValue> args)
        {
            if (string.IsNullOrEmpty(name))
            {
                args = new List<IIntValue>();
                if (target != null) args.Add(target);
                args.AddRange(this.args);
                if (val is Function)
                    return val;
                else if (val is Function.Ptr)
                    return (val as Function.Ptr).GetFunction();
                return null;
            }

            Function ret = null;
            if (target == null)
            {
                ret = parent.GetFunction(name);
                if (ret == null)
                    throw Abort("undefined function: {0}", name);
                else if (ret.HasThis)
                    return GetFunction(codes, new Struct.This(parent), out args);
                args = this.args;
                return ret;
            }

            Struct.Define st = Types.GetStruct(target.Type);
            if (st != null)
            {
                ret = st.GetFunction(name);
                if (ret == null)
                {
                    var mem = st.GetMember(name);
                    if (mem != null)
                    {
                        args = this.args;
                        var mem2 = new Struct.Member(parent, name);
                        if (target is Struct.Member)
                        {
                            var mem3 = (target as Struct.Member).Duplicate();
                            mem3.Append(mem2);
                            return mem3;
                        }
                        mem2.Target = target;
                        return mem2;
                    }
                }
            }
            if (ret == null) ret = parent.GetFunction(name);
            if (ret == null)
            {
                if (st == null)
                    throw Abort("undefined function: " + name);
                else
                    throw Abort("undefined method: " + st.GetMemberName(name));
            }
            args = new List<IIntValue>();
            args.Add(target);
            args.AddRange(this.args);
            return ret;
        }

        public override void AddCodes(OpCodes codes)
        {
            if (name != null && name.StartsWith("__"))
            {
                if (AddIntrinsicCodes(codes)) return;
                if (AddSIMDCodes(codes)) return;
            }

            List<IIntValue> args;
            var f = GetFunction(codes, target, out args);
            var args_array = args.ToArray();
            if (f is Function)
            {
                (f.Type as TypeFunction).CheckArgs(this, args_array);
                AddCodes(codes, f as Function, args_array);
                return;
            }

            var val = this.val;
            if (f != null) val = f;
            var t = val.Type;
            if (t is TypeFunction)
                (t as TypeFunction).CheckArgs(this, args_array);
            bool cleanup = NeedsDtor(val);
            if (cleanup)
                codes.Add(I386.Sub(Reg32.ESP, 4));
            AddCodes(codes, args_array, callType, delegate
            {
                if (!cleanup)
                {
                    val.AddCodes(codes, "mov", null);
                    codes.Add(I386.Call(Reg32.EAX));
                }
                else
                {
                    var ad = new Addr32(Reg32.ESP, args_array.Length * 4);
                    val.AddCodes(codes, "mov", ad);
                    codes.AddRange(new[]
                    {
                        I386.Call(ad),
                        I386.Push(Reg32.EAX),
                    });
                    var ad2 = new Addr32(Reg32.ESP, 4);
                    if (callType == CallType.CDecl)
                        ad2.Add(args_array.Length * 4);
                    val.Type.AddDestructor(codes, ad2);
                    codes.Add(I386.Pop(Reg32.EAX));
                }
            });
            if (cleanup)
                codes.Add(I386.Add(Reg32.ESP, 4));
        }

        public void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            AddCodes(codes);
            codes.AddCodes(op, dest);
        }

        public static void AddCodes(OpCodes codes, Function f, IIntValue[] args)
        {
            AddCodes(codes, args, f.CallType, delegate
            {
                codes.Add(I386.Call(f.First));
            });
        }

        public static bool NeedsDtor(IIntValue arg)
        {
            if (arg is Call || arg is Delegate)
            {
                var t = arg.Type;
                if (t != null) return t.NeedsDtor;
            }
            return false;
        }

        public static void AddCodes(
            OpCodes codes, IIntValue[] args, CallType type, Action delg)
        {
            var args2 = args.Clone() as IIntValue[];
            Array.Reverse(args2);
            foreach (IIntValue arg in args2)
            {
                arg.AddCodes(codes, "push", null);
            }
            delg();
            if (type == CallType.CDecl && args2.Length > 0)
            {
                int p = 0;
                foreach (var arg in args)
                {
                    if (NeedsDtor(arg))
                        arg.Type.AddDestructor(codes, new Addr32(Reg32.ESP, p));
                    p += 4;
                }
                codes.Add(I386.Add(Reg32.ESP, (byte)(args2.Length * 4)));
            }
        }

        protected TypeBase type;
        protected bool doneInferType = false;

        public TypeBase Type
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
                else
                {
                    List<IIntValue> args;
                    var f = GetFunction(null, target, out args);
                    if (f is Function)
                        type = (f as Function).ReturnType;
                    else
                        type = TypeVar.Instance;
                }
                return type;
            }
        }

        public void PipeForward(IIntValue arg)
        {
            args.Add(arg);
        }

        public void PipeBack(IIntValue arg)
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
