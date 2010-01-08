using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class UnsignedShiftLeft : ShiftLeft
    {
        public UnsignedShiftLeft() { }
        public UnsignedShiftLeft(Block parent) : base(parent) { }
        public UnsignedShiftLeft(Block parent, IntValue[] values) : base(parent, values) { }
        public UnsignedShiftLeft(Block parent, XmlTextReader xr) : base(parent, xr) { }

        protected override string Shift { get { return "shl"; } }
    }
}
