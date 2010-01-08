using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Neg : Operator, IIntValue
    {
        public override int Min { get { return 1; } }
        public override int Max { get { return 1; } }

        public Neg() { }
        public Neg(BlockBase parent) : base(parent) { }
        public Neg(BlockBase parent, IIntValue value) : base(parent, value) { }
        public Neg(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            values[0].AddCodes(codes, m, "mov", null);
            codes.Add(I386.Neg(Reg32.EAX));
            IntValue.AddCodes(codes, op, dest);
        }
    }
}
