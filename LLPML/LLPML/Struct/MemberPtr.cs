using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class MemberPtr : NodeBase, IIntValue
    {
        private Member src;

        public MemberPtr() { }
        public MemberPtr(Member src) { this.src = src; }
        public MemberPtr(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            src = new Member(parent, xr);
        }

        public Addr32 GetAddress(List<OpCode> codes, Module m)
        {
            return src.GetAddress(codes, m);
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            codes.Add(I386.Lea(Reg32.EAX, GetAddress(codes, m)));
            IntValue.AddCodes(codes, op, dest);
        }

        public string Type { get { return src.Type; } }
    }
}
