using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class ShiftRight : ShiftLeft
    {
        public ShiftRight() { }
        public ShiftRight(Block parent) : base(parent) { }
        public ShiftRight(Block parent, IntValue[] values) : base(parent, values) { }
        public ShiftRight(Block parent, XmlTextReader xr) : base(parent, xr) { }

        protected override string Shift { get { return "sar"; } }
    }
}
