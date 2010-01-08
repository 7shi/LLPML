using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Greater : Operator, IIntValue
    {
        public virtual Cc Condition { get { return Cc.G; } }
        public virtual Cc NotCondition { get { return Cc.NG; } }

        public Greater() { }
        public Greater(BlockBase parent) : base(parent) { }
        public Greater(BlockBase parent, IntValue[] values) : base(parent, values) { }
        public Greater(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            OpCode last = new OpCode();
            Addr32 ad = new Addr32(Reg32.ESP);
            for (int i = 0; i < values.Count - 1; i++)
            {
                if (i == 0)
                    values[i].AddCodes(codes, m, "push", null);
                else
                    values[i].AddCodes(codes, m, "mov", ad);
                values[i + 1].AddCodes(codes, m, "mov", null);
                codes.Add(I386.Cmp(ad, Reg32.EAX));
                if (i < values.Count - 2)
                    codes.Add(I386.Jcc(NotCondition, last.Address));
            }
            codes.AddRange(new OpCode[]
            {
                last,
                I386.Mov(Reg32.EAX, (uint)0),
                I386.Setcc(Condition, Reg8.AL),
                I386.Add(Reg32.ESP, 4)
            });
            IntValue.AddCodes(codes, op, dest);
        }
    }
}
