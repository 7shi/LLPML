using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class UnsignedMul : Add
    {
        public UnsignedMul() { }
        public UnsignedMul(Block parent) : base(parent) { }
        public UnsignedMul(Block parent, IntValue[] values) : base(parent, values) { }
        public UnsignedMul(Block parent, XmlTextReader xr) : base(parent, xr) { }

        protected override void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
        {
            v.AddCodes(codes, m, "mov", null);
            codes.Add(I386.Mul(ad));
            codes.Add(I386.Mov(ad, Reg32.EAX));
        }
    }
}
