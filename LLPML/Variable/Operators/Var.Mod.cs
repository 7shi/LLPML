using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Var
    {
        public class Mod : Add
        {
            public override int Max { get { return 2; } }

            public Mod() { }
            public Mod(BlockBase parent, Var dest) : base(parent, dest) { }
            public Mod(BlockBase parent, Var dest, params IIntValue[] values) : base(parent, dest, values) { }
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
}
