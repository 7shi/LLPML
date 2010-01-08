using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Size : NodeBase, IIntValue
    {
        public Size() { }
        public Size(BlockBase parent, string name) : base(parent, name) { }
        public Size(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequiresName(xr);
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            Define st = parent.GetStruct(name);
            if (st == null) throw Abort("undefined struct: " + name);

            IntValue.AddCodes(codes, op, dest, (uint)st.GetSize());
        }
    }
}
