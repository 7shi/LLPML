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
        private Struct.Declare src;
        private Var var;
        private bool isRoot = true;
        private Member member;

        public Member(Block parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public Member(Member parent, XmlTextReader xr)
        {
            this.parent = parent.parent;
            isRoot = false;
            Read(xr);
        }

        public override void Read(XmlTextReader xr)
        {
            RequiresName(xr);

            if (isRoot)
            {
                string src = xr["src"];
                string var = xr["var"];
                if (src == null && var == null)
                    throw Abort(xr, "src or var required");
                else if (src != null && var == null)
                {
                    this.src = parent.GetPointer(src) as Struct.Declare;
                    if (this.src == null)
                        throw Abort(xr, "undefined struct: " + src);
                }
                else if (src == null && var != null)
                {
                    this.var = new Var(parent, var);
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
            if (src != null)
            {
                Addr32 ad = src.Address;
                if (parent.Level == src.Parent.Level || ad.IsAddress)
                {
                    return ad;
                }
                int lv = src.Parent.Level;
                if (lv <= 0 || lv >= parent.Level)
                {
                    throw new Exception("Invalid variable scope: " + name);
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
                throw new Exception("can not get address");
        }

        public int GetOffset(Define st)
        {
            int ret = st.GetOffset(name);
            if (ret < 0) throw new Exception("undefined member: " + name);
            if (member == null) return ret;
            return ret + member.GetOffset(st.GetMeber(name).GetStruct());
        }

        public override Addr32 GetAddress(List<OpCode> codes, Module m)
        {
            Addr32 ret = new Addr32(GetStructAddress(codes, m));
            Define st;
            if (src != null)
                st = src.GetStruct();
            else if (var != null)
                st = var.Reference.GetStruct();
            else
                throw new Exception("can not get address");
            ret.Add(GetOffset(st));
            return ret;
        }

        public string Type
        {
            get
            {
                return src.GetStruct().GetMeber(name).Type;
            }
        }
    }
}
