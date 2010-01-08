using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Equal : Operator, IIntValue
    {
        public Equal() { }
        public Equal(BlockBase parent) : base(parent) { }
        public Equal(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Equal(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            OpCode last = new OpCode();
            Addr32 ad = new Addr32(Reg32.ESP);
            for (int i = 0; i < values.Count; i++)
            {
                if (i == 0)
                {
                    values[i].AddCodes(codes, m, "push", null);
                }
                else
                {
                    values[i].AddCodes(codes, m, "mov", null);
                    codes.Add(I386.Cmp(ad, Reg32.EAX));
                    if (i < values.Count - 1)
                        codes.Add(I386.Jcc(Cc.NE, last.Address));
                }
            }
            codes.AddRange(new OpCode[]
            {
                last,
                I386.Mov(Reg32.EAX, (uint)0),
                I386.Setcc(Cc.E, Reg8.AL),
                I386.Add(Reg32.ESP, 4)
            });
            IntValue.AddCodes(codes, op, dest);
        }
    }
}
