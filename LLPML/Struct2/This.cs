using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Girl.LLPML.Struct2
{
    public class This : Var
    {
        public This(BlockBase parent) : base(parent) { Reference = parent.GetVar(name = "this"); }
        public This(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            name = "this";

            Reference = parent.GetVar(name);
        }
    }
}
