using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class UnsignedGreater : Greater
    {
        public override Cc Condition { get { return Cc.A; } }
        public override Cc NotCondition { get { return Cc.NA; } }

        public UnsignedGreater() { }
        public UnsignedGreater(Block parent) : base(parent) { }
        public UnsignedGreater(Block parent, IntValue[] values) : base(parent, values) { }
        public UnsignedGreater(Block parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
