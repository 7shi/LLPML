using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class UnsignedLess : Greater
    {
        public override Cc Condition { get { return Cc.B; } }
        public override Cc NotCondition { get { return Cc.NB; } }

        public UnsignedLess() { }
        public UnsignedLess(Block parent) : base(parent) { }
        public UnsignedLess(Block parent, IntValue[] values) : base(parent, values) { }
        public UnsignedLess(Block parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
