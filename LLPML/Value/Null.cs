using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Null : NodeBase, IIntValue
    {
        public Null(BlockBase parent) : base(parent, "null") { }
        public Null(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            name = "null";
        }

        public TypeBase Type { get { return null; } }

        public void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            codes.AddCodes(op, dest, Val32.New(0));
        }
    }
}
