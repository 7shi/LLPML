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
        public ArgPtr() { }
        public ArgPtr(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequireName(xr);

            parent.AddPointer(this);
        }
    }
}
