using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class For : Block
    {
        private string name;
        private VarInt count;
        private int to, step;
        private OpCode start, end;

        public For() { }
        public For(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            this.count = null;
            name = xr["name"];
            if (name == null) name = "__loop_counter";
            string from = xr["from"], to = xr["to"], step = xr["step"];
            if (from != null && to != null)
            {
                this.to = int.Parse(to);
                this.step = step == null ? 1 : int.Parse(step);
                this.count = new VarInt(this, name, int.Parse(from));
            }
            base.Read(xr);
        }

        protected override void BeforeAddCodes(List<OpCode> codes, Module m)
        {
            base.BeforeAddCodes(codes, m);
            start = new OpCode();
            end = new OpCode();
            if (count != null) count.AddCodes(codes, m);
            codes.Add(start);
            if (count != null)
            {
                codes.Add(I386.Cmp(count.Address, (uint)to));
                if (step > 0)
                {
                    codes.Add(I386.Jcc(Cond.G, end.Address));
                }
                else if (step < 0)
                {
                    codes.Add(I386.Jcc(Cond.L, end.Address));
                }
            }
        }

        protected override void AfterAddCodes(List<OpCode> codes, Module m)
        {
            if (count != null)
            {
                if (step == 1)
                    codes.Add(I386.Inc(count.Address));
                else if (step == -1)
                    codes.Add(I386.Dec(count.Address));
                else if (step > 1)
                    codes.Add(I386.Add(count.Address, (uint)step));
                else if (step < -1)
                    codes.Add(I386.Sub(count.Address, (uint)-step));
            }
            codes.Add(I386.Jmp(start.Address));
            codes.Add(end);
            base.AfterAddCodes(codes, m);
        }
    }
}
