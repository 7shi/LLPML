using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Let : Operand2
    {
        public Let() { }
        public Let(Block parent, string name) : base(parent, name) { }
        public Let(Block parent, string name, int value) : base(parent, name, value) { }
        public Let(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            value.AddCodes(codes, m, "mov", dest.GetAddress(codes, m));
        }
    }
}
