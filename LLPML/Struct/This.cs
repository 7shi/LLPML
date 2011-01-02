using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Girl.LLPML.Struct
{
    public class This : Var
    {
        public This(BlockBase parent)
            : base(parent)
        {
            name = "this";
            Reference = Parent.GetVar(name);
        }
    }
}
