using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Movzx, Movsx

        public static OpCode MovzxB(Reg32 op1, Reg8 op2) { return FromNameB("movzx", op1, op2); }
        public static OpCode MovzxBA(Reg32 op1, Addr32 op2) { return FromNameBA("movzx", op1, op2); }

        public static OpCode MovzxWB(Reg16 op1, Reg8 op2) { return FromNameWB("movzx", op1, op2); }
        public static OpCode MovzxWBA(Reg16 op1, Addr32 op2) { return FromNameWBA("movzx", op1, op2); }

        public static OpCode MovsxB(Reg32 op1, Reg8 op2) { return FromNameB("movsx", op1, op2); }
        public static OpCode MovsxBA(Reg32 op1, Addr32 op2) { return FromNameBA("movsx", op1, op2); }

        public static OpCode MovsxWB(Reg16 op1, Reg8 op2) { return FromNameWB("movsx", op1, op2); }
        public static OpCode MovsxWBA(Reg16 op1, Addr32 op2) { return FromNameWBA("movsx", op1, op2); }

        public static OpCode FromNameB(string op, Reg32 op1, Reg8 op2)
        {
            byte b;
            switch (op)
            {
                case "movzx":
                    b = 0xb6;
                    break;
                case "movsx":
                    b = 0xbe;
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            return OpCode.NewBytes(Util.GetBytes3(0x0f, b, (byte)(0xc0 + (((int)op1) << 3) + op2)));
        }

        public static OpCode FromNameWB(string op, Reg16 op1, Reg8 op2)
        {
            byte b;
            switch (op)
            {
                case "movzx":
                    b = 0xb6;
                    break;
                case "movsx":
                    b = 0xbe;
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            return OpCode.NewBytes(Util.GetBytes4(0x66, 0x0f, b, (byte)(0xc0 + (((int)op1) << 3) + op2)));
        }

        public static OpCode FromNameBA(string op, Reg32 op1, Addr32 op2)
        {
            byte b;
            switch (op)
            {
                case "movzx":
                    b = 0xb6;
                    break;
                case "movsx":
                    b = 0xbe;
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            return OpCode.NewA(Util.GetBytes2(0x0f, b), Addr32.NewAdM(op2, (byte)op1));
        }

        public static OpCode FromNameWBA(string op, Reg16 op1, Addr32 op2)
        {
            byte b;
            switch (op)
            {
                case "movzx":
                    b = 0xb6;
                    break;
                case "movsx":
                    b = 0xbe;
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            return OpCode.NewA(Util.GetBytes3(0x66, 0x0f, b), Addr32.NewAdM(op2, (byte)op1));
        }
    }
}
