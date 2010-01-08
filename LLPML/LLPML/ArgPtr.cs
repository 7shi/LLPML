using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class ArgPtr : Pointer.Define
    {
        public ArgPtr() { }
        public ArgPtr(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            if (!xr.IsEmptyElement)
                throw Abort(xr, "<" + xr.Name + "> can not have any children");

            name = xr["name"];
            parent.AddPointer(this);
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
        }
    }
}
