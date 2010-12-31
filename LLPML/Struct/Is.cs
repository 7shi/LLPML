using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Is : Operator
    {
        public override string Tag { get { return "__type_is"; } }

        public override int Min { get { return 2; } }
        public override int Max { get { return 2; } }

        public Is(BlockBase parent, IIntValue v1, IIntValue v2) : base(parent, v1, v2) { }
        public Is(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            var f = Parent.GetFunction(Tag);
            if (f == null) throw Abort("is: can not find: {0}", Tag);

            TypeOf.AddCodes(this, Parent, values[1], codes, "push", null);
            TypeOf.AddCodes(this, Parent, values[0], codes, "push", null);
            codes.Add(I386.Call(f.First));
            codes.Add(I386.Add(Reg32.ESP, Val32.New(8)));
            codes.AddCodes(op, dest);
        }

        public override TypeBase Type
        {
            get { return TypeBool.Instance; }
        }

        public override IntValue GetConst()
        {
            return null;
        }
    }
}
