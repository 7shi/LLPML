using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Mov, Add, Or, Adc, Sbb, And, Sub, Xor, Cmp, Test, Xchg

        public static OpCode MovB(Reg8 op1, Reg8 op2) { return FromNameB("mov", op1, op2); }
        public static OpCode MovB(Reg8 op1, byte op2) { return FromNameB("mov", op1, op2); }
        public static OpCode MovB(Reg8 op1, Addr32 op2) { return FromNameB("mov", op1, op2); }
        public static OpCode MovB(Addr32 op1, Reg8 op2) { return FromNameB("mov", op1, op2); }
        public static OpCode MovB(Addr32 op1, byte op2) { return FromNameB("mov", op1, op2); }

        public static OpCode AddB(Reg8 op1, Reg8 op2) { return FromNameB("add", op1, op2); }
        public static OpCode AddB(Reg8 op1, byte op2) { return FromNameB("add", op1, op2); }
        public static OpCode AddB(Reg8 op1, Addr32 op2) { return FromNameB("add", op1, op2); }
        public static OpCode AddB(Addr32 op1, Reg8 op2) { return FromNameB("add", op1, op2); }
        public static OpCode AddB(Addr32 op1, byte op2) { return FromNameB("add", op1, op2); }

        public static OpCode OrB(Reg8 op1, Reg8 op2) { return FromNameB("or", op1, op2); }
        public static OpCode OrB(Reg8 op1, byte op2) { return FromNameB("or", op1, op2); }
        public static OpCode OrB(Reg8 op1, Addr32 op2) { return FromNameB("or", op1, op2); }
        public static OpCode OrB(Addr32 op1, Reg8 op2) { return FromNameB("or", op1, op2); }
        public static OpCode OrB(Addr32 op1, byte op2) { return FromNameB("or", op1, op2); }

        public static OpCode AdcB(Reg8 op1, Reg8 op2) { return FromNameB("adc", op1, op2); }
        public static OpCode AdcB(Reg8 op1, byte op2) { return FromNameB("adc", op1, op2); }
        public static OpCode AdcB(Reg8 op1, Addr32 op2) { return FromNameB("adc", op1, op2); }
        public static OpCode AdcB(Addr32 op1, Reg8 op2) { return FromNameB("adc", op1, op2); }
        public static OpCode AdcB(Addr32 op1, byte op2) { return FromNameB("adc", op1, op2); }

        public static OpCode SbbB(Reg8 op1, Reg8 op2) { return FromNameB("sbb", op1, op2); }
        public static OpCode SbbB(Reg8 op1, byte op2) { return FromNameB("sbb", op1, op2); }
        public static OpCode SbbB(Reg8 op1, Addr32 op2) { return FromNameB("sbb", op1, op2); }
        public static OpCode SbbB(Addr32 op1, Reg8 op2) { return FromNameB("sbb", op1, op2); }
        public static OpCode SbbB(Addr32 op1, byte op2) { return FromNameB("sbb", op1, op2); }

        public static OpCode AndB(Reg8 op1, Reg8 op2) { return FromNameB("and", op1, op2); }
        public static OpCode AndB(Reg8 op1, byte op2) { return FromNameB("and", op1, op2); }
        public static OpCode AndB(Reg8 op1, Addr32 op2) { return FromNameB("and", op1, op2); }
        public static OpCode AndB(Addr32 op1, Reg8 op2) { return FromNameB("and", op1, op2); }
        public static OpCode AndB(Addr32 op1, byte op2) { return FromNameB("and", op1, op2); }

        public static OpCode SubB(Reg8 op1, Reg8 op2) { return FromNameB("sub", op1, op2); }
        public static OpCode SubB(Reg8 op1, byte op2) { return FromNameB("sub", op1, op2); }
        public static OpCode SubB(Reg8 op1, Addr32 op2) { return FromNameB("sub", op1, op2); }
        public static OpCode SubB(Addr32 op1, Reg8 op2) { return FromNameB("sub", op1, op2); }
        public static OpCode SubB(Addr32 op1, byte op2) { return FromNameB("sub", op1, op2); }

        public static OpCode XorB(Reg8 op1, Reg8 op2) { return FromNameB("xor", op1, op2); }
        public static OpCode XorB(Reg8 op1, byte op2) { return FromNameB("xor", op1, op2); }
        public static OpCode XorB(Reg8 op1, Addr32 op2) { return FromNameB("xor", op1, op2); }
        public static OpCode XorB(Addr32 op1, Reg8 op2) { return FromNameB("xor", op1, op2); }
        public static OpCode XorB(Addr32 op1, byte op2) { return FromNameB("xor", op1, op2); }

        public static OpCode CmpB(Reg8 op1, Reg8 op2) { return FromNameB("cmp", op1, op2); }
        public static OpCode CmpB(Reg8 op1, byte op2) { return FromNameB("cmp", op1, op2); }
        public static OpCode CmpB(Reg8 op1, Addr32 op2) { return FromNameB("cmp", op1, op2); }
        public static OpCode CmpB(Addr32 op1, Reg8 op2) { return FromNameB("cmp", op1, op2); }
        public static OpCode CmpB(Addr32 op1, byte op2) { return FromNameB("cmp", op1, op2); }

        public static OpCode TestB(Reg8 op1, Reg8 op2) { return FromNameB("test", op1, op2); }
        public static OpCode TestB(Reg8 op1, byte op2) { return FromNameB("test", op1, op2); }
        public static OpCode TestB(Reg8 op1, Addr32 op2) { return TestB(op2, op1); }
        public static OpCode TestB(Addr32 op1, Reg8 op2) { return FromNameB("test", op1, op2); }
        public static OpCode TestB(Addr32 op1, byte op2) { return FromNameB("test", op1, op2); }

        public static OpCode XchgB(Reg8 op1, Reg8 op2) { return FromNameB("xchg", op1, op2); }
        public static OpCode XchgB(Reg8 op1, Addr32 op2) { return FromNameB("xchg", op1, op2); }
        public static OpCode XchgB(Addr32 op1, Reg8 op2) { return XchgB(op2, op1); }

        public static OpCode FromNameB(string op, Reg8 op1, Reg8 op2)
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

        public static OpCode FromNameB(string op, Reg8 op1, byte op2)
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

        public static OpCode FromNameB(string op, Reg8 op1, Addr32 op2)
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

        public static OpCode FromNameB(string op, Addr32 op1, Reg8 op2)
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

        public static OpCode FromNameB(string op, Addr32 op1, byte op2)
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
