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
        public class Sub : Operand2
        {
            public Sub() { }
            public Sub(Block parent, string name) : base(parent, name) { }
            public Sub(Block parent, string name, IntValue value) : base(parent, name, value) { }
            public Sub(Block parent, string name, int value) : base(parent, name, value) { }
            public Sub(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                value.AddCodes(codes, m, "sub", dest.GetAddress(codes, m));
            }
        }
    }
}
