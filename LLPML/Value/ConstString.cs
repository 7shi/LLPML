using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class ConstString : StringValue
    {
        public ConstString(BlockBase parent, string value)
            : base(parent, value)
        {
        }
    }
}
