using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Mov, Add, Or, Adc, Sbb, And, Sub, Xor, Cmp, Test, Xchg

        public static OpCode MovW(Reg16 op1, Reg16 op2) { return FromName2W("mov", op1, op2); }
        public static OpCode MovWR(Reg16 op1, ushort op2) { return FromName2WR("mov", op1, op2); }
        public static OpCode MovWRA(Reg16 op1, Addr32 op2) { return FromName2WRA("mov", op1, op2); }
        public static OpCode MovWAR(Addr32 op1, Reg16 op2) { return FromName2WAR("mov", op1, op2); }
        public static OpCode MovWA(Addr32 op1, ushort op2) { return FromName2WA("mov", op1, op2); }

        public static OpCode AddW(Reg16 op1, Reg16 op2) { return FromName2W("add", op1, op2); }
        public static OpCode AddWR(Reg16 op1, ushort op2) { return FromName2WR("add", op1, op2); }
        public static OpCode AddWRA(Reg16 op1, Addr32 op2) { return FromName2WRA("add", op1, op2); }
        public static OpCode AddWAR(Addr32 op1, Reg16 op2) { return FromName2WAR("add", op1, op2); }
        public static OpCode AddWA(Addr32 op1, ushort op2) { return FromName2WA("add", op1, op2); }

        public static OpCode OrW(Reg16 op1, Reg16 op2) { return FromName2W("or", op1, op2); }
        public static OpCode OrWR(Reg16 op1, ushort op2) { return FromName2WR("or", op1, op2); }
        public static OpCode OrWRA(Reg16 op1, Addr32 op2) { return FromName2WRA("or", op1, op2); }
        public static OpCode OrWAR(Addr32 op1, Reg16 op2) { return FromName2WAR("or", op1, op2); }
        public static OpCode OrWA(Addr32 op1, ushort op2) { return FromName2WA("or", op1, op2); }

        public static OpCode AdcW(Reg16 op1, Reg16 op2) { return FromName2W("adc", op1, op2); }
        public static OpCode AdcWR(Reg16 op1, ushort op2) { return FromName2WR("adc", op1, op2); }
        public static OpCode AdcWRA(Reg16 op1, Addr32 op2) { return FromName2WRA("adc", op1, op2); }
        public static OpCode AdcWAR(Addr32 op1, Reg16 op2) { return FromName2WAR("adc", op1, op2); }
        public static OpCode AdcWA(Addr32 op1, ushort op2) { return FromName2WA("adc", op1, op2); }

        public static OpCode SbbW(Reg16 op1, Reg16 op2) { return FromName2W("sbb", op1, op2); }
        public static OpCode SbbWR(Reg16 op1, ushort op2) { return FromName2WR("sbb", op1, op2); }
        public static OpCode SbbWRA(Reg16 op1, Addr32 op2) { return FromName2WRA("sbb", op1, op2); }
        public static OpCode SbbWAR(Addr32 op1, Reg16 op2) { return FromName2WAR("sbb", op1, op2); }
        public static OpCode SbbWA(Addr32 op1, ushort op2) { return FromName2WA("sbb", op1, op2); }

        public static OpCode AndW(Reg16 op1, Reg16 op2) { return FromName2W("and", op1, op2); }
        public static OpCode AndWR(Reg16 op1, ushort op2) { return FromName2WR("and", op1, op2); }
        public static OpCode AndWRA(Reg16 op1, Addr32 op2) { return FromName2WRA("and", op1, op2); }
        public static OpCode AndWAR(Addr32 op1, Reg16 op2) { return FromName2WAR("and", op1, op2); }
        public static OpCode AndWA(Addr32 op1, ushort op2) { return FromName2WA("and", op1, op2); }

        public static OpCode SubW(Reg16 op1, Reg16 op2) { return FromName2W("sub", op1, op2); }
        public static OpCode SubWR(Reg16 op1, ushort op2) { return FromName2WR("sub", op1, op2); }
        public static OpCode SubWRA(Reg16 op1, Addr32 op2) { return FromName2WRA("sub", op1, op2); }
        public static OpCode SubWAR(Addr32 op1, Reg16 op2) { return FromName2WAR("sub", op1, op2); }
        public static OpCode SubWA(Addr32 op1, ushort op2) { return FromName2WA("sub", op1, op2); }

        public static OpCode XorW(Reg16 op1, Reg16 op2) { return FromName2W("xor", op1, op2); }
        public static OpCode XorWR(Reg16 op1, ushort op2) { return FromName2WR("xor", op1, op2); }
        public static OpCode XorWRA(Reg16 op1, Addr32 op2) { return FromName2WRA("xor", op1, op2); }
        public static OpCode XorWAR(Addr32 op1, Reg16 op2) { return FromName2WAR("xor", op1, op2); }
        public static OpCode XorWA(Addr32 op1, ushort op2) { return FromName2WA("xor", op1, op2); }

        public static OpCode CmpW(Reg16 op1, Reg16 op2) { return FromName2W("cmp", op1, op2); }
        public static OpCode CmpWR(Reg16 op1, ushort op2) { return FromName2WR("cmp", op1, op2); }
        public static OpCode CmpWRA(Reg16 op1, Addr32 op2) { return FromName2WRA("cmp", op1, op2); }
        public static OpCode CmpWAR(Addr32 op1, Reg16 op2) { return FromName2WAR("cmp", op1, op2); }
        public static OpCode CmpWA(Addr32 op1, ushort op2) { return FromName2WA("cmp", op1, op2); }

        public static OpCode TestW(Reg16 op1, Reg16 op2) { return FromName2W("test", op1, op2); }
        public static OpCode TestWR(Reg16 op1, ushort op2) { return FromName2WR("test", op1, op2); }
        public static OpCode TestWRA(Reg16 op1, Addr32 op2) { return TestWAR(op2, op1); }
        public static OpCode TestWAR(Addr32 op1, Reg16 op2) { return FromName2WAR("test", op1, op2); }
        public static OpCode TestWA(Addr32 op1, ushort op2) { return FromName2WA("test", op1, op2); }

        public static OpCode XchgW(Reg16 op1, Reg16 op2) { return FromName2W("xchg", op1, op2); }
        public static OpCode XchgWRA(Reg16 op1, Addr32 op2) { return FromName2WRA("xchg", op1, op2); }
        public static OpCode XchgWAR(Addr32 op1, Reg16 op2) { return XchgWRA(op2, op1); }

        public static OpCode FromName2W(string op, Reg16 op1, Reg16 op2)
        {
            byte b;
            switch (op)
            {
                case "mov":
                    b = 0x89;
                    break;
                case "test":
                    b = 0x85;
                    break;
                case "xchg":
                    if (op1 == Reg16.AX)
                        return OpCode.NewBytes(Util.GetBytes2(0x66, (byte)(0x90 + op2)));
                    else if (op2 == Reg16.AX)
                        return OpCode.NewBytes(Util.GetBytes2(0x66, (byte)(0x90 + op1)));
                    else
                        b = 0x87;
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    b = (byte)(code * 8 + 1);
                    break;
            }
            return OpCode.NewBytes(Util.GetBytes3(0x66, b, (byte)(0xc0 + (((int)op2) << 3) + op1)));
        }

        public static OpCode FromName2WR(string op, Reg16 op1, ushort op2)
        {
            byte[] bytes;
            switch (op)
            {
                case "mov":
                    bytes = Util.GetBytes2(0x66, (byte)(0xb8 + op1));
                    break;
                case "test":
                    if (op1 == Reg16.AX)
                        bytes = Util.GetBytes2(0x66, 0xa9);
                    else
                        bytes = Util.GetBytes3(0x66, 0xf7, (byte)(0xc0 + op1));
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    if (op1 == Reg16.AX)
                        bytes = Util.GetBytes2(0x66, (byte)(code * 8 + 5));
                    else
                        bytes = Util.GetBytes3(0x66, 0x81, (byte)(code * 8 + 0xc0 + op1));
                    break;
            }
            return OpCode.NewW(bytes, op2);
        }

        public static OpCode FromName2WRA(string op, Reg16 op1, Addr32 op2)
        {
            byte b;
            switch (op)
            {
                case "mov":
                    if (op1 == Reg16.AX && op2.IsAddress)
                        return OpCode.NewD(Util.GetBytes2(0x66, 0xa1), op2.Address);
                    b = 0x8b;
                    break;
                case "xchg":
                    b = 0x87;
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    b = (byte)(code * 8 + 3);
                    break;
            }
            return OpCode.NewA(Util.GetBytes2(0x66, b), Addr32.NewAdM(op2, (byte)op1));
        }

        public static OpCode FromName2WAR(string op, Addr32 op1, Reg16 op2)
        {
            byte b;
            switch (op)
            {
                case "mov":
                    if (op2 == Reg16.AX && op1.IsAddress)
                        return OpCode.NewD(Util.GetBytes2(0x66, 0xa3), op1.Address);
                    b = 0x89;
                    break;
                case "test":
                    b = 0x85;
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    b = (byte)(code * 8 + 1);
                    break;
            }
            return OpCode.NewA(Util.GetBytes2(0x66, b), Addr32.NewAdM(op1, (byte)op2));
        }

        public static OpCode FromName2WA(string op, Addr32 op1, ushort op2)
        {
            switch (op)
            {
                case "mov":
                    return OpCode.NewWA(Util.GetBytes2(0x66, 0xc7), op2, op1);
                case "test":
                    return OpCode.NewWA(Util.GetBytes2(0x66, 0xf7), op2, op1);
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    return OpCode.NewWA(Util.GetBytes2(0x66, 0x81), op2, Addr32.NewAdM(op1, (byte)code));
            }
        }
    }
}
