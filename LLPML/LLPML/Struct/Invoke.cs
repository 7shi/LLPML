using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Invoke : Call
    {
        public Invoke() { }
        public Invoke(Block parent, XmlTextReader xr) : base(parent, xr) { }

        protected override void AddArg(XmlTextReader xr)
        {
            IIntValue v = IntValue.Read(parent, xr, false);
            if (v == null) return;

            if (args.Count == 0)
            {
                string type = null;
                if (v is Var)
                {
                    type = (v as Var).Reference.Type;
                }
                else if (v is Pointer)
                {
                    Struct.Declare st = (v as Pointer).Reference as Struct.Declare;
                    if (st != null) type = st.Type;
                }
                if (type == null)
                    throw Abort(xr, "struct instance or pointer required");
                name = type + "::" + name;
            }
            args.Add(v);
        }
    }
}
