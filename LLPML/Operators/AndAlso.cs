using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class AndAlso : Operator
    {
        public override string Tag { get { return "and-also"; } }

        public AndAlso(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public AndAlso(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            OpCode last = new OpCode();
            foreach (IIntValue v in values)
            {
                v.AddCodes(codes, "mov", null);
                codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
                codes.Add(I386.Jcc(Cc.Z, last.Address));
            }
            codes.Add(I386.Mov(Reg32.EAX, 1));
            codes.Add(last);
            codes.AddCodes(op, dest);
        }
    }
}
