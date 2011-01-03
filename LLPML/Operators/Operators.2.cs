using System;
using System.Collections.Generic;
using System.Text;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class Operator2 : Operator
    {
        protected abstract int Calculate(int a, int b);

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            if (AddConstCodes(codes, op, dest)) return;

            var ad = Addr32.New(Reg32.ESP);
            var tb = CheckFunc();
            var v = values[0] as NodeBase;
            var schar = "";
            var sint = "";
            if (v.Type is TypeString)
            {
                if (v.Type.CheckFunc(Tag + "-char"))
                    schar = Tag + "-char";
                if (v.Type.CheckFunc(Tag + "-int"))
                    sint = Tag + "-int";
            }
            var tr = v.Type as TypeReference;
            if (tr != null && tr.UseGC && !OpModule.NeedsDtor(v))
            {
                v.AddCodesV(codes, "mov", null);
                codes.Add(I386.Push(Reg32.EAX));
                TypeReference.AddReferenceCodes(codes);
            }
            else
                v.AddCodesV(codes, "push", null);
            for (int i = 1; i < values.Count; i++)
            {
                var vv = values[i] as NodeBase;
                var tag = Tag;
                if (schar != "" && vv.Type is TypeChar)
                    tag = schar;
                else if (sint != "" && vv.Type is TypeIntBase)
                    tag = sint;
                codes.AddOperatorCodes(tb, tag, ad, vv, false);
            }
            if (op != "push")
            {
                codes.Add(I386.Pop(Reg32.EAX));
                codes.AddCodes(op, dest);
            }
        }

        public override IntValue GetConst()
        {
            var v = IntValue.GetValue(values[0] as NodeBase);
            if (v == null) return null;

            var ret = v.Value;
            for (int i = 1; i < values.Count; i++)
            {
                var iv = IntValue.GetValue(values[i] as NodeBase);
                if (iv == null) return null;
                ret = Calculate(ret, iv.Value);
            }
            return IntValue.New(ret);
        }
    }

    public class Add : Operator2
    {
        public override string Tag { get { return "add"; } }
        protected override int Calculate(int a, int b) { return a + b; }

        public static Add New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new Add(), parent, arg1, arg2) as Add;
        }
    }

    public class Sub : Operator2
    {
        public override string Tag { get { return "sub"; } }
        protected override int Calculate(int a, int b) { return a - b; }

        public static Sub New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new Sub(), parent, arg1, arg2) as Sub;
        }
    }

    public class And : Operator2
    {
        public override string Tag { get { return "and"; } }
        protected override int Calculate(int a, int b) { return a & b; }

        public static And New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new And(), parent, arg1, arg2) as And;
        }
    }

    public class Or : Operator2
    {
        public override string Tag { get { return "or"; } }
        protected override int Calculate(int a, int b) { return a | b; }

        public static Or New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new Or(), parent, arg1, arg2) as Or;
        }
    }

    public class Xor : Operator2
    {
        public override string Tag { get { return "xor"; } }
        protected override int Calculate(int a, int b) { return a ^ b; }

        public static Xor New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new Xor(), parent, arg1, arg2) as Xor;
        }
    }

    public class ShiftLeft : Operator2
    {
        public override string Tag { get { return "shift-left"; } }
        protected override int Calculate(int a, int b) { return a << b; }

        public static ShiftLeft New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new ShiftLeft(), parent, arg1, arg2) as ShiftLeft;
        }
    }

    public class ShiftRight : Operator2
    {
        public override string Tag { get { return "shift-right"; } }
        protected override int Calculate(int a, int b) { return a >> b; }

        public static ShiftRight New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new ShiftRight(), parent, arg1, arg2) as ShiftRight;
        }
    }

    public class Mul : Operator2
    {
        public override string Tag { get { return "mul"; } }
        protected override int Calculate(int a, int b) { return a * b; }

        public static Mul New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new Mul(), parent, arg1, arg2) as Mul;
        }
    }

    public class Div : Operator2
    {
        public override string Tag { get { return "div"; } }
        protected override int Calculate(int a, int b) { return a / b; }

        public static Div New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new Div(), parent, arg1, arg2) as Div;
        }
    }

    public class Mod : Operator2
    {
        public override string Tag { get { return "mod"; } }
        protected override int Calculate(int a, int b) { return a % b; }

        public static Mod New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new Mod(), parent, arg1, arg2) as Mod;
        }
    }
}
