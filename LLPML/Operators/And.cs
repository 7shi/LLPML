using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class And : Add
    {
        public And() { }
        public And(BlockBase parent) : base(parent) { }
        public And(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public And(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        protected override void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
        {
            v.AddCodes(codes, m, "and", ad);
        }
    }
}
