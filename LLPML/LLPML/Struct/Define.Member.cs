using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Girl.LLPML.Struct
{
    public partial class Define : NodeBase
    {
        public class Member : NodeBase
        {
            protected int size = sizeof(int);
            public int Size { get { return size; } }

            public Member() { }
            public Member(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void Read(XmlTextReader xr)
            {
                NoChild(xr);
                RequireName(xr);
            }
        }
    }
}
