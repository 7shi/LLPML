using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Mov, Add, Or, Adc, Sbb, And, Sub, Xor, Cmp, Test, Xchg

        public static OpCode Mov(Reg16 op1, Reg16 op2) { return FromName("mov", op1, op2); }
        public static OpCode Mov(Reg16 op1, ushort op2) { return FromName("mov", op1, op2); }
        public static OpCode Mov(Reg16 op1, Addr32 op2) { return FromName("mov", op1, op2); }
        public static OpCode Mov(Addr32 op1, Reg16 op2) { return FromName("mov", op1, op2); }
        public static OpCode Mov(Addr32 op1, ushort op2) { return FromName("mov", op1, op2); }

        public static OpCode Add(Reg16 op1, Reg16 op2) { return FromName("add", op1, op2); }
        public static OpCode Add(Reg16 op1, ushort op2) { return FromName("add", op1, op2); }
        public static OpCode Add(Reg16 op1, Addr32 op2) { return FromName("add", op1, op2); }
        public static OpCode Add(Addr32 op1, Reg16 op2) { return FromName("add", op1, op2); }
        public static OpCode Add(Addr32 op1, ushort op2) { return FromName("add", op1, op2); }

        public static OpCode Or(Reg16 op1, Reg16 op2) { return FromName("or", op1, op2); }
        public static OpCode Or(Reg16 op1, ushort op2) { return FromName("or", op1, op2); }
        public static OpCode Or(Reg16 op1, Addr32 op2) { return FromName("or", op1, op2); }
        public static OpCode Or(Addr32 op1, Reg16 op2) { return FromName("or", op1, op2); }
        public static OpCode Or(Addr32 op1, ushort op2) { return FromName("or", op1, op2); }

        public static OpCode Adc(Reg16 op1, Reg16 op2) { return FromName("adc", op1, op2); }
        public static OpCode Adc(Reg16 op1, ushort op2) { return FromName("adc", op1, op2); }
        public static OpCode Adc(Reg16 op1, Addr32 op2) { return FromName("adc", op1, op2); }
        public static OpCode Adc(Addr32 op1, Reg16 op2) { return FromName("adc", op1, op2); }
        public static OpCode Adc(Addr32 op1, ushort op2) { return FromName("adc", op1, op2); }

        public static OpCode Sbb(Reg16 op1, Reg16 op2) { return FromName("sbb", op1, op2); }
        public static OpCode Sbb(Reg16 op1, ushort op2) { return FromName("sbb", op1, op2); }
        public static OpCode Sbb(Reg16 op1, Addr32 op2) { return FromName("sbb", op1, op2); }
        public static OpCode Sbb(Addr32 op1, Reg16 op2) { return FromName("sbb", op1, op2); }
        public static OpCode Sbb(Addr32 op1, ushort op2) { return FromName("sbb", op1, op2); }

        public static OpCode And(Reg16 op1, Reg16 op2) { return FromName("and", op1, op2); }
        public static OpCode And(Reg16 op1, ushort op2) { return FromName("and", op1, op2); }
        public static OpCode And(Reg16 op1, Addr32 op2) { return FromName("and", op1, op2); }
        public static OpCode And(Addr32 op1, Reg16 op2) { return FromName("and", op1, op2); }
        public static OpCode And(Addr32 op1, ushort op2) { return FromName("and", op1, op2); }

        public static OpCode Sub(Reg16 op1, Reg16 op2) { return FromName("sub", op1, op2); }
        public static OpCode Sub(Reg16 op1, ushort op2) { return FromName("sub", op1, op2); }
        public static OpCode Sub(Reg16 op1, Addr32 op2) { return FromName("sub", op1, op2); }
        public static OpCode Sub(Addr32 op1, Reg16 op2) { return FromName("sub", op1, op2); }
        public static OpCode Sub(Addr32 op1, ushort op2) { return FromName("sub", op1, op2); }

        public static OpCode Xor(Reg16 op1, Reg16 op2) { return FromName("xor", op1, op2); }
        public static OpCode Xor(Reg16 op1, ushort op2) { return FromName("xor", op1, op2); }
        public static OpCode Xor(Reg16 op1, Addr32 op2) { return FromName("xor", op1, op2); }
        public static OpCode Xor(Addr32 op1, Reg16 op2) { return FromName("xor", op1, op2); }
        public static OpCode Xor(Addr32 op1, ushort op2) { return FromName("xor", op1, op2); }

        public static OpCode Cmp(Reg16 op1, Reg16 op2) { return FromName("cmp", op1, op2); }
        public static OpCode Cmp(Reg16 op1, ushort op2) { return FromName("cmp", op1, op2); }
        public static OpCode Cmp(Reg16 op1, Addr32 op2) { return FromName("cmp", op1, op2); }
        public static OpCode Cmp(Addr32 op1, Reg16 op2) { return FromName("cmp", op1, op2); }
        public static OpCode Cmp(Addr32 op1, ushort op2) { return FromName("cmp", op1, op2); }

        public static OpCode Test(Reg16 op1, Reg16 op2) { return FromName("test", op1, op2); }
        public static OpCode Test(Reg16 op1, ushort op2) { return FromName("test", op1, op2); }
        public static OpCode Test(Reg16 op1, Addr32 op2) { return Test(op2, op1); }
        public static OpCode Test(Addr32 op1, Reg16 op2) { return FromName("test", op1, op2); }
        public static OpCode Test(Addr32 op1, ushort op2) { return FromName("test", op1, op2); }

        public static OpCode Xchg(Reg16 op1, Reg16 op2) { return FromName("xchg", op1, op2); }
        public static OpCode Xchg(Reg16 op1, Addr32 op2) { return FromName("xchg", op1, op2); }
        public static OpCode Xchg(Addr32 op1, Reg16 op2) { return Xchg(op2, op1); }

        public static OpCode FromName(string op, Reg16 op1, Reg16 op2)
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
                        return new OpCode(new byte[] { 0x66, (byte)(0x90 + op2) });
                    else if (op2 == Reg16.AX)
                        return new OpCode(new byte[] { 0x66, (byte)(0x90 + op1) });
                    else
                        b = 0x87;
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    b = (byte)(code * 8 + 1);
                    break;
            }
            return new OpCode(new byte[] { 0x66, b, (byte)(0xc0 + (((int)op2) << 3) + op1) });
        }

        public static OpCode FromName(string op, Reg16 op1, ushort op2)
        {
            byte[] bytes;
            switch (op)
            {
                case "mov":
                    bytes = new byte[] { 0x66, (byte)(0xb8 + op1) };
                    break;
                case "test":
                    if (op1 == Reg16.AX)
                        bytes = new byte[] { 0x66, 0xa9 };
                    else
                        bytes = new byte[] { 0x66, 0xf7, (byte)(0xc0 + op1) };
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    if (op1 == Reg16.AX)
                        bytes = new byte[] { 0x66, (byte)(code * 8 + 5) };
                    else
                        bytes = new byte[] { 0x66, 0x81, (byte)(code * 8 + 0xc0 + op1) };
                    break;
            }
            return new OpCode(bytes, op2);
        }

        public static OpCode FromName(string op, Reg16 op1, Addr32 op2)
        {
            byte b;
            switch (op)
            {
                case "mov":
                    if (op1 == Reg16.AX && op2.IsAddress)
                        return new OpCode(new byte[] { 0x66, 0xa1 }, op2.Address);
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
            return new OpCode(new byte[] { 0x66, b }, null, new Addr32(op2, (byte)op1));
        }

        public static OpCode FromName(string op, Addr32 op1, Reg16 op2)
        {
            byte b;
            switch (op)
            {
                case "mov":
                    if (op2 == Reg16.AX && op1.IsAddress)
                        return new OpCode(new byte[] { 0x66, 0xa3 }, op1.Address);
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
            return new OpCode(new byte[] { 0x66, b }, null, new Addr32(op1, (byte)op2));
        }

        public static OpCode FromName(string op, Addr32 op1, ushort op2)
        {
            switch (op)
            {
                case "mov":
                    return new OpCode(new byte[] { 0x66, 0xc7 }, op2, op1);
                case "test":
                    return new OpCode(new byte[] { 0x66, 0xf7 }, op2, op1);
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    return new OpCode(new byte[] { 0x66, 0x81 }, op2, new Addr32(op1, (byte)code));
            }
        }
    }
}
