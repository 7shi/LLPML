using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Push, Pop, Inc, Dec, Not, Neg, Mul, Imul, Div, Idiv

        public static OpCode Push(ushort op1)
        {
            return new OpCode(new byte[] { 0x66, 0x68 }, op1);
        }
        public static OpCode Push(Reg16 op1) { return FromName("push", op1); }
        public static OpCode PushW(Addr32 op1) { return FromNameW("push", op1); }

        public static OpCode Pop(Reg16 op1) { return FromName("pop", op1); }
        public static OpCode PopW(Addr32 op1) { return FromNameW("pop", op1); }

        public static OpCode Inc(Reg16 op1) { return FromName("inc", op1); }
        public static OpCode IncW(Addr32 op1) { return FromNameW("inc", op1); }

        public static OpCode Dec(Reg16 op1) { return FromName("dec", op1); }
        public static OpCode DecW(Addr32 op1) { return FromNameW("dec", op1); }

        public static OpCode Not(Reg16 op1) { return FromName("not", op1); }
        public static OpCode NotW(Addr32 op1) { return FromNameW("not", op1); }

        public static OpCode Neg(Reg16 op1) { return FromName("neg", op1); }
        public static OpCode NegW(Addr32 op1) { return FromNameW("neg", op1); }

        public static OpCode Mul(Reg16 op1) { return FromName("mul", op1); }
        public static OpCode MulW(Addr32 op1) { return FromNameW("mul", op1); }

        public static OpCode Imul(Reg16 op1) { return FromName("imul", op1); }
        public static OpCode ImulW(Addr32 op1) { return FromNameW("imul", op1); }

        public static OpCode Div(Reg16 op1) { return FromName("div", op1); }
        public static OpCode DivW(Addr32 op1) { return FromNameW("div", op1); }

        public static OpCode Idiv(Reg16 op1) { return FromName("idiv", op1); }
        public static OpCode IdivW(Addr32 op1) { return FromNameW("idiv", op1); }

        public static OpCode FromName(string op, Reg16 op1)
        {
            switch (op)
            {
                case "push":
                    return new OpCode(new byte[] { 0x66, (byte)(0x50 + op1) });
                case "pop":
                    return new OpCode(new byte[] { 0x66, (byte)(0x58 + op1) });
                case "inc":
                    return new OpCode(new byte[] { 0x66, (byte)(0x40 + op1) });
                case "dec":
                    return new OpCode(new byte[] { 0x66, (byte)(0x48 + op1) });
                case "not":
                    return new OpCode(new byte[] { 0x66, 0xf7, (byte)(0xd0 + op1) });
                case "neg":
                    return new OpCode(new byte[] { 0x66, 0xf7, (byte)(0xd8 + op1) });
                case "mul":
                    return new OpCode(new byte[] { 0x66, 0xf7, (byte)(0xe0 + op1) });
                case "imul":
                    return new OpCode(new byte[] { 0x66, 0xf7, (byte)(0xe8 + op1) });
                case "div":
                    return new OpCode(new byte[] { 0x66, 0xf7, (byte)(0xf0 + op1) });
                case "idiv":
                    return new OpCode(new byte[] { 0x66, 0xf7, (byte)(0xf8 + op1) });
                default:
                    throw new Exception("invalid operator: " + op);
            }
        }

        public static OpCode FromNameW(string op, Addr32 op1)
        {
            switch (op)
            {
                case "push":
                    return new OpCode(new byte[] { 0x66, 0xff }, null, new Addr32(op1, 6));
                case "pop":
                    return new OpCode(new byte[] { 0x66, 0x8f }, null, op1);
                case "inc":
                    return new OpCode(new byte[] { 0x66, 0xff }, null, op1);
                case "dec":
                    return new OpCode(new byte[] { 0x66, 0xff }, null, new Addr32(op1, 1));
                case "not":
                    return new OpCode(new byte[] { 0x66, 0xf7 }, null, new Addr32(op1, 2));
                case "neg":
                    return new OpCode(new byte[] { 0x66, 0xf7 }, null, new Addr32(op1, 3));
                case "mul":
                    return new OpCode(new byte[] { 0x66, 0xf7 }, null, new Addr32(op1, 4));
                case "imul":
                    return new OpCode(new byte[] { 0x66, 0xf7 }, null, new Addr32(op1, 5));
                case "div":
                    return new OpCode(new byte[] { 0x66, 0xf7 }, null, new Addr32(op1, 6));
                case "idiv":
                    return new OpCode(new byte[] { 0x66, 0xf7 }, null, new Addr32(op1, 7));
                default:
                    throw new Exception("invalid operator: " + op);
            }
        }
    }
}
