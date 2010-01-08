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

            public Add(BlockBase parent, IIntValue dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Add(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

            private Addr32 Calculate(OpModule codes)
            {
                var dest = Var.Get(this.dest);
                if (dest == null)
                    throw Abort("{0}: destination is not variable", Tag);
                var ad = dest.GetAddress(codes);
                var ad2 = ad;
                var f = GetFunc();
                TypeBase.Func schar = null, sint = null;
                if (dest.Type is TypeString)
                {
                    schar = dest.Type.GetFunc(Tag + "-char");
                    sint = dest.Type.GetFunc(Tag + "-int");
                }
                var size = dest.Type.Size;
                var cleanup = OpModule.NeedsDtor(dest);
                var indirect = (dest.Reference != null && dest.Reference.Parent != Parent)
                    || size < Var.DefaultSize || cleanup;
                if (indirect)
                {
                    if (!ad.IsAddress && ad.Register != Reg32.EBP)
                        codes.Add(I386.Push(ad.Register));
                    dest.Type.AddGetCodes(codes, "push", null, ad);
                    ad2 = new Addr32(Reg32.ESP);
                }
                foreach (IIntValue v in values)
                {
                    var vv = v;
                    var ff = f;
                    if (schar != null && vv.Type is TypeChar)
                        ff = schar;
                    else if (sint != null && vv.Type is TypeIntBase)
                        ff = sint;
                    codes.AddOperatorCodes(ff, ad2, vv, false);
                }
                if (indirect)
                {
                    codes.Add(I386.Pop(Reg32.EAX));
                    if (!ad.IsAddress && ad.Register != Reg32.EBP)
                        codes.Add(I386.Pop(ad.Register));
                    codes.Add(I386.Mov(ad, Reg32.EAX));
                }
                return ad;
            }

            public override void AddCodes(OpModule codes)
            {
                Calculate(codes);
            }

            public override void AddCodes(OpModule codes, string op, Addr32 dest)
            {
                var ad = Calculate(codes);
                if (this.dest.Type.Size < Var.DefaultSize)
                    codes.AddCodes(op, dest);
                else
                    codes.AddCodes(op, dest, ad);
            }
        }

        public class Sub : Add
        {
            public override string Tag { get { return "sub"; } }
            public Sub(BlockBase parent, IIntValue dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Sub(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class And : Add
        {
            public override string Tag { get { return "and"; } }
            public And(BlockBase parent, IIntValue dest, params IIntValue[] values) : base(parent, dest, values) { }
            public And(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class Or : Add
        {
            public override string Tag { get { return "or"; } }
            public Or(BlockBase parent, IIntValue dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Or(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class Xor : Add
        {
            public override string Tag { get { return "xor"; } }
            public Xor(BlockBase parent, IIntValue dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Xor(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class ShiftLeft : Add
        {
            public override string Tag { get { return "shift-left"; } }
            public ShiftLeft(BlockBase parent, IIntValue dest, params IIntValue[] values) : base(parent, dest, values) { }
            public ShiftLeft(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class ShiftRight : ShiftLeft
        {
            public override string Tag { get { return "shift-right"; } }
            public ShiftRight(BlockBase parent, IIntValue dest, params IIntValue[] values) : base(parent, dest, values) { }
            public ShiftRight(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class Mul : Add
        {
            public override string Tag { get { return "mul"; } }
            public Mul(BlockBase parent, IIntValue dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Mul(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class Div : Add
        {
            public override string Tag { get { return "div"; } }
            public Div(BlockBase parent, IIntValue dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Div(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }

        public class Mod : Add
        {
            public override string Tag { get { return "mod"; } }
            public Mod(BlockBase parent, IIntValue dest, params IIntValue[] values) : base(parent, dest, values) { }
            public Mod(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        }
    }
}
