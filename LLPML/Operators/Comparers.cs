using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Equal : Operator
    {
        public override string Tag { get { return "equal"; } }
        public override TypeBase Type { get { return TypeBool.Instance; } }

        public Equal(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Equal(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            OpCode last = new OpCode();
            Addr32 ad = new Addr32(Reg32.ESP);
            var f = GetFunc();
            var c = GetCond();
            for (int i = 0; i < values.Count; i++)
            {
                if (i == 0)
                    values[i].AddCodes(codes, "push", null);
                else
                {
                    f(codes, ad, values[i]);
                    if (i < values.Count - 1)
                        codes.AddRange(new[]
                        {
                            I386.Jcc(c.NotCondition, last.Address),
                            I386.Mov(ad, Reg32.EAX)
                        });
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

    public class Less : Equal
    {
        public override string Tag { get { return "less"; } }
        public Less(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Less(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class Greater : Equal
    {
        public override string Tag { get { return "greater"; } }
        public Greater(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Greater(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class LessEqual : Equal
    {
        public override string Tag { get { return "less-equal"; } }
        public LessEqual(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public LessEqual(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class GreaterEqual : Equal
    {
        public override string Tag { get { return "greater-equal"; } }
        public GreaterEqual(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public GreaterEqual(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
