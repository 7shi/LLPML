﻿using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Movzx, Movsx

        public static OpCode MovzxW(Reg32 op1, Reg16 op2) { return FromNameW("movzx", op1, op2); }
        public static OpCode MovzxWA(Reg32 op1, Addr32 op2) { return FromNameWA("movzx", op1, op2); }

        public static OpCode MovsxW(Reg32 op1, Reg16 op2) { return FromNameW("movsx", op1, op2); }
        public static OpCode MovsxWA(Reg32 op1, Addr32 op2) { return FromNameWA("movsx", op1, op2); }

        public static OpCode FromNameW(string op, Reg32 op1, Reg16 op2)
        {
            byte b;
            switch (op)
            {
                case "movzx":
                    b = 0xb7;
                    break;
                case "movsx":
                    b = 0xbf;
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            return OpCode.NewBytes(Util.GetBytes3(0x0f, b, (byte)(0xc0 + (((int)op1) << 3) + op2)));
        }

        public static OpCode FromNameWA(string op, Reg32 op1, Addr32 op2)
        {
            byte b;
            switch (op)
            {
                case "movzx":
                    b = 0xb7;
                    break;
                case "movsx":
                    b = 0xbf;
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            return OpCode.NewA(Util.GetBytes2(0x0f, b), Addr32.NewAdM(op2, (byte)op1));
        }
    }
}
