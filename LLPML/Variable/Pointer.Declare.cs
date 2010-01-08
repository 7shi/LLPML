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
            public int TypeSize { get; private set; }

            private int length = 0;
            public virtual int Length { get { return length; } }

            protected string type;

            public Declare() { }

            public Declare(BlockBase parent, string name)
                : base(parent, name)
            {
                AddToParent();
            }

            public Declare(BlockBase parent, string name, string type, int length)
                : this(parent, name)
            {
                this.type = type;
                TypeSize = Size.GetTypeSize(parent, type);
                this.length = TypeSize * length;
            }

            public Declare(BlockBase parent, string name, int length)
                : this(parent, name, "byte", length)
            {
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
                TypeSize = Size.GetTypeSize(parent, type);
                if (TypeSize == 0) throw Abort(xr, "unknown type: " + type);
                length = TypeSize * len;

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
