using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Shl, Shr, Sal, Sar

        public static OpCode Shl(Reg32 op1, byte op2) { return Shift("shl", op1, op2); }
        public static OpCode ShlR(Reg32 op1, Reg8 op2) { return ShiftR("shl", op1, op2); }
        public static OpCode ShlA(Addr32 op1, byte op2) { return ShiftA("shl", op1, op2); }
        public static OpCode ShlAR(Addr32 op1, Reg8 op2) { return ShiftAR("shl", op1, op2); }

        public static OpCode Shr(Reg32 op1, byte op2) { return Shift("shr", op1, op2); }
        public static OpCode ShrR(Reg32 op1, Reg8 op2) { return ShiftR("shr", op1, op2); }
        public static OpCode ShrA(Addr32 op1, byte op2) { return ShiftA("shr", op1, op2); }
        public static OpCode ShrAR(Addr32 op1, Reg8 op2) { return ShiftAR("shr", op1, op2); }

        public static OpCode Sal(Reg32 op1, byte op2) { return Shift("sal", op1, op2); }
        public static OpCode SalR(Reg32 op1, Reg8 op2) { return ShiftR("sal", op1, op2); }
        public static OpCode SalA(Addr32 op1, byte op2) { return ShiftA("sal", op1, op2); }
        public static OpCode SalAR(Addr32 op1, Reg8 op2) { return ShiftAR("sal", op1, op2); }

        public static OpCode Sar(Reg32 op1, byte op2) { return Shift("sar", op1, op2); }
        public static OpCode SarR(Reg32 op1, Reg8 op2) { return ShiftR("sar", op1, op2); }
        public static OpCode SarA(Addr32 op1, byte op2) { return ShiftA("sar", op1, op2); }
        public static OpCode SarAR(Addr32 op1, Reg8 op2) { return ShiftAR("sar", op1, op2); }

        public static OpCode Shift(string op, Reg32 op1, byte op2)
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
                return OpCode.NewBytes(Util.GetBytes2(0xd1, b));
            else
                return OpCode.NewB(Util.GetBytes2(0xc1, b), op2);
        }

        public static OpCode ShiftR(string op, Reg32 op1, Reg8 op2)
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
                return OpCode.NewBytes(Util.GetBytes2(0xd3, b));
        }

        public static OpCode ShiftA(string op, Addr32 op1, byte op2)
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
                return OpCode.NewA(Util.GetBytes1(0xd1), ad);
            else
                return OpCode.NewBA(Util.GetBytes1(0xc1), op2, ad);
        }

        public static OpCode ShiftAR(string op, Addr32 op1, Reg8 op2)
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
                return OpCode.NewA(Util.GetBytes1(0xd3), ad);
        }
    }
}
