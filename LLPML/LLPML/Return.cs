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
        private IIntValue value;
        private VarInt.Declare retval;

        public Return() { }
        public Return(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                IIntValue v = IntValue.Read(parent, xr, false);
                if (v != null)
                {
                    if (value != null) throw Abort(xr, "multiple values");
                    value = v;
                }
            });
            if (value != null)
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
