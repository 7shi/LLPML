using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Inc : Var.Operator, IIntValue
    {
        public override int Min { get { return 0; } }
        public override int Max { get { return 0; } }

        public Inc() { }
        public Inc(Block parent, Var dest) : base(parent, dest) { }
        public Inc(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            codes.Add(I386.Inc(dest.GetAddress(codes, m)));
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            Addr32 ad = new Addr32(Reg32.EAX);
            codes.AddRange(new OpCode[]
            {
                I386.Lea(Reg32.EAX, this.dest.GetAddress(codes, m)),
                I386.Inc(ad),
                I386.Mov(Reg32.EAX, ad)
            });
            IntValue.AddCodes(codes, op, dest);
        }
    }
}
