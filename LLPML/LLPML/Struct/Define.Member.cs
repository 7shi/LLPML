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
            protected string type;
            public string Type { get { return type; } }

            public Member() { }
            public Member(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

            public override void Read(XmlTextReader xr)
            {
                NoChild(xr);
                RequiresName(xr);
                type = xr["type"];
            }

            public int GetSize()
            {
                if (type == null) return sizeof(int);
                return GetStruct().GetSize();
            }

            public Define GetStruct()
            {
                if (type == null) return null;
                Define st = parent.GetStruct(type);
                if (st == null) throw Abort("undefined type: " + type);
                return st;
            }
        }
    }
}
