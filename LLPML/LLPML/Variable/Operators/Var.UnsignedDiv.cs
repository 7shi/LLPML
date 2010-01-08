using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Var : VarBase, IIntValue
    {
        public class UnsignedDiv : Add
        {
            public override int Max { get { return 2; } }

            public UnsignedDiv() { }
            public UnsignedDiv(Block parent, Var dest) : base(parent, dest) { }
            public UnsignedDiv(Block parent, Var dest, IntValue[] values) : base(parent, dest, values) { }
            public UnsignedDiv(Block parent, XmlTextReader xr) : base(parent, xr) { }

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
}
