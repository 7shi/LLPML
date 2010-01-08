using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Mod : Add
    {
        public override int Min { get { return 2; } }
        public override int Max { get { return 2; } }

        public Mod() { }
        public Mod(BlockBase parent) : base(parent) { }
        public Mod(BlockBase parent, IntValue[] values) : base(parent, values) { }
        public Mod(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        protected override void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
        {
            v.AddCodes(codes, m, "mov", null);
            codes.AddRange(new OpCode[]
            {
                I386.Xchg(Reg32.EAX, ad),
                I386.Cdq(),
                I386.Idiv(ad),
                I386.Mov(ad, Reg32.EDX)
            });
        }
    }
}
