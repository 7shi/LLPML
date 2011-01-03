using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class OrElse : Operator
    {
        public override string Tag { get { return "or-else"; } }
        public override TypeBase Type { get { return TypeBool.Instance; } }

        public static OrElse New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new OrElse(), parent, arg1, arg2) as OrElse;
        }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            if (AddConstCodes(codes, op, dest)) return;

            OpCode last = new OpCode();
            for (int i = 0; i < values.Count; i++)
            {
                (values[i] as NodeBase).AddCodesV(codes, "mov", null);
                codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
                if (i < values.Count - 1)
                    codes.Add(I386.Jcc(Cc.NZ, last.Address));
            }
            codes.Add(last);
            codes.Add(I386.MovR(Reg32.EAX, Val32.New(0)));
            codes.Add(I386.Setcc(Cc.NZ, Reg8.AL));
            codes.AddCodes(op, dest);
        }

        public override IntValue GetConst()
        {
            var v = IntValue.GetValue(values[0] as NodeBase);
            if (v == null) return null;

            var ret = v.Value != 0;
            for (int i = 1; i < values.Count; i++)
            {
                var iv = IntValue.GetValue(values[i] as NodeBase);
                if (iv == null) return null;
                ret = Calculate(ret, iv.Value != 0);
            }
            if (ret)
                return IntValue.One;
            else
                return IntValue.Zero;
        }

        protected bool Calculate(bool a, bool b) { return a || b; }
    }
}
