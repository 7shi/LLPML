﻿using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Mov, Add, Or, Adc, Sbb, And, Sub, Xor, Cmp, Test, Xchg

        public static OpCode MovW(Reg16 op1, Reg16 op2) { return FromNameW("mov", op1, op2); }
        public static OpCode MovW(Reg16 op1, ushort op2) { return FromNameW("mov", op1, op2); }
        public static OpCode MovW(Reg16 op1, Addr32 op2) { return FromNameW("mov", op1, op2); }
        public static OpCode MovW(Addr32 op1, Reg16 op2) { return FromNameW("mov", op1, op2); }
        public static OpCode MovW(Addr32 op1, ushort op2) { return FromNameW("mov", op1, op2); }

        public static OpCode AddW(Reg16 op1, Reg16 op2) { return FromNameW("add", op1, op2); }
        public static OpCode AddW(Reg16 op1, ushort op2) { return FromNameW("add", op1, op2); }
        public static OpCode AddW(Reg16 op1, Addr32 op2) { return FromNameW("add", op1, op2); }
        public static OpCode AddW(Addr32 op1, Reg16 op2) { return FromNameW("add", op1, op2); }
        public static OpCode AddW(Addr32 op1, ushort op2) { return FromNameW("add", op1, op2); }

        public static OpCode OrW(Reg16 op1, Reg16 op2) { return FromNameW("or", op1, op2); }
        public static OpCode OrW(Reg16 op1, ushort op2) { return FromNameW("or", op1, op2); }
        public static OpCode OrW(Reg16 op1, Addr32 op2) { return FromNameW("or", op1, op2); }
        public static OpCode OrW(Addr32 op1, Reg16 op2) { return FromNameW("or", op1, op2); }
        public static OpCode OrW(Addr32 op1, ushort op2) { return FromNameW("or", op1, op2); }

        public static OpCode AdcW(Reg16 op1, Reg16 op2) { return FromNameW("adc", op1, op2); }
        public static OpCode AdcW(Reg16 op1, ushort op2) { return FromNameW("adc", op1, op2); }
        public static OpCode AdcW(Reg16 op1, Addr32 op2) { return FromNameW("adc", op1, op2); }
        public static OpCode AdcW(Addr32 op1, Reg16 op2) { return FromNameW("adc", op1, op2); }
        public static OpCode AdcW(Addr32 op1, ushort op2) { return FromNameW("adc", op1, op2); }

        public static OpCode SbbW(Reg16 op1, Reg16 op2) { return FromNameW("sbb", op1, op2); }
        public static OpCode SbbW(Reg16 op1, ushort op2) { return FromNameW("sbb", op1, op2); }
        public static OpCode SbbW(Reg16 op1, Addr32 op2) { return FromNameW("sbb", op1, op2); }
        public static OpCode SbbW(Addr32 op1, Reg16 op2) { return FromNameW("sbb", op1, op2); }
        public static OpCode SbbW(Addr32 op1, ushort op2) { return FromNameW("sbb", op1, op2); }

        public static OpCode AndW(Reg16 op1, Reg16 op2) { return FromNameW("and", op1, op2); }
        public static OpCode AndW(Reg16 op1, ushort op2) { return FromNameW("and", op1, op2); }
        public static OpCode AndW(Reg16 op1, Addr32 op2) { return FromNameW("and", op1, op2); }
        public static OpCode AndW(Addr32 op1, Reg16 op2) { return FromNameW("and", op1, op2); }
        public static OpCode AndW(Addr32 op1, ushort op2) { return FromNameW("and", op1, op2); }

        public static OpCode SubW(Reg16 op1, Reg16 op2) { return FromNameW("sub", op1, op2); }
        public static OpCode SubW(Reg16 op1, ushort op2) { return FromNameW("sub", op1, op2); }
        public static OpCode SubW(Reg16 op1, Addr32 op2) { return FromNameW("sub", op1, op2); }
        public static OpCode SubW(Addr32 op1, Reg16 op2) { return FromNameW("sub", op1, op2); }
        public static OpCode SubW(Addr32 op1, ushort op2) { return FromNameW("sub", op1, op2); }

        public static OpCode XorW(Reg16 op1, Reg16 op2) { return FromNameW("xor", op1, op2); }
        public static OpCode XorW(Reg16 op1, ushort op2) { return FromNameW("xor", op1, op2); }
        public static OpCode XorW(Reg16 op1, Addr32 op2) { return FromNameW("xor", op1, op2); }
        public static OpCode XorW(Addr32 op1, Reg16 op2) { return FromNameW("xor", op1, op2); }
        public static OpCode XorW(Addr32 op1, ushort op2) { return FromNameW("xor", op1, op2); }

        public static OpCode CmpW(Reg16 op1, Reg16 op2) { return FromNameW("cmp", op1, op2); }
        public static OpCode CmpW(Reg16 op1, ushort op2) { return FromNameW("cmp", op1, op2); }
        public static OpCode CmpW(Reg16 op1, Addr32 op2) { return FromNameW("cmp", op1, op2); }
        public static OpCode CmpW(Addr32 op1, Reg16 op2) { return FromNameW("cmp", op1, op2); }
        public static OpCode CmpW(Addr32 op1, ushort op2) { return FromNameW("cmp", op1, op2); }

        public static OpCode TestW(Reg16 op1, Reg16 op2) { return FromNameW("test", op1, op2); }
        public static OpCode TestW(Reg16 op1, ushort op2) { return FromNameW("test", op1, op2); }
        public static OpCode TestW(Reg16 op1, Addr32 op2) { return TestW(op2, op1); }
        public static OpCode TestW(Addr32 op1, Reg16 op2) { return FromNameW("test", op1, op2); }
        public static OpCode TestW(Addr32 op1, ushort op2) { return FromNameW("test", op1, op2); }

        public static OpCode XchgW(Reg16 op1, Reg16 op2) { return FromNameW("xchg", op1, op2); }
        public static OpCode XchgW(Reg16 op1, Addr32 op2) { return FromNameW("xchg", op1, op2); }
        public static OpCode XchgW(Addr32 op1, Reg16 op2) { return XchgW(op2, op1); }

        public static OpCode FromNameW(string op, Reg16 op1, Reg16 op2)
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

        public static OpCode FromNameW(string op, Reg16 op1, ushort op2)
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

        public static OpCode FromNameW(string op, Reg16 op1, Addr32 op2)
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

        public static OpCode FromNameW(string op, Addr32 op1, Reg16 op2)
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

        public static OpCode FromNameW(string op, Addr32 op1, ushort op2)
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
