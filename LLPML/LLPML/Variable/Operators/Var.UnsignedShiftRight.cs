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
        public class UnsignedShiftRight : ShiftLeft
        {
            public UnsignedShiftRight() { }
            public UnsignedShiftRight(Block parent, Var dest) : base(parent, dest) { }
            public UnsignedShiftRight(Block parent, Var dest, IntValue[] values) : base(parent, dest, values) { }
            public UnsignedShiftRight(Block parent, XmlTextReader xr) : base(parent, xr) { }

            protected override string Shift { get { return "shr"; } }
        }
    }
}
