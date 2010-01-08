using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Mul : Add
    {
        public Mul() { }
        public Mul(Block parent) : base(parent) { }
        public Mul(Block parent, IntValue[] values) : base(parent, values) { }
        public Mul(Block parent, XmlTextReader xr) : base(parent, xr) { }

        protected override void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
        {
            v.AddCodes(codes, m, "mov", null);
            codes.Add(I386.Imul(ad));
            codes.Add(I386.Mov(ad, Reg32.EAX));
        }
    }
}
