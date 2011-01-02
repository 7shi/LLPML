using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class AndAlso : Operator
    {
        public override string Tag { get { return "and-also"; } }
        public override TypeBase Type { get { return TypeBool.Instance; } }

        public AndAlso(BlockBase parent, params NodeBase[] values) : base(parent, values) { }

        public override void AddCodesValue(OpModule codes, string op, Addr32 dest)
        {
            if (AddConstCodes(codes, op, dest)) return;

            var last = new OpCode();
            foreach (var v in values)
            {
                v.AddCodesValue(codes, "mov", null);
                codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
                codes.Add(I386.Jcc(Cc.Z, last.Address));
            }
            codes.Add(I386.MovR(Reg32.EAX, Val32.New(1)));
            codes.Add(last);
            codes.AddCodes(op, dest);
        }

        public override IntValue GetConst()
        {
            var v = IntValue.GetValue(values[0]);
            if (v == null) return null;

            var ret = v.Value == 0 ? false : true;
            for (int i = 1; i < values.Count; i++)
            {
                var iv = IntValue.GetValue(values[i]);
                if (iv == null) return null;
                ret = Calculate(ret, iv.Value == 0 ? false : true);
            }
            return new IntValue(ret ? 1 : 0);
        }

        protected bool Calculate(bool a, bool b) { return a && b; }
    }
}
