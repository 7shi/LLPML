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
        public class Define : DefineBase
        {
            private int? value;
            public int Value { get { return (int)value; } }
            public bool HasValue { get { return value != null; } }

            public Define() { }

            public Define(Block parent, string name)
                : base(parent, name)
            {
                parent.AddVarInt(this);
            }

            public Define(Block parent, string name, int value)
                : this(parent, name)
            {
                this.value = value;
            }

            public Define(Block parent, XmlTextReader xr)
                : base(parent, xr)
            {
            }

            public override void Read(XmlTextReader xr)
            {
                name = xr["name"];
                value = null;
                Parse(xr, delegate
                {
                    if (xr.NodeType == XmlNodeType.Text)
                    {
                        value = int.Parse(xr.Value);
                    }
                    else if (xr.NodeType != XmlNodeType.Whitespace)
                    {
                        throw Abort(xr, "invalid node");
                    }
                });
                parent.AddVarInt(this);
            }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                if (value != null)
                {
                    codes.Add(I386.Mov(Address, (uint)value));
                }
            }
        }
    }
}
