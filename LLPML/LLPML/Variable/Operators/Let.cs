using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Let : Var.Operator
    {
        public override int Min { get { return 1; } }
        public override int Max { get { return 1; } }

        public Let() { }
        public Let(BlockBase parent, Var dest) : base(parent, dest) { }

        public Let(BlockBase parent, Var dest, IntValue value)
            : base(parent, dest)
        {
            this.values.Add(value);
        }

        public Let(BlockBase parent, Var dest, int value)
            : this(parent, dest, new IntValue(value))
        {
        }

        public Let(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            values[0].AddCodes(codes, m, "mov", null);
            codes.Add(I386.Mov(dest.GetAddress(codes, m), Reg32.EAX));
        }
    }
}
