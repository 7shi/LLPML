using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Pointer : VarBase
    {
        public override Addr32 Address
        {
            get { return parent.GetPointer(name).address; }
            set { parent.GetPointer(name).address = value; }
        }

        private int length = 0;
        public int Length { get { return length; } }

        public Pointer() { }
        public Pointer(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            name = xr["name"];
            Parse(xr, delegate
            {
                if (xr.NodeType == XmlNodeType.Element)
                {
                    int c = 1;
                    string count = xr["count"];
                    if (count != null) c = int.Parse(count);
                    switch (xr.Name)
                    {
                        case "int":
                            length = 4 * c;
                            break;
                        case "byte":
                            length = c;
                            break;
                        default:
                            throw Abort(xr);
                    }
                }
            });
            if (length > 0) parent.AddPointer(this);
        }
    }
}
