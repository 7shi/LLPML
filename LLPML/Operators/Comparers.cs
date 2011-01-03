using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class OperatorCmp : Operator
    {
        protected abstract bool Calculate(int a, int b);

        public override TypeBase Type { get { return TypeBool.Instance; } }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            if (AddConstCodes(codes, op, dest)) return;

            OpCode last = new OpCode();
            Addr32 ad = Addr32.New(Reg32.ESP);
            var tb = CheckFunc();
            var c = GetCond();
            var v = values[0];
            v.AddCodesV(codes, "push", null);
            for (int i = 1; i < values.Count; i++)
            {
                codes.AddOperatorCodes(tb, Tag, ad, values[i], true);
                if (i < values.Count - 1)
                {
                    codes.Add(I386.Jcc(c.NotCondition, last.Address));
                    codes.Add(I386.MovAR(ad, Reg32.EAX));
                }
            }
            codes.Add(last);
            codes.Add(I386.MovR(Reg32.EAX, Val32.New(0)));
            codes.Add(I386.Setcc(c.Condition, Reg8.AL));
            if (!OpModule.NeedsDtor(v))
                codes.Add(I386.AddR(Reg32.ESP, Val32.New(4)));
            else
            {
                codes.Add(I386.XchgRA(Reg32.EAX, Addr32.New(Reg32.ESP)));
                codes.Add(I386.Push(Reg32.EAX));
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
                if (!Calculate(a.Value, b.Value)) return IntValue.New(0);
            }
            return IntValue.New(1);
        }
    }

    public class Equal : OperatorCmp
    {
        public override string Tag { get { return "equal"; } }
        protected override bool Calculate(int a, int b) { return a == b; }

        public static Equal New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new Equal(), parent, arg1, arg2) as Equal;
        }
    }

    public class NotEqual : OperatorCmp
    {
        public override string Tag { get { return "not-equal"; } }
        protected override bool Calculate(int a, int b) { return a != b; }

        public static NotEqual New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new NotEqual(), parent, arg1, arg2) as NotEqual;
        }
    }

    public class Less : OperatorCmp
    {
        public override string Tag { get { return "less"; } }
        protected override bool Calculate(int a, int b) { return a < b; }

        public static Less New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new Less(), parent, arg1, arg2) as Less;
        }
    }

    public class Greater : OperatorCmp
    {
        public override string Tag { get { return "greater"; } }
        protected override bool Calculate(int a, int b) { return a > b; }

        public static Greater New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new Greater(), parent, arg1, arg2) as Greater;
        }
    }

    public class LessEqual : OperatorCmp
    {
        public override string Tag { get { return "less-equal"; } }
        protected override bool Calculate(int a, int b) { return a <= b; }

        public static LessEqual New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new LessEqual(), parent, arg1, arg2) as LessEqual;
        }
    }

    public class GreaterEqual : OperatorCmp
    {
        public override string Tag { get { return "greater-equal"; } }
        protected override bool Calculate(int a, int b) { return a >= b; }

        public static GreaterEqual New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new GreaterEqual(), parent, arg1, arg2) as GreaterEqual;
        }
    }
}
