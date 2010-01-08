using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Loop : Block
    {
        private VarInt count;
        private OpCode start;

        public Loop() { }
        public Loop(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            this.count = null;
            string count = xr["count"];
            if (count != null)
            {
                this.count = new VarInt(this, "__loop_counter", int.Parse(xr["count"]));
            }
            base.Read(xr);
        }

        protected override void BeforeAddCodes(List<OpCode> codes, Module m)
        {
            base.BeforeAddCodes(codes, m);
            count.AddCodes(codes, m);
            start = new OpCode();
            codes.Add(start);
        }

        protected override void AfterAddCodes(List<OpCode> codes, Module m)
        {
            if (count != null)
            {
                codes.Add(I386.Dec(count.Address));
                codes.Add(I386.Jnz(start.Address));
            }
            else
            {
                codes.Add(I386.Jmp(start.Address));
            }
            base.AfterAddCodes(codes, m);
        }
    }
}
