using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        public static OpCode Jmp(Addr32 op1)
        {
            return new OpCode(new byte[] { 0xff }, null, new Addr32(op1, 4));
        }

        public static OpCode Jmp(Val32 op1)
        {
            return new OpCode(new byte[] { 0xe9 }, op1, true);
        }

        public static OpCode Jcc(Cc c, Val32 op1)
        {
            return new OpCode(new byte[] { 0x0f, (byte)(0x80 + c) }, op1, true);
        }

        public static OpCode Ret()
        {
            return new OpCode(new byte[] { 0xc3 });
        }

        public static OpCode Ret(ushort op1)
        {
            return new OpCode(new byte[] { 0xc2 }, op1);
        }

        public static OpCode Lea(Reg32 op1, Addr32 op2)
        {
            return new OpCode(new byte[] { 0x8d }, null, new Addr32(op2, (byte)op1));
        }

        public static OpCode Setcc(Cc c, Reg8 op1)
        {
            return new OpCode(new byte[] { 0x0f, (byte)(0x90 + c), (byte)(0xc0 + op1) });
        }

        public static OpCode Enter(ushort op1, byte op2)
        {
            return new OpCode(new byte[] { 0xc8 }, op1, op2);
        }

        public static OpCode Leave()
        {
            return new OpCode(new byte[] { 0xc9 });
        }

        public static OpCode Nop()
        {
            return new OpCode(new byte[] { 0x90 });
        }

        public static OpCode Cdq()
        {
            return new OpCode(new byte[] { 0x99 });
        }
    }
}
