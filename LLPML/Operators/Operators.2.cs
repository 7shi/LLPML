using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Add : Operator
    {
        public override string Tag { get { return "add"; } }
        public Add(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Add(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            var ad = new Addr32(Reg32.ESP);
            var f = GetFunc();
            var v = values[0];
            if (!OpModule.NeedsDtor(v))
                v.AddCodes(codes, "push", null);
            else
            {
                v.AddCodes(codes, "mov", null);
                codes.Add(I386.Push(Reg32.EAX));
                if (OpModule.NeedsCtor(v)) codes.AddCtorCodes();
            }
            for (int i = 1; i < values.Count; i++)
                codes.AddOperatorCodes(f, ad, values[i], false);
            if (op != "push")
            {
                codes.Add(I386.Pop(Reg32.EAX));
                codes.AddCodes(op, dest);
            }
        }
    }

    public class Sub : Add
    {
        public override string Tag { get { return "sub"; } }
        public Sub(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Sub(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class And : Add
    {
        public override string Tag { get { return "and"; } }
        public And(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public And(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class Or : Add
    {
        public override string Tag { get { return "or"; } }
        public Or(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Or(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class Xor : Add
    {
        public override string Tag { get { return "xor"; } }
        public Xor(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Xor(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class ShiftLeft : Add
    {
        public override string Tag { get { return "shift-left"; } }
        public ShiftLeft(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public ShiftLeft(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class ShiftRight : Add
    {
        public override string Tag { get { return "shift-right"; } }
        public ShiftRight(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public ShiftRight(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class Mul : Add
    {
        public override string Tag { get { return "mul"; } }
        public Mul(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Mul(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class Div : Add
    {
        public override string Tag { get { return "div"; } }
        public Div(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Div(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class Mod : Add
    {
        public override string Tag { get { return "mod"; } }
        public Mod(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Mod(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
