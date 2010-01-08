using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Neg : Operator
    {
        public override string Tag { get { return "neg"; } }

        public override int Min { get { return 1; } }
        public override int Max { get { return 1; } }

        public Neg(BlockBase parent, IIntValue value) : base(parent, value) { }
        public Neg(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            codes.AddOperatorCodes(GetFunc(), dest, values[0], false);
            codes.AddCodes(op, dest);
        }
    }

    public class Rev : Neg
    {
        public override string Tag { get { return "rev"; } }
        public Rev(BlockBase parent, IIntValue value) : base(parent, value) { }
        public Rev(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class Not : Neg
    {
        public override string Tag { get { return "not"; } }
        public override TypeBase Type { get { return TypeBool.Instance; } }
        public Not(BlockBase parent, IIntValue value) : base(parent, value) { }
        public Not(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
