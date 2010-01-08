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
        public class Add : Operand2
        {
            public Add() { }
            public Add(Block parent, string name) : base(parent, name) { }
            public Add(Block parent, string name, IntValue value) : base(parent, name, value) { }
            public Add(Block parent, string name, int value) : base(parent, name, value) { }
            public Add(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                value.AddCodes(codes, m, "add", dest.GetAddress(codes, m));
            }
        }
    }
}
