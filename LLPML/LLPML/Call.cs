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

        private Var ptr;
        private CallType type;

        public Call() { }
        public Call(Block parent, XmlTextReader xr) : base(parent, xr) { }

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
            object[] args = this.args.ToArray();
            Array.Reverse(args);
            foreach (IIntValue arg in args)
            {
                arg.AddCodes(codes, m, "push", null);
            }
            if (name != null)
            {
                Function f = parent.GetFunction(name);
                if (f == null)
                    throw new Exception("undefined function: " + name);
                type = f.Type;
                codes.Add(I386.Call(f.First));
            }
            else
            {
                codes.Add(I386.Call(ptr.GetAddress(codes, m)));
            }
            if (type == CallType.CDecl && args.Length > 0)
            {
                codes.Add(I386.Add(Reg32.ESP, (byte)(args.Length * 4)));
            }
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            AddCodes(codes, m);
            IntValue.AddCodes(codes, op, dest);
        }
    }
}
