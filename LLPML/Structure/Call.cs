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

        public Call()
        {
        }

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

        public Call(BlockBase parent, IIntValue val)
            : base(parent)
        {
            this.val = val;
        }

        public Call(BlockBase parent, IIntValue val, IIntValue target, params IIntValue[] args)
            : this(parent, val)
        {
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

        public Function GetFunction(OpCodes codes, IIntValue target, out List<IIntValue> args)
        {
            Function ret = null;
            bool isStatic = target is Function.Ptr;
            if (target == null || isStatic)
            {
                ret = parent.GetFunction(name);
                if (ret == null)
                    throw Abort("undefined function: {0}", name);
                else if (ret.HasThis && !isStatic)
                    return GetFunction(codes, new Struct.This(parent), out args);
                args = this.args;
                return ret;
            }

            string type = null;
            Struct.Define st = null;
            IIntValue arg1 = null;
            if (target is Struct.Member)
            {
                st = (target as Struct.Member).GetStruct();
                arg1 = target;
            }
            else if (target is Index)
            {
                var ad = new AddrOf(parent, target as Index);
                st = ad.Target.GetStruct();
                arg1 = ad;
            }
            else if (target is Var)
            {
                type = (target as Var).Type.Name;
                st = (target as Var).GetStruct();
                arg1 = target;
            }
            else
            {
                //throw Abort("struct instance or pointer required: " + name);
                arg1 = target;
            }
            args = new List<IIntValue>();
            if (st != null)
            {
                ret = st.GetFunction(name);
                if (ret == null)
                {
                    var mem = st.GetMember(name);
                    if (mem != null)
                    {
                        var mem2 = new Struct.Member(parent, name);
                        if (arg1 is Struct.Member)
                        {
                            var mem3 = arg1 as Struct.Member;
                            mem3.Append(mem2);
                            mem2 = mem3;
                        }
                        else
                            mem2.Target = arg1 as Var;
                        AddCodes(codes, this.args, CallType.CDecl, delegate
                        {
                            codes.Add(I386.Call(mem2.GetAddress(codes)));
                        });
                        return null;
                    }
                }
            }
            if (ret == null)
                ret = parent.GetFunction(name);
            if (ret == null)
            {
                if (st == null)
                    throw Abort("undefined struct: " + type);
                else
                    throw Abort("undefined method: " + st.GetMemberName(name));
            }
            args.Add(arg1);
            args.AddRange(this.args);
            return ret;
        }

        public override void AddCodes(OpCodes codes)
        {
            if (codes == null) return;

            if (name != null)
            {
                if (name.StartsWith("__"))
                {
                    if (AddIntrinsicCodes(codes)) return;
                    if (AddSIMDCodes(codes)) return;
                }

                List<IIntValue> args;
                var f = GetFunction(codes, target, out args);
                if (f == null) return;
                var fargs = f.Args.ToArray();
                var len = fargs.Length;
                if (!((len > 0 && fargs[len - 1] is ArgPtr && args.Count >= len - 1) || args.Count == len))
                    throw Abort("argument mismatched: " + name);
                AddCodes(codes, f, args);
            }
            else
            {
                AddCodes(codes, args, callType, delegate
                {
                    if (val is Var)
                        codes.Add(I386.Call((val as Var).GetAddress(codes)));
                    else
                    {
                        val.AddCodes(codes, "mov", null);
                        codes.Add(I386.Call(Reg32.EAX));
                    }
                });
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
                List<IIntValue> args;
                var f = GetFunction(null, target, out args);
                if (f != null)
                    type = f.ReturnType;
                else
                    type = TypeInt.Instance;
                return type;
            }
        }

        public void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            AddCodes(codes);
            codes.AddCodes(op, dest);
        }

        public static void AddCodes(OpCodes codes, Function f, List<IIntValue> args)
        {
            AddCodes(codes, args, f.CallType, delegate
            {
                codes.Add(I386.Call(f.First));
            });
        }

        public static void AddCodes(
            OpCodes codes, List<IIntValue> args, CallType type, Action delg)
        {
            object[] args2 = args.ToArray();
            Array.Reverse(args2);
            foreach (IIntValue arg in args2)
            {
                arg.AddCodes(codes, "push", null);
            }
            delg();
            if (type == CallType.CDecl && args2.Length > 0)
            {
                codes.Add(I386.Add(Reg32.ESP, (byte)(args2.Length * 4)));
            }
        }
    }
}
