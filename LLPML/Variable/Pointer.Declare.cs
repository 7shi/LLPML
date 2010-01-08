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
            public virtual TypeBase Type { get { return Types.GetType(TypeName); } }
            public virtual string TypeName { get; set; }
            public int TypeSize { get; protected set; }
            public int Count { get; private set; }

            private int length = 0;
            public virtual int Length { get { return length; } }

            public Declare() { }

            public Declare(BlockBase parent, string name)
                : base(parent, name)
            {
                AddToParent();
            }

            public Declare(BlockBase parent, string name, string type, int count)
                : this(parent, name)
            {
                this.TypeName = type;
                TypeSize = SizeOf.GetTypeSize(parent, type);
                Count = count;
                this.length = TypeSize * count;
            }

            public Declare(BlockBase parent, string name, int count)
                : this(parent, name, "byte", count)
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

                var type = xr["type"];
                if (type == null) type = "byte";
                TypeName = type;

                TypeSize = SizeOf.GetTypeSize(parent, TypeName);
                if (TypeSize == 0) throw Abort(xr, "unknown type: " + TypeName);

                string slen = xr["length"];
                if (slen == null) throw Abort(xr, "length required");
                Count = IntValue.Parse(slen);

                length = TypeSize * Count;
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
