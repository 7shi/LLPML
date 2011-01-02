using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Girl.LLPML.Struct
{
    public class This : Var
    {
        public static This New(BlockBase parent)
        {
            var ret = new This();
            ret.Parent = parent;
            ret.name = "this";
            ret.Reference = parent.GetVar(ret.name);
            return ret;
        }
    }
}
