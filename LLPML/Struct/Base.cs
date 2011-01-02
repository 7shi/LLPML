using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.LLPML.Parsing;

namespace Girl.LLPML.Struct
{
    public class Base : Var
    {
        public static Base New(BlockBase parent, SrcInfo si)
        {
            var ret = new Base();
            ret.Parent = parent;
            ret.name = "base";
            ret.Reference = parent.GetVar("this");
            ret.SrcInfo = si;
            return ret;
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
                    throw Abort("base: is not struct member: {0}", Parent.FullName);
                var bst = st.GetBaseStruct();
                if (bst == null)
                    throw Abort("base: has no base type: {0}", st.Name);
                type = Types.ToVarType(bst.Type);
                return type;
            }
        }
    }
}
