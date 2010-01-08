using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Arg : Var.Declare
    {
        public Arg()
        {
        }

        public Arg(Block parent, string name, string type)
            : base(parent, name)
        {
            this.type = type;
        }

        public Arg(Block parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequiresName(xr);

            type = xr["type"];
            parent.AddVar(this);
        }
    }
}
