using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Shl, Shr, Sal, Sar

        public static OpCode ShlB(Reg8 op1, byte op2) { return ShiftB("shl", op1, op2); }
        public static OpCode ShlBR(Reg8 op1, Reg8 op2) { return ShiftBR("shl", op1, op2); }
        public static OpCode ShlBA(Addr32 op1, byte op2) { return ShiftBA("shl", op1, op2); }
        public static OpCode ShlBAR(Addr32 op1, Reg8 op2) { return ShiftBAR("shl", op1, op2); }

        public static OpCode ShrB(Reg8 op1, byte op2) { return ShiftB("shr", op1, op2); }
        public static OpCode ShrBR(Reg8 op1, Reg8 op2) { return ShiftBR("shr", op1, op2); }
        public static OpCode ShrBA(Addr32 op1, byte op2) { return ShiftBA("shr", op1, op2); }
        public static OpCode ShrBAR(Addr32 op1, Reg8 op2) { return ShiftBAR("shr", op1, op2); }

        public static OpCode SalB(Reg8 op1, byte op2) { return ShiftB("sal", op1, op2); }
        public static OpCode SalBR(Reg8 op1, Reg8 op2) { return ShiftBR("sal", op1, op2); }
        public static OpCode SalBA(Addr32 op1, byte op2) { return ShiftBA("sal", op1, op2); }
        public static OpCode SalBAR(Addr32 op1, Reg8 op2) { return ShiftBAR("sal", op1, op2); }

        public static OpCode SarB(Reg8 op1, byte op2) { return ShiftB("sar", op1, op2); }
        public static OpCode SarBR(Reg8 op1, Reg8 op2) { return ShiftBR("sar", op1, op2); }
        public static OpCode SarBA(Addr32 op1, byte op2) { return ShiftBA("sar", op1, op2); }
        public static OpCode SarBAR(Addr32 op1, Reg8 op2) { return ShiftBAR("sar", op1, op2); }

        public static OpCode ShiftB(string op, Reg8 op1, byte op2)
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
                return OpCode.NewBytes(Util.GetBytes2(0xd0, b));
            else
                return OpCode.NewB(Util.GetBytes2(0xc0, b), op2);
        }

        public static OpCode ShiftBR(string op, Reg8 op1, Reg8 op2)
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
                return OpCode.NewBytes(Util.GetBytes2(0xd2, b));
        }

        public static OpCode ShiftBA(string op, Addr32 op1, byte op2)
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
                return OpCode.NewA(Util.GetBytes1(0xd0), ad);
            else
                return OpCode.NewBA(Util.GetBytes1(0xc0), op2, ad);
        }

        public static OpCode ShiftBAR(string op, Addr32 op1, Reg8 op2)
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
                return OpCode.NewA(Util.GetBytes1(0xd2), ad);
        }
    }
}
