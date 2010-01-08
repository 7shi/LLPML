using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class ArgInt : VarInt.Declare
    {
        public ArgInt() { }
        public ArgInt(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequireName(xr);

            parent.AddVarInt(this);
        }
    }
}
