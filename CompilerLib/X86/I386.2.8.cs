using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Mov, Add, Or, Adc, Sbb, And, Sub, Xor, Cmp, Test, Xchg

        public static OpCode MovB(Reg8 op1, Reg8 op2) { return FromName2B("mov", op1, op2); }
        public static OpCode MovBR(Reg8 op1, byte op2) { return FromName2BR("mov", op1, op2); }
        public static OpCode MovBRA(Reg8 op1, Addr32 op2) { return FromName2BRA("mov", op1, op2); }
        public static OpCode MovBAR(Addr32 op1, Reg8 op2) { return FromName2BAR("mov", op1, op2); }
        public static OpCode MovBA(Addr32 op1, byte op2) { return FromName2BA("mov", op1, op2); }

        public static OpCode AddB(Reg8 op1, Reg8 op2) { return FromName2B("add", op1, op2); }
        public static OpCode AddBR(Reg8 op1, byte op2) { return FromName2BR("add", op1, op2); }
        public static OpCode AddBRA(Reg8 op1, Addr32 op2) { return FromName2BRA("add", op1, op2); }
        public static OpCode AddBAR(Addr32 op1, Reg8 op2) { return FromName2BAR("add", op1, op2); }
        public static OpCode AddBA(Addr32 op1, byte op2) { return FromName2BA("add", op1, op2); }

        public static OpCode OrB(Reg8 op1, Reg8 op2) { return FromName2B("or", op1, op2); }
        public static OpCode OrBR(Reg8 op1, byte op2) { return FromName2BR("or", op1, op2); }
        public static OpCode OrBRA(Reg8 op1, Addr32 op2) { return FromName2BRA("or", op1, op2); }
        public static OpCode OrBAR(Addr32 op1, Reg8 op2) { return FromName2BAR("or", op1, op2); }
        public static OpCode OrBA(Addr32 op1, byte op2) { return FromName2BA("or", op1, op2); }

        public static OpCode AdcB(Reg8 op1, Reg8 op2) { return FromName2B("adc", op1, op2); }
        public static OpCode AdcBR(Reg8 op1, byte op2) { return FromName2BR("adc", op1, op2); }
        public static OpCode AdcBRA(Reg8 op1, Addr32 op2) { return FromName2BRA("adc", op1, op2); }
        public static OpCode AdcBAR(Addr32 op1, Reg8 op2) { return FromName2BAR("adc", op1, op2); }
        public static OpCode AdcBA(Addr32 op1, byte op2) { return FromName2BA("adc", op1, op2); }

        public static OpCode SbbB(Reg8 op1, Reg8 op2) { return FromName2B("sbb", op1, op2); }
        public static OpCode SbbBR(Reg8 op1, byte op2) { return FromName2BR("sbb", op1, op2); }
        public static OpCode SbbBRA(Reg8 op1, Addr32 op2) { return FromName2BRA("sbb", op1, op2); }
        public static OpCode SbbBAR(Addr32 op1, Reg8 op2) { return FromName2BAR("sbb", op1, op2); }
        public static OpCode SbbBA(Addr32 op1, byte op2) { return FromName2BA("sbb", op1, op2); }

        public static OpCode AndB(Reg8 op1, Reg8 op2) { return FromName2B("and", op1, op2); }
        public static OpCode AndBR(Reg8 op1, byte op2) { return FromName2BR("and", op1, op2); }
        public static OpCode AndBRA(Reg8 op1, Addr32 op2) { return FromName2BRA("and", op1, op2); }
        public static OpCode AndBAR(Addr32 op1, Reg8 op2) { return FromName2BAR("and", op1, op2); }
        public static OpCode AndBA(Addr32 op1, byte op2) { return FromName2BA("and", op1, op2); }

        public static OpCode SubB(Reg8 op1, Reg8 op2) { return FromName2B("sub", op1, op2); }
        public static OpCode SubBR(Reg8 op1, byte op2) { return FromName2BR("sub", op1, op2); }
        public static OpCode SubBRA(Reg8 op1, Addr32 op2) { return FromName2BRA("sub", op1, op2); }
        public static OpCode SubBAR(Addr32 op1, Reg8 op2) { return FromName2BAR("sub", op1, op2); }
        public static OpCode SubBA(Addr32 op1, byte op2) { return FromName2BA("sub", op1, op2); }

        public static OpCode XorB(Reg8 op1, Reg8 op2) { return FromName2B("xor", op1, op2); }
        public static OpCode XorBR(Reg8 op1, byte op2) { return FromName2BR("xor", op1, op2); }
        public static OpCode XorBRA(Reg8 op1, Addr32 op2) { return FromName2BRA("xor", op1, op2); }
        public static OpCode XorBAR(Addr32 op1, Reg8 op2) { return FromName2BAR("xor", op1, op2); }
        public static OpCode XorBA(Addr32 op1, byte op2) { return FromName2BA("xor", op1, op2); }

        public static OpCode CmpB(Reg8 op1, Reg8 op2) { return FromName2B("cmp", op1, op2); }
        public static OpCode CmpBR(Reg8 op1, byte op2) { return FromName2BR("cmp", op1, op2); }
        public static OpCode CmpBRA(Reg8 op1, Addr32 op2) { return FromName2BRA("cmp", op1, op2); }
        public static OpCode CmpBAR(Addr32 op1, Reg8 op2) { return FromName2BAR("cmp", op1, op2); }
        public static OpCode CmpBA(Addr32 op1, byte op2) { return FromName2BA("cmp", op1, op2); }

        public static OpCode TestB(Reg8 op1, Reg8 op2) { return FromName2B("test", op1, op2); }
        public static OpCode TestBR(Reg8 op1, byte op2) { return FromName2BR("test", op1, op2); }
        public static OpCode TestBRA(Reg8 op1, Addr32 op2) { return TestBAR(op2, op1); }
        public static OpCode TestBAR(Addr32 op1, Reg8 op2) { return FromName2BAR("test", op1, op2); }
        public static OpCode TestBA(Addr32 op1, byte op2) { return FromName2BA("test", op1, op2); }

        public static OpCode XchgB(Reg8 op1, Reg8 op2) { return FromName2B("xchg", op1, op2); }
        public static OpCode XchgBRA(Reg8 op1, Addr32 op2) { return FromName2BRA("xchg", op1, op2); }
        public static OpCode XchgBAR(Addr32 op1, Reg8 op2) { return XchgBRA(op2, op1); }

        public static OpCode FromName2B(string op, Reg8 op1, Reg8 op2)
        {
            byte b;
            switch (op)
            {
                case "mov":
                    b = 0x88;
                    break;
                case "test":
                    b = 0x84;
                    break;
                case "xchg":
                    b = 0x86;
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    b = (byte)(code * 8);
                    break;
            }
            return OpCode.NewBytes(Util.GetBytes2(b, (byte)(0xc0 + (((int)op2) << 3) + op1)));
        }

        public static OpCode FromName2BR(string op, Reg8 op1, byte op2)
        {
            byte[] bytes;
            switch (op)
            {
                case "mov":
                    bytes = Util.GetBytes1((byte)(0xb0 + op1));
                    break;
                case "test":
                    if (op1 == Reg8.AL)
                        bytes = Util.GetBytes1(0xa8);
                    else
                        bytes = Util.GetBytes2(0xf6, (byte)(0xc0 + op1));
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    if (op1 == Reg8.AL)
                        bytes = Util.GetBytes1((byte)(code * 8 + 4));
                    else
                        bytes = Util.GetBytes2(0x80, (byte)(code * 8 + 0xc0 + op1));
                    break;
            }
            return OpCode.NewB(bytes, op2);
        }

        public static OpCode FromName2BRA(string op, Reg8 op1, Addr32 op2)
        {
            byte b;
            switch (op)
            {
                case "mov":
                    if (op1 == Reg8.AL && op2.IsAddress)
                        return OpCode.NewD(Util.GetBytes1(0xa0), op2.Address);
                    b = 0x8a;
                    break;
                case "xchg":
                    b = 0x86;
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    b = (byte)(code * 8 + 2);
                    break;
            }
            return OpCode.NewA(Util.GetBytes1(b), Addr32.NewAdM(op2, (byte)op1));
        }

        public static OpCode FromName2BAR(string op, Addr32 op1, Reg8 op2)
        {
            byte b;
            switch (op)
            {
                case "mov":
                    if (op2 == Reg8.AL && op1.IsAddress)
                        return OpCode.NewD(Util.GetBytes1(0xa2), op1.Address);
                    b = 0x88;
                    break;
                case "test":
                    b = 0x84;
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    b = (byte)(code * 8);
                    break;
            }
            return OpCode.NewA(Util.GetBytes1(b), Addr32.NewAdM(op1, (byte)op2));
        }

        public static OpCode FromName2BA(string op, Addr32 op1, byte op2)
        {
            switch (op)
            {
                case "mov":
                    return OpCode.NewBA(Util.GetBytes1(0xc6), op2, op1);
                case "test":
                    return OpCode.NewBA(Util.GetBytes1(0xf6), op2, op1);
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    return OpCode.NewBA(Util.GetBytes1(0x80), op2, Addr32.NewAdM(op1, (byte)code));
            }
        }
    }
}
