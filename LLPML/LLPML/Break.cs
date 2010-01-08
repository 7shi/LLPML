using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Break : NodeBase
    {
        public Break() { }
        public Break(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            codes.Add(I386.Jmp(parent.Last));
        }
    }
}
