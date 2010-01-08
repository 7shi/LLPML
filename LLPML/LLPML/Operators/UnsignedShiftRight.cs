using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class UnsignedShiftRight : ShiftLeft
    {
        public UnsignedShiftRight() { }
        public UnsignedShiftRight(Block parent) : base(parent) { }
        public UnsignedShiftRight(Block parent, IntValue[] values) : base(parent, values) { }
        public UnsignedShiftRight(Block parent, XmlTextReader xr) : base(parent, xr) { }

        protected override string Shift { get { return "shr"; } }
    }
}
