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
        public class Operand2 : Operand1
        {
            protected IIntValue value;

            public Operand2() { }
            public Operand2(Block parent, string name) : base(parent, name) { }

            public Operand2(Block parent, string name, IntValue value)
                : this(parent, name)
            {
                this.value = value;
            }

            public Operand2(Block parent, string name, int value)
                : this(parent, name, new IntValue(value))
            {
            }

            public Operand2(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void Read(XmlTextReader xr)
            {
                Parse(xr, delegate
                {
                    IIntValue v = IntValue.Read(parent, xr, false);
                    if (v != null)
                    {
                        if (dest == null)
                        {
                            if (!(v is VarInt)) throw Abort(xr, "no variable specified");
                            dest = v as VarInt;
                        }
                        else if (value == null)
                            value = v;
                        else
                            throw Abort(xr, "too many operands");
                    }
                });
                if (dest == null)
                    throw Abort(xr, "no variable specified");
                else if (value == null)
                    throw Abort(xr, "no value specified");
            }
        }
    }
}
