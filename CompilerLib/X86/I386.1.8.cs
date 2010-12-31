﻿using System;
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
            return OpCode.New(Util.GetBytes1(0x6a), op1);
        }

        public static OpCode IncB(Reg8 op1) { return FromNameB("inc", op1); }
        public static OpCode IncB(Addr32 op1) { return FromNameB("inc", op1); }

        public static OpCode DecB(Reg8 op1) { return FromNameB("dec", op1); }
        public static OpCode DecB(Addr32 op1) { return FromNameB("dec", op1); }

        public static OpCode NotB(Reg8 op1) { return FromNameB("not", op1); }
        public static OpCode NotB(Addr32 op1) { return FromNameB("not", op1); }

        public static OpCode NegB(Reg8 op1) { return FromNameB("neg", op1); }
        public static OpCode NegB(Addr32 op1) { return FromNameB("neg", op1); }

        public static OpCode MulB(Reg8 op1) { return FromNameB("mul", op1); }
        public static OpCode MulB(Addr32 op1) { return FromNameB("mul", op1); }

        public static OpCode ImulB(Reg8 op1) { return FromNameB("imul", op1); }
        public static OpCode ImulB(Addr32 op1) { return FromNameB("imul", op1); }

        public static OpCode DivB(Reg8 op1) { return FromNameB("div", op1); }
        public static OpCode DivB(Addr32 op1) { return FromNameB("div", op1); }

        public static OpCode IdivB(Reg8 op1) { return FromNameB("idiv", op1); }
        public static OpCode IdivB(Addr32 op1) { return FromNameB("idiv", op1); }

        public static OpCode FromNameB(string op, Reg8 op1)
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

        public static OpCode FromNameB(string op, Addr32 op1)
        {
            switch (op)
            {
                case "inc":
                    return OpCode.NewA(Util.GetBytes1(0xfe), null, op1);
                case "dec":
                    return OpCode.NewA(Util.GetBytes1(0xfe), null, Addr32.NewAdM(op1, 1));
                case "not":
                    return OpCode.NewA(Util.GetBytes1(0xf6), null, Addr32.NewAdM(op1, 2));
                case "neg":
                    return OpCode.NewA(Util.GetBytes1(0xf6), null, Addr32.NewAdM(op1, 3));
                case "mul":
                    return OpCode.NewA(Util.GetBytes1(0xf6), null, Addr32.NewAdM(op1, 4));
                case "imul":
                    return OpCode.NewA(Util.GetBytes1(0xf6), null, Addr32.NewAdM(op1, 5));
                case "div":
                    return OpCode.NewA(Util.GetBytes1(0xf6), null, Addr32.NewAdM(op1, 6));
                case "idiv":
                    return OpCode.NewA(Util.GetBytes1(0xf6), null, Addr32.NewAdM(op1, 7));
                default:
                    throw new Exception("invalid operator: " + op);
            }
        }
    }
}
