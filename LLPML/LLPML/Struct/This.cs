using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Girl.LLPML.Struct
{
    public partial class This : Var
    {
        public This(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            name = "this";

            reference = parent.GetVar(name);
            if (reference == null)
                throw Abort(xr, "undefined variable: " + name);
        }
    }
}
