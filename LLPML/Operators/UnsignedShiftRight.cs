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
        public UnsignedShiftRight(BlockBase parent) : base(parent) { }
        public UnsignedShiftRight(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public UnsignedShiftRight(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        protected override string Shift { get { return "shr"; } }
    }
}
