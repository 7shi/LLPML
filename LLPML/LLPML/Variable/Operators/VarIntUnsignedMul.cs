using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class VarInt : VarBase
    {
        public class UnsignedMul : Add
        {
            public UnsignedMul() { }
            public UnsignedMul(Block parent, VarInt dest) : base(parent, dest) { }
            public UnsignedMul(Block parent, VarInt dest, IntValue[] values) : base(parent, dest, values) { }
            public UnsignedMul(Block parent, XmlTextReader xr) : base(parent, xr) { }

            protected override void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
            {
                v.AddCodes(codes, m, "mov", null);
                codes.Add(I386.Mul(ad));
                codes.Add(I386.Mov(ad, Reg32.EAX));
            }
        }
    }
}
