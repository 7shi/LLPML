using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Return : NodeBase
    {
        public bool IsLast = false;
        private IntValue value;
        private VarInt.Declare retval;

        public Return() { }
        public Return(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            value = new IntValue(parent);
            Parse(xr, delegate
            {
                value.ReadValue(xr, false);
            });
            if (value.HasValue)
            {
                retval = new VarInt.Declare(parent, "__retval", 0);
            }
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            if (value != null)
            {
                value.AddCodes(codes, m, "mov", retval.Address);
            }
            if (!IsLast) codes.Add(I386.Jmp(parent.Last));
        }
    }
}
