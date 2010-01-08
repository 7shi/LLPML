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
        private string memberName;

        public Method(Struct.Define parent, XmlTextReader xr)
        {
            st = parent;
            this.parent = parent.Parent;
            Read(xr);
        }

        protected override void ReadName(XmlTextReader xr)
        {
            memberName = xr["name"];
            if (memberName == null) throw Abort(xr, "name required");
            name = st.GetMemberName(memberName);

            string sta = xr["static"];
            if (sta == null || sta != "true")
                args.Add(new Arg(this, "this", st.Name));
        }
    }
}
