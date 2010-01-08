using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class VarBase : NodeBase
    {
        protected string name;
        public string Name { get { return name; } }

        public VarBase()
        {
        }

        public VarBase(Block parent, string name)
            : base(parent)
        {
            this.name = name;
        }

        public VarBase(Block parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public class DefineBase : VarBase
        {
            protected Addr32 address;
            public Addr32 Address
            {
                get { return address; }
                set { address = value; }
            }

            public DefineBase() { }
            public DefineBase(Block parent, string name) : base(parent, name) { }
            public DefineBase(Block parent, XmlTextReader xr) : base(parent, xr) { }
        }
    }
}
