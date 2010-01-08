using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class NotEqual : Operator
    {
        public override string Tag { get { return "not-equal"; } }
        public override TypeBase Type { get { return TypeBool.Instance; } }

        public NotEqual(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public NotEqual(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            var last = new OpCode();
            var ad = new Addr32(Reg32.ESP);
            var f = GetFunc();
            var c = GetCond();
            for (int i = 0; i < values.Count - 1; i++)
            {
                if (i == 0)
                    values[i].AddCodes(codes, "push", null);
                else
                    values[i].AddCodes(codes, "mov", ad);
                for (int j = i + 1; j < values.Count; j++)
                {
                    f(codes, ad, values[j]);
                    if (i < values.Count - 2)
                        codes.Add(I386.Jcc(c.NotCondition, last.Address));
                }
            }
            codes.AddRange(new[]
            {
                last,
                I386.Mov(Reg32.EAX, (Val32)0),
                I386.Setcc(c.Condition, Reg8.AL),
                I386.Add(Reg32.ESP, 4)
            });
            codes.AddCodes(op, dest);
        }
    }
}
