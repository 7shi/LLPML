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
        public override int Min { get { return 1; } }
        public override int Max { get { return 1; } }
        public Val32 First, Next;

        public Cond(BlockBase parent) : base(parent) { }
        public Cond(BlockBase parent, IntValue values)
            : base(parent, new IntValue[] { values }) { }
        public Cond(BlockBase parent, XmlTextReader xr)
            : base(parent, xr) { }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            if (Next != null)
            {
                values[0].AddCodes(codes, m, "mov", null);
                codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
                codes.Add(I386.Jcc(Cc.Z, Next));
            }
            else if (First != null)
            {
                values[0].AddCodes(codes, m, "mov", null);
                codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
                codes.Add(I386.Jcc(Cc.NZ, First));
            }
        }

        public bool IsAlwaysFalse
        {
            get
            {
                IntValue v = values[0] as IntValue;
                if (v == null) return false;
                return v.Value == 0;
            }
        }

        public bool IsAlwaysTrue
        {
            get
            {
                IntValue v = values[0] as IntValue;
                if (v == null) return false;
                return v.Value != 0;
            }
        }
    }
}
