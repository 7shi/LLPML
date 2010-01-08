using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class UnsignedGreaterEqual : Greater
    {
        public override Cc Condition { get { return Cc.AE; } }
        public override Cc NotCondition { get { return Cc.NAE; } }

        public UnsignedGreaterEqual() { }
        public UnsignedGreaterEqual(BlockBase parent) : base(parent) { }
        public UnsignedGreaterEqual(BlockBase parent, IntValue[] values) : base(parent, values) { }
        public UnsignedGreaterEqual(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
