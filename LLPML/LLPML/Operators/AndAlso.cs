using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class AndAlso : Operator, IIntValue
    {
        public AndAlso() { }
        public AndAlso(BlockBase parent) : base(parent) { }
        public AndAlso(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public AndAlso(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            OpCode last = new OpCode();
            foreach (IIntValue v in values)
            {
                v.AddCodes(codes, m, "mov", null);
                codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
                codes.Add(I386.Jcc(Cc.Z, last.Address));
            }
            codes.Add(I386.Mov(Reg32.EAX, 1));
            codes.Add(last);
            IntValue.AddCodes(codes, op, dest);
        }
    }
}
