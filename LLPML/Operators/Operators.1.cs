using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Not : Operator
    {
        public override string Tag { get { return "not"; } }

        public override int Min { get { return 1; } }
        public override int Max { get { return 1; } }

        public Not(BlockBase parent, IIntValue value) : base(parent, value) { }
        public Not(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            values[0].AddCodes(codes, "mov", null);
            GetFunc()(codes, dest, values[0]);
            codes.AddCodes(op, dest);
        }
    }

    public class Neg : Not
    {
        public override string Tag { get { return "neg"; } }
        public Neg(BlockBase parent, IIntValue value) : base(parent, value) { }
        public Neg(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class Rev : Not
    {
        public override string Tag { get { return "rev"; } }
        public Rev(BlockBase parent, IIntValue value) : base(parent, value) { }
        public Rev(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
