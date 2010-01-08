using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class UnsignedDiv : Add
    {
        public override int Max { get { return 2; } }

        public UnsignedDiv() { }
        public UnsignedDiv(BlockBase parent) : base(parent) { }
        public UnsignedDiv(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public UnsignedDiv(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        protected override void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
        {
            v.AddCodes(codes, m, "mov", null);
            codes.AddRange(new OpCode[]
            {
                I386.Xchg(Reg32.EAX, ad),
                I386.Xor(Reg32.EDX, Reg32.EDX),
                I386.Div(ad),
                I386.Mov(ad, Reg32.EAX)
            });
        }
    }
}
