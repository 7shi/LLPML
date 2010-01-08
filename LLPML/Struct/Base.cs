using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Girl.LLPML.Struct
{
    public class Base : Var
    {
        private Struct.Define target;

        public Base(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            name = "base";

            target = parent.ThisStruct;
            if (target.BaseType == null)
                throw Abort(xr, "has no base type: " + target.Name);
        }

        public override TypeBase Type
        {
            get
            {
                return Types.GetType(parent, target.BaseType);
            }
        }
    }
}
