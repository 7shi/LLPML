using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class UnsignedLessEqual : Greater
    {
        public override Cc Condition { get { return Cc.BE; } }
        public override Cc NotCondition { get { return Cc.NBE; } }

        public UnsignedLessEqual() { }
        public UnsignedLessEqual(Block parent) : base(parent) { }
        public UnsignedLessEqual(Block parent, IntValue[] values) : base(parent, values) { }
        public UnsignedLessEqual(Block parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
