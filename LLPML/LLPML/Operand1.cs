using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Operand1 : NodeBase
    {
        protected VarInt dest;

        public Operand1() { }

        public Operand1(Block parent, string name)
            : base(parent)
        {
            dest = new VarInt(parent, name);
        }

        public Operand1(Block parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

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
                    else
                        throw Abort(xr, "too many operands");
                }
            });
            if (dest == null)
                throw Abort(xr, "no variable specified");
        }
    }
}
