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
            var tr = v.Type as TypeReference;
            if (tr != null && tr.UseGC && !OpModule.NeedsDtor(v))
            {
                v.AddCodes(codes, "mov", null);
                codes.Add(I386.Push(Reg32.EAX));
                TypeReference.AddReferenceCodes(codes);
            }
            else
                v.AddCodes(codes, "push", null);
            for (int i = 1; i < values.Count; i++)
                codes.AddOperatorCodes(f, ad, values[i], false);
            if (op != "push")
            {
                codes.Add(I386.Pop(Reg32.EAX));
                codes.AddCodes(op, dest);
            }
        }

        public override IntValue GetConst()
        {
            var v = GetValue(values[0]);
            if (v == null) return null;

            var ret = v.Value;
            for (int i = 1; i < values.Count; i++)
            {
                var iv = GetValue(values[i]);
                if (iv == null) return null;
                ret = Calculate(ret, iv.Value);
            }
            return new IntValue(ret);
        }

        protected virtual int Calculate(int a, int b) { return a + b; }
    }

    public class Sub : Add
    {
        public override string Tag { get { return "sub"; } }
        public Sub(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Sub(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override int Calculate(int a, int b) { return a - b; }
    }

    public class And : Add
    {
        public override string Tag { get { return "and"; } }
        public And(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public And(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override int Calculate(int a, int b) { return a & b; }
    }

    public class Or : Add
    {
        public override string Tag { get { return "or"; } }
        public Or(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Or(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override int Calculate(int a, int b) { return a | b; }
    }

    public class Xor : Add
    {
        public override string Tag { get { return "xor"; } }
        public Xor(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Xor(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override int Calculate(int a, int b) { return a ^ b; }
    }

    public class ShiftLeft : Add
    {
        public override string Tag { get { return "shift-left"; } }
        public ShiftLeft(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public ShiftLeft(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override int Calculate(int a, int b) { return a << b; }
    }

    public class ShiftRight : Add
    {
        public override string Tag { get { return "shift-right"; } }
        public ShiftRight(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public ShiftRight(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override int Calculate(int a, int b) { return a >> b; }
    }

    public class Mul : Add
    {
        public override string Tag { get { return "mul"; } }
        public Mul(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Mul(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override int Calculate(int a, int b) { return a * b; }
    }

    public class Div : Add
    {
        public override string Tag { get { return "div"; } }
        public Div(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Div(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override int Calculate(int a, int b) { return a / b; }
    }

    public class Mod : Add
    {
        public override string Tag { get { return "mod"; } }
        public Mod(BlockBase parent, params IIntValue[] values) : base(parent, values) { }
        public Mod(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override int Calculate(int a, int b) { return a % b; }
    }
}
