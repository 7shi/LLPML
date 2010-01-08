using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    // operator_X(a, b, c) => a X b && b X c

    public class Equal : Operator
    {
        public override string Tag { get { return "equal"; } }
        public override TypeBase Type { get { return TypeBool.Instance; } }

        public Equal(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Equal(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            if (AddConstCodes(codes, op, dest)) return;

            OpCode last = new OpCode();
            Addr32 ad = new Addr32(Reg32.ESP);
            var f = GetFunc();
            var c = GetCond();
            var v = values[0];
            v.AddCodes(codes, "push", null);
            for (int i = 1; i < values.Count; i++)
            {
                codes.AddOperatorCodes(f, ad, values[i], true);
                if (i < values.Count - 1)
                    codes.AddRange(new[]
                    {
                        I386.Jcc(c.NotCondition, last.Address),
                        I386.Mov(ad, Reg32.EAX)
                    });
            }
            codes.AddRange(new[]
            {
                last,
                I386.Mov(Reg32.EAX, (Val32)0),
                I386.Setcc(c.Condition, Reg8.AL),
            });
            if (!OpModule.NeedsDtor(v))
                codes.Add(I386.Add(Reg32.ESP, 4));
            else
            {
                codes.AddRange(new[]
                {
                    I386.Xchg(Reg32.EAX, new Addr32(Reg32.ESP)),
                    I386.Push(Reg32.EAX),
                });
                codes.AddDtorCodes(v.Type);
                codes.Add(I386.Pop(Reg32.EAX));
            }
            codes.AddCodes(op, dest);
        }

        public override IntValue GetConst()
        {
            for (int i = 0; i < values.Count - 1; i++)
            {
                var a = IntValue.GetValue(values[i]);
                var b = IntValue.GetValue(values[i + 1]);
                if (a == null || b == null) return null;
                if (!Calculate(a.Value, b.Value)) return new IntValue(0);
            }
            return new IntValue(1);
        }

        protected virtual bool Calculate(int a, int b) { return a == b; }
    }

    public class NotEqual : Equal
    {
        public override string Tag { get { return "not-equal"; } }
        public NotEqual(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public NotEqual(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override bool Calculate(int a, int b) { return a != b; }
    }

    public class Less : Equal
    {
        public override string Tag { get { return "less"; } }
        public Less(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Less(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override bool Calculate(int a, int b) { return a < b; }
    }

    public class Greater : Equal
    {
        public override string Tag { get { return "greater"; } }
        public Greater(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Greater(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override bool Calculate(int a, int b) { return a > b; }
    }

    public class LessEqual : Equal
    {
        public override string Tag { get { return "less-equal"; } }
        public LessEqual(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public LessEqual(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override bool Calculate(int a, int b) { return a <= b; }
    }

    public class GreaterEqual : Equal
    {
        public override string Tag { get { return "greater-equal"; } }
        public GreaterEqual(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public GreaterEqual(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override bool Calculate(int a, int b) { return a >= b; }
    }
}
