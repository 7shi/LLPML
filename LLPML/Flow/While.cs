using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class While : Do
    {
        public While(BlockBase parent) : base(parent) { }
        public While(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        protected override void BeforeAddCodes(List<OpCode> codes, Module m)
        {
            base.BeforeAddCodes(codes, m);
            codes.Add(I386.Jmp(Block.Last));
        }
    }
}