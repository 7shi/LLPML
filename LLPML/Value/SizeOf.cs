using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class SizeOf : IntValue
    {
        public IIntValue target;

        public SizeOf(BlockBase parent, IIntValue target)
            : base(parent)
        {
            this.target = target;
        }

        public SizeOf(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequiresName(xr);
        }

        public override int Value
        {
            get
            {
                if (target is Variant)
                {
                    var v = Parent.GetVar((target as Variant).Name);
                    if (v != null)
                    {
                        var vt = v.Type;
                        if (vt is TypeArray) return vt.Size;
                    }
                }
                TypeBase t = null;
                try
                {
                    t = TypeOf.GetType(Parent, target);
                }
                catch { }
                if (t != null) return t.Size;
                throw Abort("undefined type: " + name);
            }
        }
    }
}
