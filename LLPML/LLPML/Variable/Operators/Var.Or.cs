using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Var : VarBase
    {
        public class Or : Add
        {
            public Or() { }
            public Or(Block parent, Var dest) : base(parent, dest) { }
            public Or(Block parent, Var dest, IntValue[] values) : base(parent, dest, values) { }
            public Or(Block parent, XmlTextReader xr) : base(parent, xr) { }

            protected override void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
            {
                v.AddCodes(codes, m, "or", ad);
            }
        }
    }
}
