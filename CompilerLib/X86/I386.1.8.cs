using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Push, Pop, Inc, Dec, Not, Neg, Mul, Imul, Div, Idiv

        public static OpCode PushB(byte op1)
        {
            return OpCode.NewB(Util.GetBytes1(0x6a), op1);
        }

        public static OpCode IncB(Reg8 op1) { return FromName1B("inc", op1); }
        public static OpCode IncBA(Addr32 op1) { return FromName1BA("inc", op1); }

        public static OpCode DecB(Reg8 op1) { return FromName1B("dec", op1); }
        public static OpCode DecBA(Addr32 op1) { return FromName1BA("dec", op1); }

        public static OpCode NotB(Reg8 op1) { return FromName1B("not", op1); }
        public static OpCode NotBA(Addr32 op1) { return FromName1BA("not", op1); }

        public static OpCode NegB(Reg8 op1) { return FromName1B("neg", op1); }
        public static OpCode NegBA(Addr32 op1) { return FromName1BA("neg", op1); }

        public static OpCode MulB(Reg8 op1) { return FromName1B("mul", op1); }
        public static OpCode MulBA(Addr32 op1) { return FromName1BA("mul", op1); }

        public static OpCode ImulB(Reg8 op1) { return FromName1B("imul", op1); }
        public static OpCode ImulBA(Addr32 op1) { return FromName1BA("imul", op1); }

        public static OpCode DivB(Reg8 op1) { return FromName1B("div", op1); }
        public static OpCode DivBA(Addr32 op1) { return FromName1BA("div", op1); }

        public static OpCode IdivB(Reg8 op1) { return FromName1B("idiv", op1); }
        public static OpCode IdivBA(Addr32 op1) { return FromName1BA("idiv", op1); }

        public static OpCode FromName1B(string op, Reg8 op1)
        {
            switch (op)
            {
                case "inc":
                    return OpCode.NewBytes(Util.GetBytes2(0xfe, (byte)(0xc0 + op1)));
                case "dec":
                    return OpCode.NewBytes(Util.GetBytes2(0xfe, (byte)(0xc8 + op1)));
                case "not":
                    return OpCode.NewBytes(Util.GetBytes2(0xf6, (byte)(0xd0 + op1)));
                case "neg":
                    return OpCode.NewBytes(Util.GetBytes2(0xf6, (byte)(0xd8 + op1)));
                case "mul":
                    return OpCode.NewBytes(Util.GetBytes2(0xf6, (byte)(0xe0 + op1)));
                case "imul":
                    return OpCode.NewBytes(Util.GetBytes2(0xf6, (byte)(0xe8 + op1)));
                case "div":
                    return OpCode.NewBytes(Util.GetBytes2(0xf6, (byte)(0xf0 + op1)));
                case "idiv":
                    return OpCode.NewBytes(Util.GetBytes2(0xf6, (byte)(0xf8 + op1)));
                default:
                    throw new Exception("invalid operator: " + op);
            }
        }

        public static OpCode FromName1BA(string op, Addr32 op1)
        {
            switch (op)
            {
                case "inc":
                    return OpCode.NewA(Util.GetBytes1(0xfe), op1);
                case "dec":
                    return OpCode.NewA(Util.GetBytes1(0xfe), Addr32.NewAdM(op1, 1));
                case "not":
                    return OpCode.NewA(Util.GetBytes1(0xf6), Addr32.NewAdM(op1, 2));
                case "neg":
                    return OpCode.NewA(Util.GetBytes1(0xf6), Addr32.NewAdM(op1, 3));
                case "mul":
                    return OpCode.NewA(Util.GetBytes1(0xf6), Addr32.NewAdM(op1, 4));
                case "imul":
                    return OpCode.NewA(Util.GetBytes1(0xf6), Addr32.NewAdM(op1, 5));
                case "div":
                    return OpCode.NewA(Util.GetBytes1(0xf6), Addr32.NewAdM(op1, 6));
                case "idiv":
                    return OpCode.NewA(Util.GetBytes1(0xf6), Addr32.NewAdM(op1, 7));
                default:
                    throw new Exception("invalid operator: " + op);
            }
        }
    }
}
