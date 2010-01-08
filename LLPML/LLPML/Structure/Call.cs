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
        protected List<IIntValue> args = new List<IIntValue>();

        protected Var var;
        protected CallType type = CallType.CDecl;

        public Call()
        {
        }

        public Call(BlockBase parent, string name)
            : base(parent, name)
        {
        }

        public Call(BlockBase parent, string name, params IIntValue[] args)
            : this(parent, name)
        {
            this.args.AddRange(args);
        }

        public Call(BlockBase parent, Var var)
            : base(parent)
        {
            this.var = var;
        }

        public Call(BlockBase parent, Var var, params IIntValue[] args)
            : this(parent, var)
        {
            this.args.AddRange(args);
        }

        public Call(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
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
                IIntValue[] v = IntValue.Read(parent, xr);
                if (v != null) args.AddRange(v);
            });
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            if (name != null)
            {
                Function f = parent.GetFunction(name);
                if (f == null)
                    throw Abort("undefined function: " + name);
                DeclareBase[] fargs = f.GetArgs();
                if (fargs != null)
                {
                    int len = fargs.Length;
                    if (!(len > 0 && fargs[len - 1] is ArgPtr) && args.Count != len)
                        throw Abort("argument mismatched: " + name);
                }
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
            AddCodes(codes, m, args, f.Type, delegate
            {
                codes.Add(I386.Call(f.First));
            });
        }

        public static void AddCodes(
            List<OpCode> codes, Module m, List<IIntValue> args, CallType type, VoidDelegate delg)
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
