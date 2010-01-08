using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public partial class Method : Function
    {
        private Struct.Define st;
        public Struct.Define GetStruct() { return st; }

        private bool isStatic;
        public bool IsStatic { get { return isStatic; } }

        private string memberName;

        public Method(Struct.Define parent, XmlTextReader xr)
        {
            st = parent;
            this.parent = parent.Parent;
            SetLine(xr);
            Read(xr);
        }

        protected override void ReadName(XmlTextReader xr)
        {
            memberName = xr["name"];
            if (memberName == null) throw Abort(xr, "name required");
            name = st.GetMemberName(memberName);

            isStatic = "true" == xr["static"];
            if (!isStatic) args.Add(new Arg(this, "this", st.Name));
        }
    }
}
