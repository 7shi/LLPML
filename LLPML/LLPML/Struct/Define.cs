using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public partial class Define : NodeBase
    {
        protected List<Member> members = new List<Member>();
        public List<Member> Members { get { return members; } }

        public Define() { }
        public Define(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            RequiresName(xr);
            Parse(xr, delegate
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xr.Name)
                        {
                            case "member":
                                {
                                    Member mem = new Member(parent, xr);
                                    if (mem.Type == name)
                                        throw Abort(xr, "can not define recursive type: " + name);
                                    members.Add(mem);
                                    break;
                                }
                            case "method":
                                new Method(this, xr);
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
            parent.AddStruct(this);
        }

        public int GetSize()
        {
            int ret = 0;
            foreach (Member m in members)
            {
                ret += m.GetSize();
            }
            if (ret == 0) ret = 4;
            return ret;
        }

        public int GetOffset(string name)
        {
            int ret = 0;
            foreach (Member m in members)
            {
                if (m.Name == name) return ret;
                ret += m.GetSize();
            }
            return -1;
        }

        public Member GetMember(string name)
        {
            foreach (Member m in members)
            {
                if (m.Name == name) return m;
            }
            return null;
        }

        public string GetMemberName(string name)
        {
            return this.name + "::" + name;
        }

        public Method GetMethod(string name)
        {
            return parent.GetFunction(GetMemberName(name)) as Method;
        }

        public void AddConstructor(List<OpCode> codes, Module m, Addr32 ad)
        {
            Method ctor = GetMethod("this");
            if (ctor == null) return;

            codes.AddRange(new OpCode[]
            {
                I386.Lea(Reg32.EAX, new Addr32(ad)),
                I386.Push(Reg32.EAX),
                I386.Call(ctor.First)
            });
            if (ctor.Type == CallType.CDecl)
            {
                codes.Add(I386.Add(Reg32.ESP, 4));
            }
        }

        public void AddDestructor(List<OpCode> codes, Module m, Addr32 ad)
        {
            Method dtor = GetMethod("~this");
            if (dtor != null)
            {
                codes.AddRange(new OpCode[]
                {
                    I386.Lea(Reg32.EAX, new Addr32(ad)),
                    I386.Push(Reg32.EAX),
                    I386.Call(dtor.First)
                });
                if (dtor.Type == CallType.CDecl)
                {
                    codes.Add(I386.Add(Reg32.ESP, 4));
                }
            }

            Addr32 ad2 = new Addr32(ad);
            foreach (Member mem in members)
            {
                Define memst = mem.GetStruct();
                if (memst != null) memst.AddDestructor(codes, m, ad2);
                ad2.Add(mem.GetSize());
            }
        }
    }
}
