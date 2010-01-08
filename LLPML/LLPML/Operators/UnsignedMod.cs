using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class UnsignedMod : Add
    {
        public override int Min { get { return 2; } }
        public override int Max { get { return 2; } }

        public UnsignedMod() { }
        public UnsignedMod(BlockBase parent) : base(parent) { }
        public UnsignedMod(BlockBase parent, IntValue[] values) : base(parent, values) { }
        public UnsignedMod(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        protected override void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
        {
            v.AddCodes(codes, m, "mov", null);
            codes.AddRange(new OpCode[]
            {
                I386.Xchg(Reg32.EAX, ad),
                I386.Xor(Reg32.EDX, Reg32.EDX),
                I386.Div(ad),
                I386.Mov(ad, Reg32.EDX)
            });
        }
    }
}
