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

        protected string baseType;
        public string BaseType { get { return baseType; } }

        public Define() { }
        public Define(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            RequiresName(xr);

            baseType = xr["base"];
            if (name == baseType)
                throw Abort(xr, "can not define recursive base type: " + name);

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

            if (!parent.AddStruct(this))
                throw Abort("multiple definitions: " + name);
        }

        public int GetSize()
        {
            int ret = 0;
            Define st = GetBaseStruct();
            if (st != null) ret = st.GetSize();
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
            Define st = GetBaseStruct();
            if (st != null) ret = st.GetSize();
            foreach (Member m in members)
            {
                if (m.Name == name) return ret;
                ret += m.GetSize();
            }
            if (st != null) return st.GetOffset(name);
            return -1;
        }

        public Member GetMember(string name)
        {
            foreach (Member m in members)
            {
                if (m.Name == name) return m;
            }
            Define st = GetBaseStruct();
            if (st != null) return st.GetMember(name);
            return null;
        }

        public Member[] GetMembers()
        {
            List<Member> list = new List<Member>();
            Define st = GetBaseStruct();
            if (st != null) list.AddRange(st.GetMembers());
            list.AddRange(members);
            return list.ToArray();
        }

        public string GetMemberName(string name)
        {
            return this.name + "::" + name;
        }

        public Method GetMethod(string name)
        {
            Method ret = parent.GetFunction(GetMemberName(name)) as Method;
            if (ret != null) return ret;
            Define st = GetBaseStruct();
            if (st != null) return st.GetMethod(name);
            return null;
        }

        public void AddConstructor(List<OpCode> codes, Module m, Addr32 ad)
        {
            Define st = GetBaseStruct();
            if (st != null) st.AddConstructor(codes, m, ad);

            Addr32 ad2 = new Addr32(ad);
            if (st != null) ad2.Add(st.GetSize());
            foreach (Member mem in members)
            {
                Define memst = mem.GetStruct();
                if (memst != null) memst.AddConstructor(codes, m, ad2);
                ad2.Add(mem.GetSize());
            }

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

            Define st = GetBaseStruct();
            Addr32 ad2 = new Addr32(ad);
            if (st != null) ad2.Add(st.GetSize());
            foreach (Member mem in members)
            {
                Define memst = mem.GetStruct();
                if (memst != null) memst.AddDestructor(codes, m, ad2);
                ad2.Add(mem.GetSize());
            }
            if (st != null) st.AddDestructor(codes, m, ad);
        }

        public Define GetBaseStruct()
        {
            if (baseType == null) return null;
            Define st = parent.GetStruct(baseType);
            if (st != null) return st;
            throw Abort("undefined struct: " + baseType);
        }

        public void CheckStruct()
        {
            CheckBaseStruct(name);
            foreach (Member mem in members)
            {
                Define st = mem.GetStruct();
                if (st != null) st.CheckStruct(name);
            }
        }

        public void CheckStruct(string type)
        {
            if (type == name)
                throw Abort("can not define recursive type: " + name);
            foreach (Member mem in members)
            {
                Define st = mem.GetStruct();
                if (st != null) st.CheckStruct(type);
            }
        }

        public void CheckBaseStruct(string name)
        {
            Define b = GetBaseStruct();
            if (b == null) return;

            if (b.name == name)
                throw Abort("can not define recursive base type: " + name);
            b.CheckBaseStruct(name);
        }
    }
}
