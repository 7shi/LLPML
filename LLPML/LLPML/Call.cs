using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Call : NodeBase
    {
        public List<IntValue> args = new List<IntValue>();

        private string name;
        private VarInt ptr;
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
                this.ptr = new VarInt(parent, ptr);
                type = CallType.CDecl;
                if (xr["type"] == "std") type = CallType.Std;
            }
            else
            {
                throw Abort(xr, "either name or ptr required");
            }

            Parse(xr, delegate
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Element:
                        args.Add(new IntValue(parent, xr));
                        break;

                    case XmlNodeType.Whitespace:
                    case XmlNodeType.Comment:
                        break;

                    default:
                        throw Abort(xr, "value required");
                }
            });
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            object[] args = this.args.ToArray();
            Array.Reverse(args);
            foreach (IntValue arg in args)
            {
                arg.GetValue(codes, m, null);
            }
            if (name != null)
            {
                Function f = parent.GetFunction(name);
                if (f == null)
                    throw new Exception("undefined function: " + name);
                type = f.Type;
                codes.Add(I386.Call(f.Address));
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
    }
}
