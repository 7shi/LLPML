using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Inc : VarInt.Operand1
    {
        public Inc() { }
        public Inc(Block parent, string name) : base(parent, name) { }
        public Inc(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            codes.Add(I386.Inc(dest.GetAddress(codes, m)));
        }
    }
}
