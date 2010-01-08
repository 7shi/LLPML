using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Xor : Add
    {
        public Xor() { }
        public Xor(Block parent) : base(parent) { }
        public Xor(Block parent, IntValue[] values) : base(parent, values) { }
        public Xor(Block parent, XmlTextReader xr) : base(parent, xr) { }

        protected override void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
        {
            v.AddCodes(codes, m, "xor", ad);
        }
    }
}
