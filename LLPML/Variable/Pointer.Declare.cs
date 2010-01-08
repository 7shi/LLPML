using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Pointer
    {
        public class Declare : DeclareBase
        {
            private int length = 0;
            public virtual int Length { get { return length; } }

            private string type;

            public Declare() { }

            public Declare(BlockBase parent, string name)
                : base(parent, name)
            {
                AddToParent();
            }

            public Declare(BlockBase parent, string name, int length)
                : this(parent, name)
            {
                this.length = length;
            }

            public Declare(BlockBase parent, XmlTextReader xr)
                : base(parent, xr)
            {
            }

            public override void Read(XmlTextReader xr)
            {
                NoChild(xr);
                RequiresName(xr);

                type = xr["type"];
                if (type == null) type = "byte";
                string slen = xr["length"];
                if (slen == null) throw Abort(xr, "length required");
                int len = IntValue.Parse(slen);

                switch (type)
                {
                    case "byte":
                        length = len;
                        break;

                    case "char":
                    case "short":
                        length = len * 2;
                        break;

                    case "int":
                        length = len * 4;
                        break;

                    case "long":
                        length = len * 8;
                        break;

                    default:
                        throw Abort(xr, "unknown type: " + type);
                }

                AddToParent();
            }

            protected virtual void AddToParent()
            {
                if (!parent.AddPointer(this))
                    throw Abort("multiple definitions: " + name);
            }
        }
    }
}
