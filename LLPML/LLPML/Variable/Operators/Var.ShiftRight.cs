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
        public class ShiftRight : ShiftLeft
        {
            public ShiftRight() { }
            public ShiftRight(BlockBase parent, Var dest) : base(parent, dest) { }
            public ShiftRight(BlockBase parent, Var dest, IntValue[] values) : base(parent, dest, values) { }
            public ShiftRight(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

            protected override string Shift { get { return "sar"; } }
        }
    }
}
