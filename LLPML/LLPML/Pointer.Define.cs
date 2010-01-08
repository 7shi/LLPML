using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Pointer : VarBase
    {
        public class Define : DefineBase
        {
            private int length = 0;
            public int Length { get { return length; } }

            public Pointer Reference;

            public Define() { }

            public Define(Block parent, string name, int length)
                : base(parent, name)
            {
                this.length = length;
                parent.AddPointer(this);
            }

            public Define(Block parent, XmlTextReader xr)
                : base(parent, xr)
            {
            }

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
                                length = c * 4;
                                break;
                            case "char":
                                length = c * 2;
                                break;
                            case "byte":
                                length = c;
                                break;
                            default:
                                throw Abort(xr);
                        }
                    }
                });
                parent.AddPointer(this);
            }
        }
    }
}
