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
        public Xor(BlockBase parent) : base(parent) { }
        public Xor(BlockBase parent, IntValue[] values) : base(parent, values) { }
        public Xor(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        protected override void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
        {
            v.AddCodes(codes, m, "xor", ad);
        }
    }
}
