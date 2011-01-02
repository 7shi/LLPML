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

        public Neg(BlockBase parent, NodeBase value) : base(parent, value) { }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            if (AddConstCodes(codes, op, dest)) return;

            codes.AddOperatorCodes(GetFunc(), dest, values[0], false);
            codes.AddCodes(op, dest);
        }

        public override IntValue GetConst()
        {
            var v = IntValue.GetValue(values[0]);
            if (v == null) return null;
            return new IntValue(Calculate(v.Value));
        }

        protected virtual int Calculate(int v) { return -v; }
    }

    public class Rev : Neg
    {
        public override string Tag { get { return "rev"; } }
        public Rev(BlockBase parent, NodeBase value) : base(parent, value) { }
        protected override int Calculate(int v) { return ~v; }
    }

    public class Not : Neg
    {
        public override string Tag { get { return "not"; } }
        public override TypeBase Type { get { return TypeBool.Instance; } }
        public Not(BlockBase parent, NodeBase value) : base(parent, value) { }
        protected override int Calculate(int v) { return v != 0 ? 0 : 1; }
    }
}
