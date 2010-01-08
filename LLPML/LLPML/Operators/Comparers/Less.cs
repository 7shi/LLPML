using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Less : Greater
    {
        public override Cc Condition { get { return Cc.L; } }
        public override Cc NotCondition { get { return Cc.NL; } }

        public Less() { }
        public Less(Block parent) : base(parent) { }
        public Less(Block parent, IntValue[] values) : base(parent, values) { }
        public Less(Block parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
