using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Member : VarInt
    {
        private Struct.Declare src;
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
                if (src == null) throw Abort(xr, "source required");
                this.src = parent.GetPointer(src) as Struct.Declare;
                if (this.src == null) throw Abort(xr, "undefined struct: " + src);
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
            codes.Add(I386.Mov(Reg32.EAX, new Addr32(Reg32.EBP, -lv * 4)));
            return new Addr32(Reg32.EAX, ad.Disp);
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
            ret.Add(GetOffset(src.GetStruct()));
            return ret;
        }
    }
}
