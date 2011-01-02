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
    public abstract class Operator1 : Operator
    {
        protected abstract int Calculate(int v);

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
            return IntValue.New(Calculate(v.Value));
        }
    }

    public class Neg : Operator1
    {
        public override string Tag { get { return "neg"; } }
        protected override int Calculate(int v) { return -v; }

        public static Neg New(BlockBase parent, NodeBase arg, SrcInfo si)
        {
            return Init3(new Neg(), parent, arg, null, si) as Neg;
        }
    }

    public class Rev : Operator1
    {
        public override string Tag { get { return "rev"; } }
        protected override int Calculate(int v) { return ~v; }

        public static Rev New(BlockBase parent, NodeBase arg, SrcInfo si)
        {
            return Init3(new Rev(), parent, arg, null, si) as Rev;
        }
    }

    public class Not : Operator1
    {
        public override string Tag { get { return "not"; } }
        public override TypeBase Type { get { return TypeBool.Instance; } }
        protected override int Calculate(int v) { return v != 0 ? 0 : 1; }

        public static Not New(BlockBase parent, NodeBase arg, SrcInfo si)
        {
            return Init3(new Not(), parent, arg, null, si) as Not;
        }
    }
}
