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
        public class Declare : DefineBase
        {
            private IntValue value;

            public Declare() { }

            public Declare(Block parent, string name)
                : base(parent, name)
            {
                parent.AddVarInt(this);
            }

            public Declare(Block parent, string name, int value)
                : this(parent, name)
            {
                this.value = new IntValue(value);
            }

            public Declare(Block parent, XmlTextReader xr)
                : base(parent, xr)
            {
            }

            public override void Read(XmlTextReader xr)
            {
                name = xr["name"];
                if (name == null) throw Abort(xr, "name required");

                IntValue v = new IntValue(parent);
                Parse(xr, delegate
                {
                    v.ReadValue(xr, false);
                });
                if (v.HasValue) value = v;

                parent.AddVarInt(this);
            }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                if (value != null)
                {
                    value.AddCodes(codes, m, "mov", address);
                }
            }
        }
    }
}
