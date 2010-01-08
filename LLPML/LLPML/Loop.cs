using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.X86;

namespace Girl.LLPML
{
    public class Loop : Block
    {
        public int? count;

        public Loop() { }
        public Loop(Root root, XmlTextReader xr) { Read(root, xr); }

        public override void Read(Root root, XmlTextReader xr)
        {
            this.count = null;
            string count = xr["count"];
            if (count != null) this.count = int.Parse(xr["count"]);
            base.Read(root, xr);
        }

        public override void AddCodes(List<OpCode> codes, Girl.PE.Module m)
        {
            if (count != null) codes.Add(I386.Push((uint)count));
            OpCode start = new OpCode();
            codes.Add(start);
            base.AddCodes(codes, m);
            if (count != null)
            {
                codes.Add(I386.Dec(new Addr32(Reg32.ESP)));
                codes.Add(I386.Jnz(start.Address));
                codes.Add(I386.Add(Reg32.ESP, 4));
            }
            else
            {
                codes.Add(I386.Jmp(start.Address));
            }
        }
    }
}
