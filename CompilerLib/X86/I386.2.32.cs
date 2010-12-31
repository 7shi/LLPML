using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Mov, Add, Or, Adc, Sbb, And, Sub, Xor, Cmp, Test, Xchg

        public static OpCode Mov(Reg32 op1, Reg32 op2) { return FromName2("mov", op1, op2); }
        public static OpCode MovR(Reg32 op1, Val32 op2) { return FromName2R("mov", op1, op2); }
        public static OpCode MovRA(Reg32 op1, Addr32 op2) { return FromName2RA("mov", op1, op2); }
        public static OpCode MovAR(Addr32 op1, Reg32 op2) { return FromName2AR("mov", op1, op2); }
        public static OpCode MovA(Addr32 op1, Val32 op2) { return FromName2A("mov", op1, op2); }

        public static OpCode Add(Reg32 op1, Reg32 op2) { return FromName2("add", op1, op2); }
        public static OpCode AddR(Reg32 op1, Val32 op2) { return FromName2R("add", op1, op2); }
        public static OpCode AddRA(Reg32 op1, Addr32 op2) { return FromName2RA("add", op1, op2); }
        public static OpCode AddAR(Addr32 op1, Reg32 op2) { return FromName2AR("add", op1, op2); }
        public static OpCode AddA(Addr32 op1, Val32 op2) { return FromName2A("add", op1, op2); }

        public static OpCode Or(Reg32 op1, Reg32 op2) { return FromName2("or", op1, op2); }
        public static OpCode OrR(Reg32 op1, Val32 op2) { return FromName2R("or", op1, op2); }
        public static OpCode OrRA(Reg32 op1, Addr32 op2) { return FromName2RA("or", op1, op2); }
        public static OpCode OrAR(Addr32 op1, Reg32 op2) { return FromName2AR("or", op1, op2); }
        public static OpCode OrA(Addr32 op1, Val32 op2) { return FromName2A("or", op1, op2); }

        public static OpCode Adc(Reg32 op1, Reg32 op2) { return FromName2("adc", op1, op2); }
        public static OpCode AdcR(Reg32 op1, Val32 op2) { return FromName2R("adc", op1, op2); }
        public static OpCode AdcRA(Reg32 op1, Addr32 op2) { return FromName2RA("adc", op1, op2); }
        public static OpCode AdcAR(Addr32 op1, Reg32 op2) { return FromName2AR("adc", op1, op2); }
        public static OpCode AdcA(Addr32 op1, Val32 op2) { return FromName2A("adc", op1, op2); }

        public static OpCode Sbb(Reg32 op1, Reg32 op2) { return FromName2("sbb", op1, op2); }
        public static OpCode SbbR(Reg32 op1, Val32 op2) { return FromName2R("sbb", op1, op2); }
        public static OpCode SbbRA(Reg32 op1, Addr32 op2) { return FromName2RA("sbb", op1, op2); }
        public static OpCode SbbAR(Addr32 op1, Reg32 op2) { return FromName2AR("sbb", op1, op2); }
        public static OpCode SbbA(Addr32 op1, Val32 op2) { return FromName2A("sbb", op1, op2); }

        public static OpCode And(Reg32 op1, Reg32 op2) { return FromName2("and", op1, op2); }
        public static OpCode AndR(Reg32 op1, Val32 op2) { return FromName2R("and", op1, op2); }
        public static OpCode AndRA(Reg32 op1, Addr32 op2) { return FromName2RA("and", op1, op2); }
        public static OpCode AndAR(Addr32 op1, Reg32 op2) { return FromName2AR("and", op1, op2); }
        public static OpCode AndA(Addr32 op1, Val32 op2) { return FromName2A("and", op1, op2); }

        public static OpCode Sub(Reg32 op1, Reg32 op2) { return FromName2("sub", op1, op2); }
        public static OpCode SubR(Reg32 op1, Val32 op2) { return FromName2R("sub", op1, op2); }
        public static OpCode SubRA(Reg32 op1, Addr32 op2) { return FromName2RA("sub", op1, op2); }
        public static OpCode SubAR(Addr32 op1, Reg32 op2) { return FromName2AR("sub", op1, op2); }
        public static OpCode SubA(Addr32 op1, Val32 op2) { return FromName2A("sub", op1, op2); }

        public static OpCode Xor(Reg32 op1, Reg32 op2) { return FromName2("xor", op1, op2); }
        public static OpCode XorR(Reg32 op1, Val32 op2) { return FromName2R("xor", op1, op2); }
        public static OpCode XorRA(Reg32 op1, Addr32 op2) { return FromName2RA("xor", op1, op2); }
        public static OpCode XorAR(Addr32 op1, Reg32 op2) { return FromName2AR("xor", op1, op2); }
        public static OpCode XorA(Addr32 op1, Val32 op2) { return FromName2A("xor", op1, op2); }

        public static OpCode Cmp(Reg32 op1, Reg32 op2) { return FromName2("cmp", op1, op2); }
        public static OpCode CmpR(Reg32 op1, Val32 op2) { return FromName2R("cmp", op1, op2); }
        public static OpCode CmpRA(Reg32 op1, Addr32 op2) { return FromName2RA("cmp", op1, op2); }
        public static OpCode CmpAR(Addr32 op1, Reg32 op2) { return FromName2AR("cmp", op1, op2); }
        public static OpCode CmpA(Addr32 op1, Val32 op2) { return FromName2A("cmp", op1, op2); }

        public static OpCode Test(Reg32 op1, Reg32 op2) { return FromName2("test", op1, op2); }
        public static OpCode TestR(Reg32 op1, Val32 op2) { return FromName2R("test", op1, op2); }
        public static OpCode TestRA(Reg32 op1, Addr32 op2) { return TestAR(op2, op1); }
        public static OpCode TestAR(Addr32 op1, Reg32 op2) { return FromName2AR("test", op1, op2); }
        public static OpCode TestA(Addr32 op1, Val32 op2) { return FromName2A("test", op1, op2); }

        public static OpCode Xchg(Reg32 op1, Reg32 op2) { return FromName2("xchg", op1, op2); }
        public static OpCode XchgRA(Reg32 op1, Addr32 op2) { return FromName2RA("xchg", op1, op2); }
        public static OpCode XchgAR(Addr32 op1, Reg32 op2) { return XchgRA(op2, op1); }

        public static int GetOperatorCode(string op)
        {
            switch (op)
            {
                case "add": return 0;
                case "or": return 1;
                case "adc": return 2;
                case "sbb": return 3;
                case "and": return 4;
                case "sub": return 5;
                case "xor": return 6;
                case "cmp": return 7;
            }
            return -1;
        }

        public static OpCode FromName2(string op, Reg32 op1, Reg32 op2)
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
                    if (op1 == Reg32.EAX)
                        return OpCode.NewBytes(Util.GetBytes1((byte)(0x90 + op2)));
                    else if (op2 == Reg32.EAX)
                        return OpCode.NewBytes(Util.GetBytes1((byte)(0x90 + op1)));
                    else
                        b = 0x87;
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    b = (byte)(code * 8 + 1);
                    break;
            }
            return OpCode.NewBytes(Util.GetBytes2(b, (byte)(0xc0 + (((int)op2) << 3) + op1)));
        }

        public static OpCode FromName2R(string op, Reg32 op1, Val32 op2)
        {
            byte[] bytes;
            switch (op)
            {
                case "mov":
                    bytes = Util.GetBytes1((byte)(0xb8 + op1));
                    break;
                case "test":
                    if (op1 == Reg32.EAX)
                        bytes = Util.GetBytes1(0xa9);
                    else
                        bytes = Util.GetBytes2(0xf7, (byte)(0xc0 + op1));
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    if (op1 == Reg32.EAX)
                        bytes = Util.GetBytes1((byte)(code * 8 + 5));
                    else
                        bytes = Util.GetBytes2(0x81, (byte)(code * 8 + 0xc0 + op1));
                    break;
            }
            return OpCode.NewD(bytes, op2);
        }

        public static OpCode FromName2RA(string op, Reg32 op1, Addr32 op2)
        {
            byte b;
            switch (op)
            {
                case "mov":
                    if (op1 == Reg32.EAX && op2.IsAddress)
                        return OpCode.NewD(Util.GetBytes1(0xa1), op2.Address);
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
            return OpCode.NewA(Util.GetBytes1(b), Addr32.NewAdM(op2, (byte)op1));
        }

        public static OpCode FromName2AR(string op, Addr32 op1, Reg32 op2)
        {
            byte b;
            switch (op)
            {
                case "mov":
                    if (op2 == Reg32.EAX && op1.IsAddress)
                        return OpCode.NewD(Util.GetBytes1(0xa3), op1.Address);
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
            return OpCode.NewA(Util.GetBytes1(b), Addr32.NewAdM(op1, (byte)op2));
        }

        public static OpCode FromName2A(string op, Addr32 op1, Val32 op2)
        {
            switch (op)
            {
                case "mov":
                    return OpCode.NewDA(Util.GetBytes1(0xc7), op2, op1);
                case "test":
                    return OpCode.NewDA(Util.GetBytes1(0xf7), op2, op1);
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    return OpCode.NewDA(Util.GetBytes1(0x81), op2, Addr32.NewAdM(op1, (byte)code));
            }
        }
    }
}
