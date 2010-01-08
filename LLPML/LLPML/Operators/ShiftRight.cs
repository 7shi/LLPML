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
        public ShiftRight(BlockBase parent) : base(parent) { }
        public ShiftRight(BlockBase parent, IntValue[] values) : base(parent, values) { }
        public ShiftRight(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        protected override string Shift { get { return "sar"; } }
    }
}
