using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class OrElse : Operator
    {
        public override string Tag { get { return "or-else"; } }
        public override TypeBase Type { get { return TypeBool.Instance; } }

        public OrElse(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public OrElse(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            OpCode last = new OpCode();
            for (int i = 0; i < values.Count; i++)
            {
                values[i].AddCodes(codes, "mov", null);
                codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
                if (i < values.Count - 1)
                    codes.Add(I386.Jcc(Cc.NZ, last.Address));
            }
            codes.AddRange(new[]
            {
                last,
                I386.Mov(Reg32.EAX, (Val32)0),
                I386.Setcc(Cc.NZ, Reg8.AL)
            });
            codes.AddCodes(op, dest);
        }

        public override IntValue GetConst()
        {
            var v = GetValue(values[0]);
            if (v == null) return null;

            var ret = v.Value == 0 ? false : true;
            for (int i = 1; i < values.Count; i++)
            {
                var iv = GetValue(values[i]);
                if (iv == null) return null;
                ret = Calculate(ret, iv.Value == 0 ? false : true);
            }
            return new IntValue(ret ? 1 : 0);
        }

        protected bool Calculate(bool a, bool b) { return a || b; }
    }
}
