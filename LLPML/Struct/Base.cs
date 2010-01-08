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

        public override Struct.Define GetStruct()
        {
            return target.GetBaseStruct();
        }

        public override string Type
        {
            get
            {
                return target.BaseType;
            }
        }
    }
}
