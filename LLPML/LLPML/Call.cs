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
        public List<IIntValue> args = new List<IIntValue>();

        protected Var ptr;
        protected CallType type;

        public Call() { }
        public Call(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            string name = xr["name"];
            string ptr = xr["ptr"];
            if (name != null && ptr == null)
            {
                this.name = name;
            }
            else if (name == null && ptr != null)
            {
                this.ptr = new Var(parent, ptr);
                type = CallType.CDecl;
                if (xr["type"] == "std") type = CallType.Std;
            }
            else
            {
                throw Abort(xr, "either name or ptr required");
            }

            Parse(xr, delegate
            {
                IIntValue v = IntValue.Read(parent, xr, false);
                if (v != null) args.Add(v);
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
                    codes.Add(I386.Call(ptr.GetAddress(codes, m)));
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
