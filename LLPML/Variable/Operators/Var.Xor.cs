using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Var
    {
        public class Xor : Add
        {
            public Xor() { }
            public Xor(BlockBase parent, Var dest) : base(parent, dest) { }
            public Xor(BlockBase parent, Var dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Xor(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

            protected override void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
            {
                v.AddCodes(codes, m, "xor", ad);
            }
        }
    }
}
