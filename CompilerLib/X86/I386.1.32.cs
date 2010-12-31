using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Push, Pop, Inc, Dec, Not, Neg, Mul, Imul, Div, Idiv

        public static OpCode Push(Val32 op1)
        {
            return OpCode.NewV(Util.GetBytes1(0x68), op1);
        }
        public static OpCode Push(Reg32 op1) { return FromName("push", op1); }
        public static OpCode Push(Addr32 op1) { return FromName("push", op1); }

        public static OpCode Pop(Reg32 op1) { return FromName("pop", op1); }
        public static OpCode Pop(Addr32 op1) { return FromName("pop", op1); }

        public static OpCode Inc(Reg32 op1) { return FromName("inc", op1); }
        public static OpCode Inc(Addr32 op1) { return FromName("inc", op1); }

        public static OpCode Dec(Reg32 op1) { return FromName("dec", op1); }
        public static OpCode Dec(Addr32 op1) { return FromName("dec", op1); }

        public static OpCode Not(Reg32 op1) { return FromName("not", op1); }
        public static OpCode Not(Addr32 op1) { return FromName("not", op1); }

        public static OpCode Neg(Reg32 op1) { return FromName("neg", op1); }
        public static OpCode Neg(Addr32 op1) { return FromName("neg", op1); }

        public static OpCode Mul(Reg32 op1) { return FromName("mul", op1); }
        public static OpCode Mul(Addr32 op1) { return FromName("mul", op1); }

        public static OpCode Imul(Reg32 op1) { return FromName("imul", op1); }
        public static OpCode Imul(Addr32 op1) { return FromName("imul", op1); }

        public static OpCode Div(Reg32 op1) { return FromName("div", op1); }
        public static OpCode Div(Addr32 op1) { return FromName("div", op1); }

        public static OpCode Idiv(Reg32 op1) { return FromName("idiv", op1); }
        public static OpCode Idiv(Addr32 op1) { return FromName("idiv", op1); }

        public static bool IsOneOperand(string op)
        {
            string[] s = { "push", "pop", "inc", "dec", "not", "neg", "mul", "imul", "div", "idiv" };
            return Array.IndexOf(s, op) >= 0;
        }

        public static OpCode FromName(string op, Reg32 op1)
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

        public static OpCode FromName(string op, Addr32 op1)
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
