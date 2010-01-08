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
        public UnsignedGreater(BlockBase parent) : base(parent) { }
        public UnsignedGreater(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public UnsignedGreater(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
