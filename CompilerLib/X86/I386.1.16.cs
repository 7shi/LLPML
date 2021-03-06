﻿using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Push, Pop, Inc, Dec, Not, Neg, Mul, Imul, Div, Idiv

        public static OpCode PushWU(ushort op1)
        {
            return OpCode.NewW(Util.GetBytes2(0x66, 0x68), op1);
        }
        public static OpCode PushW(Reg16 op1) { return FromName1W("push", op1); }
        public static OpCode PushWA(Addr32 op1) { return FromName1WA("push", op1); }

        public static OpCode PopW(Reg16 op1) { return FromName1W("pop", op1); }
        public static OpCode PopWA(Addr32 op1) { return FromName1WA("pop", op1); }

        public static OpCode IncW(Reg16 op1) { return FromName1W("inc", op1); }
        public static OpCode IncWA(Addr32 op1) { return FromName1WA("inc", op1); }

        public static OpCode DecW(Reg16 op1) { return FromName1W("dec", op1); }
        public static OpCode DecWA(Addr32 op1) { return FromName1WA("dec", op1); }

        public static OpCode NotW(Reg16 op1) { return FromName1W("not", op1); }
        public static OpCode NotWA(Addr32 op1) { return FromName1WA("not", op1); }

        public static OpCode NegW(Reg16 op1) { return FromName1W("neg", op1); }
        public static OpCode NegWA(Addr32 op1) { return FromName1WA("neg", op1); }

        public static OpCode MulW(Reg16 op1) { return FromName1W("mul", op1); }
        public static OpCode MulWA(Addr32 op1) { return FromName1WA("mul", op1); }

        public static OpCode ImulW(Reg16 op1) { return FromName1W("imul", op1); }
        public static OpCode ImulWA(Addr32 op1) { return FromName1WA("imul", op1); }

        public static OpCode DivW(Reg16 op1) { return FromName1W("div", op1); }
        public static OpCode DivWA(Addr32 op1) { return FromName1WA("div", op1); }

        public static OpCode IdivW(Reg16 op1) { return FromName1W("idiv", op1); }
        public static OpCode IdivWA(Addr32 op1) { return FromName1WA("idiv", op1); }

        public static OpCode FromName1W(string op, Reg16 op1)
        {
            switch (op)
            {
                case "push":
                    return OpCode.NewBytes(Util.GetBytes2(0x66, (byte)(0x50 + op1)));
                case "pop":
                    return OpCode.NewBytes(Util.GetBytes2(0x66, (byte)(0x58 + op1)));
                case "inc":
                    return OpCode.NewBytes(Util.GetBytes2(0x66, (byte)(0x40 + op1)));
                case "dec":
                    return OpCode.NewBytes(Util.GetBytes2(0x66, (byte)(0x48 + op1)));
                case "not":
                    return OpCode.NewBytes(Util.GetBytes3(0x66, 0xf7, (byte)(0xd0 + op1)));
                case "neg":
                    return OpCode.NewBytes(Util.GetBytes3(0x66, 0xf7, (byte)(0xd8 + op1)));
                case "mul":
                    return OpCode.NewBytes(Util.GetBytes3(0x66, 0xf7, (byte)(0xe0 + op1)));
                case "imul":
                    return OpCode.NewBytes(Util.GetBytes3(0x66, 0xf7, (byte)(0xe8 + op1)));
                case "div":
                    return OpCode.NewBytes(Util.GetBytes3(0x66, 0xf7, (byte)(0xf0 + op1)));
                case "idiv":
                    return OpCode.NewBytes(Util.GetBytes3(0x66, 0xf7, (byte)(0xf8 + op1)));
                default:
                    throw new Exception("invalid operator: " + op);
            }
        }

        public static OpCode FromName1WA(string op, Addr32 op1)
        {
            switch (op)
            {
                case "push":
                    return OpCode.NewA(Util.GetBytes2(0x66, 0xff), Addr32.NewAdM(op1, 6));
                case "pop":
                    return OpCode.NewA(Util.GetBytes2(0x66, 0x8f), op1);
                case "inc":
                    return OpCode.NewA(Util.GetBytes2(0x66, 0xff), op1);
                case "dec":
                    return OpCode.NewA(Util.GetBytes2(0x66, 0xff), Addr32.NewAdM(op1, 1));
                case "not":
                    return OpCode.NewA(Util.GetBytes2(0x66, 0xf7), Addr32.NewAdM(op1, 2));
                case "neg":
                    return OpCode.NewA(Util.GetBytes2(0x66, 0xf7), Addr32.NewAdM(op1, 3));
                case "mul":
                    return OpCode.NewA(Util.GetBytes2(0x66, 0xf7), Addr32.NewAdM(op1, 4));
                case "imul":
                    return OpCode.NewA(Util.GetBytes2(0x66, 0xf7), Addr32.NewAdM(op1, 5));
                case "div":
                    return OpCode.NewA(Util.GetBytes2(0x66, 0xf7), Addr32.NewAdM(op1, 6));
                case "idiv":
                    return OpCode.NewA(Util.GetBytes2(0x66, 0xf7), Addr32.NewAdM(op1, 7));
                default:
                    throw new Exception("invalid operator: " + op);
            }
        }
    }
}
