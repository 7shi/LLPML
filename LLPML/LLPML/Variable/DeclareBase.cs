using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.X86;

namespace Girl.LLPML
{
    public class DeclareBase : NodeBase
    {
        protected Addr32 address;
        public Addr32 Address
        {
            get { return address; }
            set { address = value; }
        }

        public DeclareBase() { }
        public DeclareBase(Block parent, string name) : base(parent, name) { }
        public DeclareBase(Block parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
