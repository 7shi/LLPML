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
        public class Declare : DeclareBase
        {
            private IIntValue value;

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
                RequireName(xr);

                Parse(xr, delegate
                {
                    IIntValue v = IntValue.Read(parent, xr, true);
                    if (v != null)
                    {
                        if (value != null) throw Abort(xr, "multiple values");
                        value = v;
                    }
                });

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
