using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class LessEqual : Greater
    {
        public override Cc Condition { get { return Cc.LE; } }
        public override Cc NotCondition { get { return Cc.NLE; } }

        public LessEqual() { }
        public LessEqual(BlockBase parent) : base(parent) { }
        public LessEqual(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public LessEqual(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
