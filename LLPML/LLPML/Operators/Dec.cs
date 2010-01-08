using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Dec : VarInt.Operand1
    {
        public Dec() { }
        public Dec(Block parent, string name) : base(parent, name) { }
        public Dec(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            codes.Add(I386.Dec(dest.GetAddress(codes, m)));
        }
    }
}
