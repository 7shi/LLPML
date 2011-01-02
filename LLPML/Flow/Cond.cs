using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Cond : Operator
    {
        public override string Tag { get { return "cond"; } }

        public Val32 First, Next;

        public static Cond New(BlockBase parent, NodeBase arg)
        {
            return Init1(new Cond(), parent, arg) as Cond;
        }

        public override void AddCodes(OpModule codes)
        {
            if (Next != null)
            {
                values[0].AddCodesV(codes, "mov", null);
                codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
                codes.Add(I386.Jcc(Cc.Z, Next));
            }
            else if (First != null)
            {
                values[0].AddCodesV(codes, "mov", null);
                codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
                codes.Add(I386.Jcc(Cc.NZ, First));
            }
        }

        public override IntValue GetConst()
        {
            return IntValue.GetValue(values[0]);
        }
    }
}
