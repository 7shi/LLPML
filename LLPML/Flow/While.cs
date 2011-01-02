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
        public static new While New(BlockBase parent)
        {
            var ret = new While();
            ret.init(parent);
            return ret;
        }

        protected override void BeforeAddCodes(OpModule codes)
        {
            base.BeforeAddCodes(codes);
            codes.Add(I386.JmpD(Block.Last));
        }
    }
}
