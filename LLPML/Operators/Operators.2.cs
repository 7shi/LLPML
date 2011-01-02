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
        public Add(BlockBase parent, params NodeBase[] values) : base(parent, values) { }

        public override void AddCodesValue(OpModule codes, string op, Addr32 dest)
        {
            if (AddConstCodes(codes, op, dest)) return;

            var ad = Addr32.New(Reg32.ESP);
            var f = GetFunc();
            var v = values[0];
            TypeBase.Func schar = null, sint = null;
            if (v.Type is TypeString)
            {
                schar = v.Type.GetFunc(Tag + "-char");
                sint = v.Type.GetFunc(Tag + "-int");
            }
            var tr = v.Type as TypeReference;
            if (tr != null && tr.UseGC && !OpModule.NeedsDtor(v))
            {
                v.AddCodesValue(codes, "mov", null);
                codes.Add(I386.Push(Reg32.EAX));
                TypeReference.AddReferenceCodes(codes);
            }
            else
                v.AddCodesValue(codes, "push", null);
            for (int i = 1; i < values.Count; i++)
            {
                var vv = values[i];
                var ff = f;
                if (schar != null && vv.Type is TypeChar)
                    ff = schar;
                else if (sint != null && vv.Type is TypeIntBase)
                    ff = sint;
                codes.AddOperatorCodes(ff, ad, vv, false);
            }
            if (op != "push")
            {
                codes.Add(I386.Pop(Reg32.EAX));
                codes.AddCodes(op, dest);
            }
        }

        public override IntValue GetConst()
        {
            var v = IntValue.GetValue(values[0]);
            if (v == null) return null;

            var ret = v.Value;
            for (int i = 1; i < values.Count; i++)
            {
                var iv = IntValue.GetValue(values[i]);
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
        public Sub(BlockBase parent, params NodeBase[] values) : base(parent, values) { }
        protected override int Calculate(int a, int b) { return a - b; }
    }

    public class And : Add
    {
        public override string Tag { get { return "and"; } }
        public And(BlockBase parent, params NodeBase[] values) : base(parent, values) { }
        protected override int Calculate(int a, int b) { return a & b; }
    }

    public class Or : Add
    {
        public override string Tag { get { return "or"; } }
        public Or(BlockBase parent, params NodeBase[] values) : base(parent, values) { }
        protected override int Calculate(int a, int b) { return a | b; }
    }

    public class Xor : Add
    {
        public override string Tag { get { return "xor"; } }
        public Xor(BlockBase parent, params NodeBase[] values) : base(parent, values) { }
        protected override int Calculate(int a, int b) { return a ^ b; }
    }

    public class ShiftLeft : Add
    {
        public override string Tag { get { return "shift-left"; } }
        public ShiftLeft(BlockBase parent, params NodeBase[] values) : base(parent, values) { }
        protected override int Calculate(int a, int b) { return a << b; }
    }

    public class ShiftRight : Add
    {
        public override string Tag { get { return "shift-right"; } }
        public ShiftRight(BlockBase parent, params NodeBase[] values) : base(parent, values) { }
        protected override int Calculate(int a, int b) { return a >> b; }
    }

    public class Mul : Add
    {
        public override string Tag { get { return "mul"; } }
        public Mul(BlockBase parent, params NodeBase[] values) : base(parent, values) { }
        protected override int Calculate(int a, int b) { return a * b; }
    }

    public class Div : Add
    {
        public override string Tag { get { return "div"; } }
        public Div(BlockBase parent, params NodeBase[] values) : base(parent, values) { }
        protected override int Calculate(int a, int b) { return a / b; }
    }

    public class Mod : Add
    {
        public override string Tag { get { return "mod"; } }
        public Mod(BlockBase parent, params NodeBase[] values) : base(parent, values) { }
        protected override int Calculate(int a, int b) { return a % b; }
    }
}
