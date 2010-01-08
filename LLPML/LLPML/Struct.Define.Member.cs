using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Girl.LLPML
{
    public partial class Struct : VarBase
    {
        public partial class Define : NodeBase
        {
            public class Member : NodeBase
            {
                protected string name;
                public string Name { get { return name; } }

                protected int size = sizeof(int);
                public int Size { get { return size; } }

                public Member() { }
                public Member(Block parent, XmlTextReader xr) : base(parent, xr) { }

                public override void Read(XmlTextReader xr)
                {
                    NoChild(xr);

                    name = xr["name"];
                    if (name == null) throw Abort(xr, "name required");
                }
            }
        }
    }
}
