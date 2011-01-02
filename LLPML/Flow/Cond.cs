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

        public override int Min { get { return 1; } }
        public override int Max { get { return 1; } }
        public Val32 First, Next;

        public Cond(BlockBase parent) : base(parent) { }
        public Cond(BlockBase parent, NodeBase values)
            : base(parent, new NodeBase[] { values }) { }
        public Cond(BlockBase parent, XmlTextReader xr)
            : base(parent, xr) { }

        public override void AddCodes(OpModule codes, string op, Addr32 dest) { }

        public override void AddCodes(OpModule codes)
        {
            if (Next != null)
            {
                values[0].AddCodes(codes, "mov", null);
                codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
                codes.Add(I386.Jcc(Cc.Z, Next));
            }
            else if (First != null)
            {
                values[0].AddCodes(codes, "mov", null);
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
