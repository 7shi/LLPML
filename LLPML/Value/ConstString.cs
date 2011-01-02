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
        public static ConstString New(BlockBase parent, string value)
        {
            var ret = new ConstString();
            ret.Parent = parent;
            ret.Value = value;
            return ret;
        }
    }
}
