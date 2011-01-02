using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.LLPML.Parsing;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class SizeOf : IntValue
    {
        public NodeBase target;

        public static SizeOf New(BlockBase parent, NodeBase target, SrcInfo si)
        {
            var ret = new SizeOf();
            ret.Parent = parent;
            ret.target = target;
            ret.SrcInfo = si;
            return ret;
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
