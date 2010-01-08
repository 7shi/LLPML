using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class VarInt : VarBase
    {
        public class UnsignedShiftLeft : ShiftLeft
        {
            public UnsignedShiftLeft() { }
            public UnsignedShiftLeft(Block parent, VarInt dest) : base(parent, dest) { }
            public UnsignedShiftLeft(Block parent, VarInt dest, IntValue[] values) : base(parent, dest, values) { }
            public UnsignedShiftLeft(Block parent, XmlTextReader xr) : base(parent, xr) { }

            protected override string Shift { get { return "shl"; } }
        }
    }
}