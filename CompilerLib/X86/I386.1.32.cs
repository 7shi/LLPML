using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Push, Pop, Inc, Dec, Not, Neg, Mul, Imul, Div, Idiv

        public static OpCode PushD(Val32 op1)
        {
            return OpCode.NewD(Util.GetBytes1(0x68), op1);
        }
        public static OpCode Push(Reg32 op1) { return FromName1("push", op1); }
        public static OpCode PushA(Addr32 op1) { return FromName1A("push", op1); }

        public static OpCode Pop(Reg32 op1) { return FromName1("pop", op1); }
        public static OpCode PopA(Addr32 op1) { return FromName1A("pop", op1); }

        public static OpCode Inc(Reg32 op1) { return FromName1("inc", op1); }
        public static OpCode IncA(Addr32 op1) { return FromName1A("inc", op1); }

        public static OpCode Dec(Reg32 op1) { return FromName1("dec", op1); }
        public static OpCode DecA(Addr32 op1) { return FromName1A("dec", op1); }

        public static OpCode Not(Reg32 op1) { return FromName1("not", op1); }
        public static OpCode NotA(Addr32 op1) { return FromName1A("not", op1); }

        public static OpCode Neg(Reg32 op1) { return FromName1("neg", op1); }
        public static OpCode NegA(Addr32 op1) { return FromName1A("neg", op1); }

        public static OpCode Mul(Reg32 op1) { return FromName1("mul", op1); }
        public static OpCode MulA(Addr32 op1) { return FromName1A("mul", op1); }

        public static OpCode Imul(Reg32 op1) { return FromName1("imul", op1); }
        public static OpCode ImulA(Addr32 op1) { return FromName1A("imul", op1); }

        public static OpCode Div(Reg32 op1) { return FromName1("div", op1); }
        public static OpCode DivA(Addr32 op1) { return FromName1A("div", op1); }

        public static OpCode Idiv(Reg32 op1) { return FromName1("idiv", op1); }
        public static OpCode IdivA(Addr32 op1) { return FromName1A("idiv", op1); }

        public static bool IsOneOperand(string op)
        {
            return op == "push"
                || op == "pop"
                || op == "inc"
                || op == "dec"
                || op == "not"
                || op == "neg"
                || op == "mul"
                || op == "imul"
                || op == "div"
                || op == "idiv";
        }

        public static OpCode FromName1(string op, Reg32 op1)
        {
            switch (op)
            {
                case "push":
                    return OpCode.NewBytes(Util.GetBytes1((byte)(0x50 + op1)));
                case "pop":
                    return OpCode.NewBytes(Util.GetBytes1((byte)(0x58 + op1)));
                case "inc":
                    return OpCode.NewBytes(Util.GetBytes1((byte)(0x40 + op1)));
                case "dec":
                    return OpCode.NewBytes(Util.GetBytes1((byte)(0x48 + op1)));
                case "not":
                    return OpCode.NewBytes(Util.GetBytes2(0xf7, (byte)(0xd0 + op1)));
                case "neg":
                    return OpCode.NewBytes(Util.GetBytes2(0xf7, (byte)(0xd8 + op1)));
                case "mul":
                    return OpCode.NewBytes(Util.GetBytes2(0xf7, (byte)(0xe0 + op1)));
                case "imul":
                    return OpCode.NewBytes(Util.GetBytes2(0xf7, (byte)(0xe8 + op1)));
                case "div":
                    return OpCode.NewBytes(Util.GetBytes2(0xf7, (byte)(0xf0 + op1)));
                case "idiv":
                    return OpCode.NewBytes(Util.GetBytes2(0xf7, (byte)(0xf8 + op1)));
                default:
                    throw new Exception("invalid operator: " + op);
            }
        }

        public static OpCode FromName1A(string op, Addr32 op1)
        {
            switch (op)
            {
                case "push":
                    return OpCode.NewA(Util.GetBytes1(0xff), Addr32.NewAdM(op1, 6));
                case "pop":
                    return OpCode.NewA(Util.GetBytes1(0x8f), op1);
                case "inc":
                    return OpCode.NewA(Util.GetBytes1(0xff), op1);
                case "dec":
                    return OpCode.NewA(Util.GetBytes1(0xff), Addr32.NewAdM(op1, 1));
                case "not":
                    return OpCode.NewA(Util.GetBytes1(0xf7), Addr32.NewAdM(op1, 2));
                case "neg":
                    return OpCode.NewA(Util.GetBytes1(0xf7), Addr32.NewAdM(op1, 3));
                case "mul":
                    return OpCode.NewA(Util.GetBytes1(0xf7), Addr32.NewAdM(op1, 4));
                case "imul":
                    return OpCode.NewA(Util.GetBytes1(0xf7), Addr32.NewAdM(op1, 5));
                case "div":
                    return OpCode.NewA(Util.GetBytes1(0xf7), Addr32.NewAdM(op1, 6));
                case "idiv":
                    return OpCode.NewA(Util.GetBytes1(0xf7), Addr32.NewAdM(op1, 7));
                default:
                    throw new Exception("invalid operator: " + op);
            }
        }
    }
}
