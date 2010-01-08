using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Call : NodeBase, IIntValue
    {
        private IIntValue target;
        protected List<IIntValue> args = new List<IIntValue>();

        private Var var;
        private CallType type = CallType.CDecl;

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

        public Call(BlockBase parent, Var var)
            : base(parent)
        {
            this.var = var;
        }

        public Call(BlockBase parent, Var var, IIntValue target, params IIntValue[] args)
            : this(parent, var)
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
                this.var = new Var(parent, var);
                if (xr["type"] == "std") type = CallType.Std;
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

        public Function GetFunction(IIntValue target, out List<IIntValue> args)
        {
            Function ret;
            if (target == null)
            {
                ret = parent.GetFunction(name);
                if (ret.HasThis)
                    return GetFunction(new Struct.This(parent), out args);
                args = this.args;
                return ret;
            }

            args = new List<IIntValue>();
            string type = null;
            Struct.Define st = null;
            if (target is Struct.Member)
            {
                var ad = new AddrOf(parent, (target as Struct.Member));
                st = ad.Target.GetStruct();
                args.Add(ad);
            }
            else if (target is Index)
            {
                var ad = new AddrOf(parent, target as Index);
                st = ad.Target.GetStruct();
                args.Add(ad);
            }
            else if (target is VarBase)
            {
                type = (target as VarBase).Type;
                st = (target as VarBase).GetStruct();
                args.Add(target);
            }
            else
                throw Abort("struct instance or pointer required: " + name);
            if (st == null)
                throw Abort("undefined struct: " + type);
            args.AddRange(this.args);
            ret = st.GetFunction(name);
            if (ret == null)
                throw Abort("undefined method: " + st.GetMemberName(name));
            return ret;
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            if (name != null)
            {
                List<IIntValue> args;
                Function f = GetFunction(target, out args);
                if (f == null)
                    throw Abort("undefined function: " + name);
                DeclareBase[] fargs = f.Args.ToArray();
                int len = fargs.Length;
                if (!((len > 0 && fargs[len - 1] is ArgPtr && args.Count >= len - 1) || args.Count == len))
                    throw Abort("argument mismatched: " + name);
                AddCodes(codes, m, f, args);
            }
            else
            {
                AddCodes(codes, m, args, type, delegate
                {
                    codes.Add(I386.Call(var.GetAddress(codes, m)));
                });
            }
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            AddCodes(codes, m);
            IntValue.AddCodes(codes, op, dest);
        }

        public static void AddCodes(List<OpCode> codes, Module m, Function f, List<IIntValue> args)
        {
            AddCodes(codes, m, args, f.CallType, delegate
            {
                codes.Add(I386.Call(f.First));
            });
        }

        public static void AddCodes(
            List<OpCode> codes, Module m, List<IIntValue> args, CallType type, Action delg)
        {
            object[] args2 = args.ToArray();
            Array.Reverse(args2);
            foreach (IIntValue arg in args2)
            {
                arg.AddCodes(codes, m, "push", null);
            }
            delg();
            if (type == CallType.CDecl && args2.Length > 0)
            {
                codes.Add(I386.Add(Reg32.ESP, (byte)(args2.Length * 4)));
            }
        }
    }
}
