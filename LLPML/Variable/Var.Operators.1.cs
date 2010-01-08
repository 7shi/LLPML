using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Var
    {
        public class Add : Operator
        {
            public override string Tag { get { return "add"; } }

            public Add(BlockBase parent, Var dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Add(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

            private Addr32 Calculate(OpCodes codes)
            {
                var ad1 = dest.GetAddress(codes);
                var ad2 = ad1;
                var f = GetFunc();
                if (dest.Size < Var.DefaultSize)
                {
                    ad2 = new Addr32(Reg32.ESP);
                    dest.Type.AddGetCodes(codes, "push", null, ad1);
                }
                foreach (IIntValue v in values)
                    f(codes, ad2, v);
                if (dest.Size < Var.DefaultSize)
                {
                    codes.Add(I386.Pop(Reg32.EAX));
                    dest.Type.AddSetCodes(codes, ad1);
                }
                return ad1;
            }

            public override void AddCodes(OpCodes codes)
            {
                Calculate(codes);
            }

            public override void AddCodes(OpCodes codes, string op, Addr32 dest)
            {
                var ad = Calculate(codes);
                if (this.dest.Size < Var.DefaultSize)
                    codes.AddCodes(op, dest);
                else
                    codes.AddCodes(op, dest, ad);
            }
        }

        public class Sub : Add
        {
            public override string Tag { get { return "sub"; } }
            public Sub(BlockBase parent, Var dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Sub(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class And : Add
        {
            public override string Tag { get { return "and"; } }
            public And(BlockBase parent, Var dest, params IIntValue[] values) : base(parent, dest, values) { }
            public And(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class Or : Add
        {
            public override string Tag { get { return "or"; } }
            public Or(BlockBase parent, Var dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Or(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class Xor : Add
        {
            public override string Tag { get { return "xor"; } }
            public Xor(BlockBase parent, Var dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Xor(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class ShiftLeft : Add
        {
            public override string Tag { get { return "shift-left"; } }
            public ShiftLeft(BlockBase parent, Var dest, params IIntValue[] values) : base(parent, dest, values) { }
            public ShiftLeft(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class ShiftRight : ShiftLeft
        {
            public override string Tag { get { return "shift-right"; } }
            public ShiftRight(BlockBase parent, Var dest, params IIntValue[] values) : base(parent, dest, values) { }
            public ShiftRight(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class Mul : Add
        {
            public override string Tag { get { return "mul"; } }
            public Mul(BlockBase parent, Var dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Mul(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class Div : Add
        {
            public override string Tag { get { return "div"; } }
            public Div(BlockBase parent, Var dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Div(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class Mod : Add
        {
            public override string Tag { get { return "mod"; } }
            public Mod(BlockBase parent, Var dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Mod(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }
    }
}
