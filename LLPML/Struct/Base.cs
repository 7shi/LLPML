using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.LLPML.Parsing;

namespace Girl.LLPML.Struct
{
    public class Base : Var
    {
        public Base(BlockBase parent) : base(parent) { Init(); }
        public Base(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        private void Init()
        {
            name = "base";
            Reference = parent.GetVar("this");
        }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            Init();
        }

        protected TypeBase type;
        protected bool doneInferType = false;

        public override TypeBase Type
        {
            get
            {
                if (doneInferType || !root.IsCompiling)
                    return type;

                doneInferType = true;
                var st = Reference.GetStruct();
                if (st == null)
                    throw Abort("base: is not struct member: {0}", parent.FullName);
                var bst = st.GetBaseStruct();
                if (bst == null)
                    throw Abort("base: has no base type: {0}", st.Name);
                type = new TypeReference(parent, bst.Type);
                return type;
            }
        }
    }
}
