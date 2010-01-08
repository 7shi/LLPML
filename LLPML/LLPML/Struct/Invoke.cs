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
        public Invoke(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        private bool initialized = false;

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            if (!initialized && args.Count > 0)
            {
                IIntValue v = args[0];
                string type = null;
                if (v is Struct.MemberPtr)
                {
                    type = (v as Struct.MemberPtr).Type;
                }
                else if (v is Var)
                {
                    type = (v as Var).Reference.Type;
                }
                else if (v is Pointer)
                {
                    Struct.Declare st = (v as Pointer).Reference as Struct.Declare;
                    if (st != null) type = st.Type;
                }
                if (type == null)
                    throw new Exception("struct instance or pointer required: " + name);
                name = type + "::" + name;
                initialized = true;
            }
            base.AddCodes(codes, m);
        }
    }
}
