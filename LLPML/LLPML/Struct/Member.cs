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
        private string member;

        public Member(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequiresName(xr);

            src = parent.GetPointer(name) as Struct.Declare;
            if (src == null) throw Abort(xr, "undefined struct: " + name);

            member = xr["member"];
            if (member == null) throw Abort(xr, "member required");
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

        public override Addr32 GetAddress(List<OpCode> codes, Module m)
        {
            Struct.Define st = src.GetStruct();
            int pos = st.GetOffset(member);
            if (pos < 0) throw new Exception("undefined member: " + member);

            Addr32 ret = new Addr32(GetStructAddress(codes, m));
            ret.Add(pos);
            return ret;
        }
    }
}
