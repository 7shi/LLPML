using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Operand2 : Operand1
    {
        protected IntValue value;

        public Operand2() { }
        public Operand2(Block parent, string name) : base(parent, name) { }

        public Operand2(Block parent, string name, int value)
            : this(parent, name)
        {
            this.value = new IntValue(value);
        }

        public Operand2(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                IntValue v = new IntValue(parent);
                v.ReadValue(xr, true);
                if (v.HasValue)
                {
                    if (dest == null)
                    {
                        if (!v.IsVarInt) throw Abort(xr, "no variable specified");
                        dest = v.Source as VarInt;
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
