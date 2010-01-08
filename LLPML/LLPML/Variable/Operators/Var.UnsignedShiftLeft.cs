using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Var : VarBase, IIntValue
    {
        public class UnsignedShiftLeft : ShiftLeft
        {
            public UnsignedShiftLeft() { }
            public UnsignedShiftLeft(BlockBase parent, Var dest) : base(parent, dest) { }
            public UnsignedShiftLeft(BlockBase parent, Var dest, IntValue[] values) : base(parent, dest, values) { }
            public UnsignedShiftLeft(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

            protected override string Shift { get { return "shl"; } }
        }
    }
}
