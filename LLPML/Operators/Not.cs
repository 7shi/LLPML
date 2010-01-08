using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Not : Operator, IIntValue
    {
        public override int Min { get { return 1; } }
        public override int Max { get { return 1; } }

        public Not() { }
        public Not(BlockBase parent) : base(parent) { }
        public Not(BlockBase parent, IIntValue value) : base(parent, value) { }
        public Not(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            values[0].AddCodes(codes, m, "mov", null);
            codes.AddRange(new OpCode[]
            {
                I386.Test(Reg32.EAX, Reg32.EAX),
                I386.Mov(Reg32.EAX, (uint)0),
                I386.Setcc(Cc.Z, Reg8.AL)
            });
            IntValue.AddCodes(codes, op, dest);
        }
    }
}
