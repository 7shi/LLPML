using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class OrElse : Operator, IIntValue
    {
        public OrElse() { }
        public OrElse(Block parent) : base(parent) { }
        public OrElse(Block parent, IntValue[] values) : base(parent, values) { }
        public OrElse(Block parent, XmlTextReader xr) : base(parent, xr) { }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            OpCode last = new OpCode();
            for (int i = 0; i < values.Count; i++)
            {
                values[i].AddCodes(codes, m, "mov", null);
                codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
                if (i < values.Count - 1)
                    codes.Add(I386.Jcc(Cc.NZ, last.Address));
            }
            codes.AddRange(new OpCode[]
            {
                last,
                I386.Mov(Reg32.EAX, (uint)0),
                I386.Setcc(Cc.NZ, Reg8.AL)
            });
            IntValue.AddCodes(codes, op, dest);
        }
    }
}
