using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Mov, Add, Or, Adc, Sbb, And, Sub, Xor, Cmp, Test, Xchg

        public static OpCode Mov(Reg8 op1, Reg8 op2) { return FromName("mov", op1, op2); }
        public static OpCode Mov(Reg8 op1, byte op2) { return FromName("mov", op1, op2); }
        public static OpCode Mov(Reg8 op1, Addr32 op2) { return FromName("mov", op1, op2); }
        public static OpCode Mov(Addr32 op1, Reg8 op2) { return FromName("mov", op1, op2); }
        public static OpCode Mov(Addr32 op1, byte op2) { return FromName("mov", op1, op2); }

        public static OpCode Add(Reg8 op1, Reg8 op2) { return FromName("add", op1, op2); }
        public static OpCode Add(Reg8 op1, byte op2) { return FromName("add", op1, op2); }
        public static OpCode Add(Reg8 op1, Addr32 op2) { return FromName("add", op1, op2); }
        public static OpCode Add(Addr32 op1, Reg8 op2) { return FromName("add", op1, op2); }
        public static OpCode Add(Addr32 op1, byte op2) { return FromName("add", op1, op2); }

        public static OpCode Or(Reg8 op1, Reg8 op2) { return FromName("or", op1, op2); }
        public static OpCode Or(Reg8 op1, byte op2) { return FromName("or", op1, op2); }
        public static OpCode Or(Reg8 op1, Addr32 op2) { return FromName("or", op1, op2); }
        public static OpCode Or(Addr32 op1, Reg8 op2) { return FromName("or", op1, op2); }
        public static OpCode Or(Addr32 op1, byte op2) { return FromName("or", op1, op2); }

        public static OpCode Adc(Reg8 op1, Reg8 op2) { return FromName("adc", op1, op2); }
        public static OpCode Adc(Reg8 op1, byte op2) { return FromName("adc", op1, op2); }
        public static OpCode Adc(Reg8 op1, Addr32 op2) { return FromName("adc", op1, op2); }
        public static OpCode Adc(Addr32 op1, Reg8 op2) { return FromName("adc", op1, op2); }
        public static OpCode Adc(Addr32 op1, byte op2) { return FromName("adc", op1, op2); }

        public static OpCode Sbb(Reg8 op1, Reg8 op2) { return FromName("sbb", op1, op2); }
        public static OpCode Sbb(Reg8 op1, byte op2) { return FromName("sbb", op1, op2); }
        public static OpCode Sbb(Reg8 op1, Addr32 op2) { return FromName("sbb", op1, op2); }
        public static OpCode Sbb(Addr32 op1, Reg8 op2) { return FromName("sbb", op1, op2); }
        public static OpCode Sbb(Addr32 op1, byte op2) { return FromName("sbb", op1, op2); }

        public static OpCode And(Reg8 op1, Reg8 op2) { return FromName("and", op1, op2); }
        public static OpCode And(Reg8 op1, byte op2) { return FromName("and", op1, op2); }
        public static OpCode And(Reg8 op1, Addr32 op2) { return FromName("and", op1, op2); }
        public static OpCode And(Addr32 op1, Reg8 op2) { return FromName("and", op1, op2); }
        public static OpCode And(Addr32 op1, byte op2) { return FromName("and", op1, op2); }

        public static OpCode Sub(Reg8 op1, Reg8 op2) { return FromName("sub", op1, op2); }
        public static OpCode Sub(Reg8 op1, byte op2) { return FromName("sub", op1, op2); }
        public static OpCode Sub(Reg8 op1, Addr32 op2) { return FromName("sub", op1, op2); }
        public static OpCode Sub(Addr32 op1, Reg8 op2) { return FromName("sub", op1, op2); }
        public static OpCode Sub(Addr32 op1, byte op2) { return FromName("sub", op1, op2); }

        public static OpCode Xor(Reg8 op1, Reg8 op2) { return FromName("xor", op1, op2); }
        public static OpCode Xor(Reg8 op1, byte op2) { return FromName("xor", op1, op2); }
        public static OpCode Xor(Reg8 op1, Addr32 op2) { return FromName("xor", op1, op2); }
        public static OpCode Xor(Addr32 op1, Reg8 op2) { return FromName("xor", op1, op2); }
        public static OpCode Xor(Addr32 op1, byte op2) { return FromName("xor", op1, op2); }

        public static OpCode Cmp(Reg8 op1, Reg8 op2) { return FromName("cmp", op1, op2); }
        public static OpCode Cmp(Reg8 op1, byte op2) { return FromName("cmp", op1, op2); }
        public static OpCode Cmp(Reg8 op1, Addr32 op2) { return FromName("cmp", op1, op2); }
        public static OpCode Cmp(Addr32 op1, Reg8 op2) { return FromName("cmp", op1, op2); }
        public static OpCode Cmp(Addr32 op1, byte op2) { return FromName("cmp", op1, op2); }

        public static OpCode Test(Reg8 op1, Reg8 op2) { return FromName("test", op1, op2); }
        public static OpCode Test(Reg8 op1, byte op2) { return FromName("test", op1, op2); }
        public static OpCode Test(Reg8 op1, Addr32 op2) { return Test(op2, op1); }
        public static OpCode Test(Addr32 op1, Reg8 op2) { return FromName("test", op1, op2); }
        public static OpCode Test(Addr32 op1, byte op2) { return FromName("test", op1, op2); }

        public static OpCode Xchg(Reg8 op1, Reg8 op2) { return FromName("xchg", op1, op2); }
        public static OpCode Xchg(Reg8 op1, Addr32 op2) { return FromName("xchg", op1, op2); }
        public static OpCode Xchg(Addr32 op1, Reg8 op2) { return Xchg(op2, op1); }

        public static OpCode FromName(string op, Reg8 op1, Reg8 op2)
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
            return new OpCode(new byte[] { b, (byte)(0xc0 + (((int)op2) << 3) + op1) });
        }

        public static OpCode FromName(string op, Reg8 op1, byte op2)
        {
            byte[] bytes;
            switch (op)
            {
                case "mov":
                    bytes = new byte[] { (byte)(0xb0 + op1) };
                    break;
                case "test":
                    if (op1 == Reg8.AL)
                        bytes = new byte[] { 0xa8 };
                    else
                        bytes = new byte[] { 0xf6, (byte)(0xc0 + op1) };
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    if (op1 == Reg8.AL)
                        bytes = new byte[] { (byte)(code * 8 + 4) };
                    else
                        bytes = new byte[] { 0x80, (byte)(code * 8 + 0xc0 + op1) };
                    break;
            }
            return new OpCode(bytes, op2);
        }

        public static OpCode FromName(string op, Reg8 op1, Addr32 op2)
        {
            byte b;
            switch (op)
            {
                case "mov":
                    if (op1 == Reg8.AL && op2.IsAddress)
                        return new OpCode(new byte[] { 0xa0 }, op2.Address);
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
            return new OpCode(new byte[] { b }, null, new Addr32(op2, (byte)op1));
        }

        public static OpCode FromName(string op, Addr32 op1, Reg8 op2)
        {
            byte b;
            switch (op)
            {
                case "mov":
                    if (op2 == Reg8.AL && op1.IsAddress)
                        return new OpCode(new byte[] { 0xa2 }, op1.Address);
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
            return new OpCode(new byte[] { b }, null, new Addr32(op1, (byte)op2));
        }

        public static OpCode FromName(string op, Addr32 op1, byte op2)
        {
            switch (op)
            {
                case "mov":
                    return new OpCode(new byte[] { 0xc6 }, op2, op1);
                case "test":
                    return new OpCode(new byte[] { 0xf6 }, op2, op1);
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    return new OpCode(new byte[] { 0x80 }, op2, new Addr32(op1, (byte)code));
            }
        }
    }
}
