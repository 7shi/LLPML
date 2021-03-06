﻿using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Shl, Shr, Sal, Sar

        public static OpCode ShlW(Reg16 op1, byte op2) { return ShiftW("shl", op1, op2); }
        public static OpCode ShlWR(Reg16 op1, Reg8 op2) { return ShiftWR("shl", op1, op2); }
        public static OpCode ShlWA(Addr32 op1, byte op2) { return ShiftWA("shl", op1, op2); }
        public static OpCode ShlWAR(Addr32 op1, Reg8 op2) { return ShiftWAR("shl", op1, op2); }

        public static OpCode ShrW(Reg16 op1, byte op2) { return ShiftW("shr", op1, op2); }
        public static OpCode ShrWR(Reg16 op1, Reg8 op2) { return ShiftWR("shr", op1, op2); }
        public static OpCode ShrWA(Addr32 op1, byte op2) { return ShiftWA("shr", op1, op2); }
        public static OpCode ShrWAR(Addr32 op1, Reg8 op2) { return ShiftWAR("shr", op1, op2); }

        public static OpCode SalW(Reg16 op1, byte op2) { return ShiftW("sal", op1, op2); }
        public static OpCode SalWR(Reg16 op1, Reg8 op2) { return ShiftWR("sal", op1, op2); }
        public static OpCode SalWA(Addr32 op1, byte op2) { return ShiftWA("sal", op1, op2); }
        public static OpCode SalWAR(Addr32 op1, Reg8 op2) { return ShiftWAR("sal", op1, op2); }

        public static OpCode SarW(Reg16 op1, byte op2) { return ShiftW("sar", op1, op2); }
        public static OpCode SarWR(Reg16 op1, Reg8 op2) { return ShiftWR("sar", op1, op2); }
        public static OpCode SarWA(Addr32 op1, byte op2) { return ShiftWA("sar", op1, op2); }
        public static OpCode SarWAR(Addr32 op1, Reg8 op2) { return ShiftWAR("sar", op1, op2); }

        public static OpCode ShiftW(string op, Reg16 op1, byte op2)
        {
            byte b;
            switch (op)
            {
                case "shl":
                case "sal":
                    b = (byte)(0xe0 + op1);
                    break;
                case "shr":
                    b = (byte)(0xe8 + op1);
                    break;
                case "sar":
                    b = (byte)(0xf8 + op1);
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            if (op2 == 1)
                return OpCode.NewBytes(Util.GetBytes3(0x66, 0xd1, b));
            else
                return OpCode.NewB(Util.GetBytes3(0x66, 0xc1, b), op2);
        }

        public static OpCode ShiftWR(string op, Reg16 op1, Reg8 op2)
        {
            byte b;
            switch (op)
            {
                case "shl":
                case "sal":
                    b = (byte)(0xe0 + op1);
                    break;
                case "shr":
                    b = (byte)(0xe8 + op1);
                    break;
                case "sar":
                    b = (byte)(0xf8 + op1);
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            if (op2 != Reg8.CL)
                throw new Exception("invalid register: " + op2);
            else
                return OpCode.NewBytes(Util.GetBytes3(0x66, 0xd3, b));
        }

        public static OpCode ShiftWA(string op, Addr32 op1, byte op2)
        {
            Addr32 ad;
            switch (op)
            {
                case "shl":
                case "sal":
                    ad = Addr32.NewAdM(op1, 4);
                    break;
                case "shr":
                    ad = Addr32.NewAdM(op1, 5);
                    break;
                case "sar":
                    ad = Addr32.NewAdM(op1, 7);
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            if (op2 == 1)
                return OpCode.NewA(Util.GetBytes2(0x66, 0xd1), ad);
            else
                return OpCode.NewBA(Util.GetBytes2(0x66, 0xc1), op2, ad);
        }

        public static OpCode ShiftWAR(string op, Addr32 op1, Reg8 op2)
        {
            Addr32 ad;
            switch (op)
            {
                case "shl":
                case "sal":
                    ad = Addr32.NewAdM(op1, 4);
                    break;
                case "shr":
                    ad = Addr32.NewAdM(op1, 5);
                    break;
                case "sar":
                    ad = Addr32.NewAdM(op1, 7);
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            if (op2 != Reg8.CL)
                throw new Exception("invalid register: " + op2);
            else
                return OpCode.NewA(Util.GetBytes2(0x66, 0xd3), ad);
        }
    }
}
