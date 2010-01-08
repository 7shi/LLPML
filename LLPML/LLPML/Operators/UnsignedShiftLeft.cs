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
        public UnsignedShiftLeft(BlockBase parent) : base(parent) { }
        public UnsignedShiftLeft(BlockBase parent, IntValue[] values) : base(parent, values) { }
        public UnsignedShiftLeft(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        protected override string Shift { get { return "shl"; } }
    }
}
