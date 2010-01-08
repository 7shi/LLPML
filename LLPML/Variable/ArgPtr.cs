using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class ArgPtr : Pointer.Declare
    {
        public ArgPtr(BlockBase parent, string name) : base(parent, name) { }
        public ArgPtr(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequiresName(xr);

            AddToParent();
        }
    }
}
