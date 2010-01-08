using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class GreaterEqual : Greater
    {
        public override Cc Condition { get { return Cc.GE; } }
        public override Cc NotCondition { get { return Cc.NGE; } }

        public GreaterEqual() { }
        public GreaterEqual(BlockBase parent) : base(parent) { }
        public GreaterEqual(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public GreaterEqual(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
