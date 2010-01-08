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
        public Size(Block parent, string name) : base(parent, name) { }
        public Size(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequireName(xr);
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            Define st = parent.GetStruct(name);
            if (st == null) throw new Exception("undefined struct: " + name);

            IntValue.AddCodes(codes, op, dest, (uint)st.GetSize());
        }
    }
}
