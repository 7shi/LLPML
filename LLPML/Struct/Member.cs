using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Member : Var
    {
        private Struct.Declare ptr;
        public Struct.Declare Ptr
        {
            set
            {
                ptr = value;
                isRoot = true;
            }
        }

        private Var var;
        public Var Var
        {
            set
            {
                var = value;
                isRoot = true;
            }
        }

        private bool isRoot = true;
        private Member member;

        public Member(BlockBase parent, string name, Member member)
            : base(parent)
        {
            isRoot = false;
            this.name = name;
            this.member = member;
        }

        public Member(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public Member(Member parent, XmlTextReader xr)
        {
            this.parent = parent.parent;
            isRoot = false;
            SetLine(xr);
            Read(xr);
        }

        public override void Read(XmlTextReader xr)
        {
            RequiresName(xr);

            if (isRoot)
            {
                string ptr = xr["ptr"];
                string var = xr["var"];
                if (ptr == null)
                {
                    if (var == null) var = "this";
                    this.var = new Var(parent, var);
                }
                else if (var == null)
                {
                    this.ptr = parent.GetPointer(ptr) as Struct.Declare;
                    if (this.ptr == null)
                        throw Abort(xr, "undefined struct: " + ptr);
                }
                else
                    throw Abort(xr, "either src or var required");
            }

            Parse(xr, delegate
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xr.Name)
                        {
                            case "struct-member":
                                if (member != null)
                                    throw Abort(xr, "multiple members");
                                member = new Member(this, xr);
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
        }

        private Addr32 GetStructAddress(List<OpCode> codes, Module m)
        {
            if (ptr != null)
            {
                Addr32 ad = ptr.Address;
                if (parent.Level == ptr.Parent.Level || ad.IsAddress)
                {
                    return ad;
                }
                int lv = ptr.Parent.Level;
                if (lv <= 0 || lv >= parent.Level)
                {
                    throw Abort("Invalid variable scope: " + name);
                }
                codes.Add(I386.Mov(Reg32.EDX, new Addr32(Reg32.EBP, -lv * 4)));
                return new Addr32(Reg32.EDX, ad.Disp);
            }
            else if (var != null)
            {
                codes.Add(I386.Mov(Reg32.EDX, var.GetAddress(codes, m)));
                return new Addr32(Reg32.EDX);
            }
            else
                throw Abort("can not get address");
        }

        public int GetOffset(Define st)
        {
            int ret = st.GetOffset(name);
            if (ret < 0) throw Abort("undefined member: " + name);
            if (member == null) return ret;
            return ret + member.GetOffset(st.GetMember(name).GetStruct());
        }

        public override Addr32 GetAddress(List<OpCode> codes, Module m)
        {
            Addr32 ret = new Addr32(GetStructAddress(codes, m));
            Define st;
            if (ptr != null)
                st = ptr.GetStruct();
            else if (var != null)
                st = var.GetStruct();
            else
                throw Abort("can not get address");
            ret.Add(GetOffset(st));
            return ret;
        }

        public override string Type
        {
            get
            {
                Define st;
                if (ptr != null)
                    st = ptr.GetStruct();
                else
                    st = var.GetStruct();
                return st.GetMember(name).Type;
            }
        }
    }
}
