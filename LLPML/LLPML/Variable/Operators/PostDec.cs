using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class PostDec : Var.Operator, IIntValue
    {
        public override int Min { get { return 0; } }
        public override int Max { get { return 0; } }

        public PostDec() { }
        public PostDec(BlockBase parent, Var dest) : base(parent, dest) { }
        public PostDec(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            codes.Add(I386.Dec(dest.GetAddress(codes, m)));
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            Addr32 ad = new Addr32(Reg32.EDX);
            codes.AddRange(new OpCode[]
            {
                I386.Lea(Reg32.EDX, this.dest.GetAddress(codes, m)),
                I386.Mov(Reg32.EAX, ad),
                I386.Dec(ad)
            });
            IntValue.AddCodes(codes, op, dest);
        }
    }
}
