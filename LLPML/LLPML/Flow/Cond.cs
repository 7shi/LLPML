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

        public Cond() { }
        public Cond(Block parent) : base(parent) { }
        public Cond(Block parent, IntValue values)
            : base(parent, new IntValue[] { values }) { }
        public Cond(Block parent, XmlTextReader xr)
            : base(parent, xr) { }

        public void AddPreCodes(List<OpCode> codes, Module m, Val32 next)
        {
            values[0].AddCodes(codes, m, "mov", null);
            codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
            codes.Add(I386.Jcc(Cc.Z, next));
        }

        public void AddPostCodes(List<OpCode> codes, Module m, Val32 first)
        {
            values[0].AddCodes(codes, m, "mov", null);
            codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
            codes.Add(I386.Jcc(Cc.NZ, first));
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
