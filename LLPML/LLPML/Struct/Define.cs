using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Girl.LLPML.Struct
{
    public partial class Define : NodeBase
    {
        protected List<Member> members = new List<Member>();

        public Define() { }
        public Define(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            RequireName(xr);
            Parse(xr, delegate
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xr.Name)
                        {
                            case "member":
                                members.Add(new Member(parent, xr));
                                break;
                            default:
                                throw Abort(xr);
                        }
                        break;

                    case XmlNodeType.Whitespace:
                    case XmlNodeType.Comment:
                        break;

                    default:
                        throw Abort(xr, "element required");
                }
            });
            if (members.Count == 0) throw Abort(xr, "member required");
            parent.AddStruct(this);
        }

        public int GetSize()
        {
            int ret = 0;
            foreach (Member m in members)
            {
                ret += m.Size;
            }
            return ret;
        }

        public int GetOffset(string name)
        {
            int ret = 0;
            foreach (Member m in members)
            {
                if (m.Name == name) return ret;
                ret += m.Size;
            }
            return -1;
        }
    }
}