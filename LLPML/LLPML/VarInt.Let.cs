using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class VarInt : VarBase
    {
        public class Let : NodeBase
        {
            private IntValue value;
            private VarInt reference;

            public Let() { }

            public Let(Block parent, string name)
                : base(parent)
            {
                reference = new VarInt(parent, name);
            }

            public Let(Block parent, string name, int value)
                : this(parent, name)
            {
                this.value = new IntValue(value);
            }

            public Let(Block parent, XmlTextReader xr)
                : base(parent, xr)
            {
            }

            public override void Read(XmlTextReader xr)
            {
                string name = xr["name"];
                if (name == null) throw Abort(xr, "name required");

                reference = new VarInt(parent, name);

                Parse(xr, delegate
                {
                    switch (xr.NodeType)
                    {
                        case XmlNodeType.Text:
                            value = new IntValue(int.Parse(xr.Value));
                            break;

                        case XmlNodeType.Element:
                            if (value != null)
                                throw Abort(xr, "multiple value");
                            value = new IntValue(parent, xr);
                            break;

                        case XmlNodeType.Whitespace:
                        case XmlNodeType.Comment:
                            break;

                        default:
                            throw Abort(xr, "value required");
                    }
                });
                if (value == null) throw Abort(xr, "value required");
            }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                value.GetValue(codes, m, reference.GetAddress(codes, m));
            }
        }
    }
}
