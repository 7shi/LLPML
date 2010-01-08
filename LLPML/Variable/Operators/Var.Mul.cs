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
        public class Mul : Add
        {
            public Mul() { }
            public Mul(BlockBase parent, Var dest) : base(parent, dest) { }
            public Mul(BlockBase parent, Var dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Mul(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

            protected override void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
            {
                v.AddCodes(codes, m, "mov", null);
                codes.Add(I386.Imul(ad));
                codes.Add(I386.Mov(ad, Reg32.EAX));
            }
        }
    }
}