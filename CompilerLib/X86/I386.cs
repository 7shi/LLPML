using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public class I386
    {
        public static OpCode Push(Ref<uint> op1)
        {
            return new OpCode(new byte[] { 0x68 }, op1);
        }

        public static OpCode Push(Reg32 op1)
        {
            return new OpCode(new byte[] { (byte)(0x50 + (int)op1) });
        }

        public static OpCode Push(Addr32 op1)
        {
            op1.MiddleBits = 6;
            return new OpCode(new byte[] { 0xff }, null, op1);
        }

        public static OpCode Pop(Reg32 op1)
        {
            return new OpCode(new byte[] { (byte)(0x58 + (int)op1) });
        }

        public static OpCode Pop(Addr32 op1)
        {
            return new OpCode(new byte[] { 0x8f }, null, op1);
        }

        public static OpCode Call(Reg32 op1)
        {
            switch (op1)
            {
                case Reg32.EAX:
                    return new OpCode(new byte[] { 0xff, 0xd0 });
            }
            throw new Exception("The method or operation is not implemented.");
        }

        public static OpCode Call(Addr32 op1)
        {
            op1.MiddleBits = 2;
            return new OpCode(new byte[] { 0xff }, null, op1);
        }

        public static OpCode[] Call(CallType call, Addr32 func, object[] args)
        {
            List<OpCode> ret = new List<OpCode>();
            Array.Reverse(args);
            foreach (object arg in args)
            {
                if (arg is int) ret.Add(Push((uint)(int)arg));
                else if (arg is uint) ret.Add(Push((uint)arg));
                else if (arg is Ref<uint>) ret.Add(Push((Ref<uint>)arg));
                else if (arg is Addr32) ret.Add(Push((Addr32)arg));
            }
            ret.Add(Call(func));
            if (call == CallType.CDecl)
            {
                ret.Add(Add(Reg32.ESP, (byte)(args.Length * 4)));
            }
            return ret.ToArray();
        }

        public static OpCode Mov(Reg32 op1, Addr32 op2)
        {
            if (op1 == Reg32.EAX && op2.IsAddress)
                return new OpCode(new byte[] { 0xa1 }, op2.Address);

            op2.MiddleBits = (byte)op1;
            return new OpCode(new byte[] { 0x8b }, null, op2);
        }

        public static OpCode Mov(Reg32 op1, Ref<uint> op2)
        {
            return new OpCode(new byte[] { (byte)(0xb8 + (int)op1) }, op2);
        }

        public static OpCode Mov(Addr32 op1, Ref<uint> op2)
        {
            return new OpCode(new byte[] { 0xc7 }, op2, op1);
        }

        public static OpCode Mov(Addr32 op1, Reg32 op2)
        {
            if (op2 == Reg32.EAX && op1.IsAddress)
                return new OpCode(new byte[] { 0xa3 }, op1.Address);

            op1.MiddleBits = (byte)op2;
            return new OpCode(new byte[] { 0x89 }, null, op1);
        }

        public static OpCode Add(Reg32 op1, Reg32 op2)
        {
            return new OpCode(new byte[] { 0x01, (byte)(0xc0 + (((int)op2) << 3) + (int)op1) });
        }

        public static OpCode Add(Reg32 op1, Ref<uint> op2)
        {
            return new OpCode(new byte[] { 0x81, (byte)(0xc0 + (int)op1) }, op2);
        }

        public static OpCode Add(Addr32 op1, Reg32 op2)
        {
            op1.MiddleBits = (byte)op2;
            return new OpCode(new byte[] { 0x01 }, null, op1);
        }

        public static OpCode Inc(Reg32 op1)
        {
            return new OpCode(new byte[] { (byte)(0x40 + (int)op1) });
        }

        public static OpCode Dec(Reg32 op1)
        {
            return new OpCode(new byte[] { (byte)(0x48 + (int)op1) });
        }

        public static OpCode Dec(Addr32 op1)
        {
            op1.MiddleBits = 1;
            return new OpCode(new byte[] { 0xff }, null, op1);
        }

        public static OpCode Test(Addr32 op1, Ref<uint> op2)
        {
            return new OpCode(new byte[] { 0xf7 }, op2, op1);
        }

        public static OpCode Jmp(Ref<uint> op1)
        {
            return new OpCode(new byte[] { 0xe9 }, op1, true);
        }

        public static OpCode Jnz(Ref<uint> op1)
        {
            return new OpCode(new byte[] { 0x0f, 0x85 }, op1, true);
        }
    }
}
