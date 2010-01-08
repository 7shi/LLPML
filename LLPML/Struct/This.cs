using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Girl.LLPML.Struct
{
    public class This : Var
    {
        public This(BlockBase parent) : base(parent) { Init(); }
        public This(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        private void Init()
        {
            name = "this";
            Reference = Parent.GetVar(name);
        }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            Init();
        }
    }
}
