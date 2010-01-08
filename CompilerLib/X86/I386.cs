using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        #region Mov, Add, Or, Adc, Sbb, And, Sub, Xor, Cmp, Test, Xchg

        public static OpCode Mov(Reg32 op1, Reg32 op2) { return FromName("mov", op1, op2); }
        public static OpCode Mov(Reg32 op1, Val32 op2) { return FromName("mov", op1, op2); }
        public static OpCode Mov(Reg32 op1, Addr32 op2) { return FromName("mov", op1, op2); }
        public static OpCode Mov(Addr32 op1, Reg32 op2) { return FromName("mov", op1, op2); }
        public static OpCode Mov(Addr32 op1, Val32 op2) { return FromName("mov", op1, op2); }

        public static OpCode Add(Reg32 op1, Reg32 op2) { return FromName("add", op1, op2); }
        public static OpCode Add(Reg32 op1, Val32 op2) { return FromName("add", op1, op2); }
        public static OpCode Add(Reg32 op1, Addr32 op2) { return FromName("add", op1, op2); }
        public static OpCode Add(Addr32 op1, Reg32 op2) { return FromName("add", op1, op2); }
        public static OpCode Add(Addr32 op1, Val32 op2) { return FromName("add", op1, op2); }

        public static OpCode Or(Reg32 op1, Reg32 op2) { return FromName("or", op1, op2); }
        public static OpCode Or(Reg32 op1, Val32 op2) { return FromName("or", op1, op2); }
        public static OpCode Or(Reg32 op1, Addr32 op2) { return FromName("or", op1, op2); }
        public static OpCode Or(Addr32 op1, Reg32 op2) { return FromName("or", op1, op2); }
        public static OpCode Or(Addr32 op1, Val32 op2) { return FromName("or", op1, op2); }

        public static OpCode Adc(Reg32 op1, Reg32 op2) { return FromName("adc", op1, op2); }
        public static OpCode Adc(Reg32 op1, Val32 op2) { return FromName("adc", op1, op2); }
        public static OpCode Adc(Reg32 op1, Addr32 op2) { return FromName("adc", op1, op2); }
        public static OpCode Adc(Addr32 op1, Reg32 op2) { return FromName("adc", op1, op2); }
        public static OpCode Adc(Addr32 op1, Val32 op2) { return FromName("adc", op1, op2); }

        public static OpCode Sbb(Reg32 op1, Reg32 op2) { return FromName("sbb", op1, op2); }
        public static OpCode Sbb(Reg32 op1, Val32 op2) { return FromName("sbb", op1, op2); }
        public static OpCode Sbb(Reg32 op1, Addr32 op2) { return FromName("sbb", op1, op2); }
        public static OpCode Sbb(Addr32 op1, Reg32 op2) { return FromName("sbb", op1, op2); }
        public static OpCode Sbb(Addr32 op1, Val32 op2) { return FromName("sbb", op1, op2); }

        public static OpCode And(Reg32 op1, Reg32 op2) { return FromName("and", op1, op2); }
        public static OpCode And(Reg32 op1, Val32 op2) { return FromName("and", op1, op2); }
        public static OpCode And(Reg32 op1, Addr32 op2) { return FromName("and", op1, op2); }
        public static OpCode And(Addr32 op1, Reg32 op2) { return FromName("and", op1, op2); }
        public static OpCode And(Addr32 op1, Val32 op2) { return FromName("and", op1, op2); }

        public static OpCode Sub(Reg32 op1, Reg32 op2) { return FromName("sub", op1, op2); }
        public static OpCode Sub(Reg32 op1, Val32 op2) { return FromName("sub", op1, op2); }
        public static OpCode Sub(Reg32 op1, Addr32 op2) { return FromName("sub", op1, op2); }
        public static OpCode Sub(Addr32 op1, Reg32 op2) { return FromName("sub", op1, op2); }
        public static OpCode Sub(Addr32 op1, Val32 op2) { return FromName("sub", op1, op2); }

        public static OpCode Xor(Reg32 op1, Reg32 op2) { return FromName("xor", op1, op2); }
        public static OpCode Xor(Reg32 op1, Val32 op2) { return FromName("xor", op1, op2); }
        public static OpCode Xor(Reg32 op1, Addr32 op2) { return FromName("xor", op1, op2); }
        public static OpCode Xor(Addr32 op1, Reg32 op2) { return FromName("xor", op1, op2); }
        public static OpCode Xor(Addr32 op1, Val32 op2) { return FromName("xor", op1, op2); }

        public static OpCode Cmp(Reg32 op1, Reg32 op2) { return FromName("cmp", op1, op2); }
        public static OpCode Cmp(Reg32 op1, Val32 op2) { return FromName("cmp", op1, op2); }
        public static OpCode Cmp(Reg32 op1, Addr32 op2) { return FromName("cmp", op1, op2); }
        public static OpCode Cmp(Addr32 op1, Reg32 op2) { return FromName("cmp", op1, op2); }
        public static OpCode Cmp(Addr32 op1, Val32 op2) { return FromName("cmp", op1, op2); }

        public static OpCode Test(Reg32 op1, Reg32 op2) { return FromName("test", op1, op2); }
        public static OpCode Test(Reg32 op1, Val32 op2) { return FromName("test", op1, op2); }
        public static OpCode Test(Reg32 op1, Addr32 op2) { return Test(op2, op1); }
        public static OpCode Test(Addr32 op1, Reg32 op2) { return FromName("test", op1, op2); }
        public static OpCode Test(Addr32 op1, Val32 op2) { return FromName("test", op1, op2); }

        public static OpCode Xchg(Reg32 op1, Reg32 op2) { return FromName("xchg", op1, op2); }
        public static OpCode Xchg(Reg32 op1, Addr32 op2) { return FromName("xchg", op1, op2); }
        public static OpCode Xchg(Addr32 op1, Reg32 op2) { return Xchg(op2, op1); }

        #region Implementations

        public static int GetOperatorCode(string op)
        {
            string[] s = { "add", "or", "adc", "sbb", "and", "sub", "xor", "cmp" };
            return Array.IndexOf(s, op);
        }

        public static OpCode FromName(string op, Reg32 op1, Reg32 op2)
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
                        return new OpCode(new byte[] { (byte)(0x90 + op2) });
                    else if (op2 == Reg32.EAX)
                        return new OpCode(new byte[] { (byte)(0x90 + op1) });
                    else
                        b = 0x87;
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    b = (byte)(code * 8 + 1);
                    break;
            }
            return new OpCode(new byte[] { b, (byte)(0xc0 + (((int)op2) << 3) + op1) });
        }

        public static OpCode FromName(string op, Reg32 op1, Val32 op2)
        {
            byte[] bytes;
            switch (op)
            {
                case "mov":
                    bytes = new byte[] { (byte)(0xb8 + op1) };
                    break;
                case "test":
                    if (op1 == Reg32.EAX)
                        bytes = new byte[] { 0xa9 };
                    else
                        bytes = new byte[] { 0xf7, (byte)(0xc0 + op1) };
                    break;
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    if (op1 == Reg32.EAX)
                        bytes = new byte[] { (byte)(code * 8 + 5) };
                    else
                        bytes = new byte[] { 0x81, (byte)(code * 8 + 0xc0 + op1) };
                    break;
            }
            return new OpCode(bytes, op2);
        }

        public static OpCode FromName(string op, Reg32 op1, Addr32 op2)
        {
            byte b;
            switch (op)
            {
                case "mov":
                    if (op1 == Reg32.EAX && op2.IsAddress)
                        return new OpCode(new byte[] { 0xa1 }, op2.Address);
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
            return new OpCode(new byte[] { b }, null, new Addr32(op2, (byte)op1));
        }

        public static OpCode FromName(string op, Addr32 op1, Reg32 op2)
        {
            byte b;
            switch (op)
            {
                case "mov":
                    if (op2 == Reg32.EAX && op1.IsAddress)
                        return new OpCode(new byte[] { 0xa3 }, op1.Address);
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
            return new OpCode(new byte[] { b }, null, new Addr32(op1, (byte)op2));
        }

        public static OpCode FromName(string op, Addr32 op1, Val32 op2)
        {
            switch (op)
            {
                case "mov":
                    return new OpCode(new byte[] { 0xc7 }, op2, op1);
                case "test":
                    return new OpCode(new byte[] { 0xf7 }, op2, op1);
                default:
                    int code = GetOperatorCode(op);
                    if (code < 0) throw new Exception("invalid operator: " + op);
                    return new OpCode(new byte[] { 0x81 }, op2, new Addr32(op1, (byte)code));
            }
        }

        #endregion

        #endregion

        #region Push, Pop, Inc, Dec, Not, Neg, Mul, Imul, Div, Idiv

        public static OpCode Push(Val32 op1)
        {
            return new OpCode(new byte[] { 0x68 }, op1);
        }
        public static OpCode Push(Reg32 op1) { return FromName("push", op1); }
        public static OpCode Push(Addr32 op1) { return FromName("push", op1); }

        public static OpCode Pop(Reg32 op1) { return FromName("pop", op1); }
        public static OpCode Pop(Addr32 op1) { return FromName("pop", op1); }

        public static OpCode Inc(Reg32 op1) { return FromName("inc", op1); }
        public static OpCode Inc(Addr32 op1) { return FromName("inc", op1); }

        public static OpCode Dec(Reg32 op1) { return FromName("dec", op1); }
        public static OpCode Dec(Addr32 op1) { return FromName("dec", op1); }

        public static OpCode Not(Reg32 op1) { return FromName("not", op1); }
        public static OpCode Not(Addr32 op1) { return FromName("not", op1); }

        public static OpCode Neg(Reg32 op1) { return FromName("neg", op1); }
        public static OpCode Neg(Addr32 op1) { return FromName("neg", op1); }

        public static OpCode Mul(Reg32 op1) { return FromName("mul", op1); }
        public static OpCode Mul(Addr32 op1) { return FromName("mul", op1); }

        public static OpCode Imul(Reg32 op1) { return FromName("imul", op1); }
        public static OpCode Imul(Addr32 op1) { return FromName("imul", op1); }

        public static OpCode Div(Reg32 op1) { return FromName("div", op1); }
        public static OpCode Div(Addr32 op1) { return FromName("div", op1); }

        public static OpCode Idiv(Reg32 op1) { return FromName("idiv", op1); }
        public static OpCode Idiv(Addr32 op1) { return FromName("idiv", op1); }

        #region Implementations

        public static bool IsOneOperand(string op)
        {
            string[] s = { "push", "pop", "inc", "dec", "not", "neg", "mul", "imul", "div", "idiv" };
            return Array.IndexOf(s, op) >= 0;
        }

        public static OpCode FromName(string op, Reg32 op1)
        {
            switch (op)
            {
                case "push":
                    return new OpCode(new byte[] { (byte)(0x50 + op1) });
                case "pop":
                    return new OpCode(new byte[] { (byte)(0x58 + op1) });
                case "inc":
                    return new OpCode(new byte[] { (byte)(0x40 + op1) });
                case "dec":
                    return new OpCode(new byte[] { (byte)(0x48 + op1) });
                case "not":
                    return new OpCode(new byte[] { 0xf7, (byte)(0xd0 + op1) });
                case "neg":
                    return new OpCode(new byte[] { 0xf7, (byte)(0xd8 + op1) });
                case "mul":
                    return new OpCode(new byte[] { 0xf7, (byte)(0xe0 + op1) });
                case "imul":
                    return new OpCode(new byte[] { 0xf7, (byte)(0xe8 + op1) });
                case "div":
                    return new OpCode(new byte[] { 0xf7, (byte)(0xf0 + op1) });
                case "idiv":
                    return new OpCode(new byte[] { 0xf7, (byte)(0xf8 + op1) });
                default:
                    throw new Exception("invalid operator: " + op);
            }
        }

        public static OpCode FromName(string op, Addr32 op1)
        {
            switch (op)
            {
                case "push":
                    return new OpCode(new byte[] { 0xff }, null, new Addr32(op1, 6));
                case "pop":
                    return new OpCode(new byte[] { 0x8f }, null, op1);
                case "inc":
                    return new OpCode(new byte[] { 0xff }, null, op1);
                case "dec":
                    return new OpCode(new byte[] { 0xff }, null, new Addr32(op1, 1));
                case "not":
                    return new OpCode(new byte[] { 0xf7 }, null, new Addr32(op1, 2));
                case "neg":
                    return new OpCode(new byte[] { 0xf7 }, null, new Addr32(op1, 3));
                case "mul":
                    return new OpCode(new byte[] { 0xf7 }, null, new Addr32(op1, 4));
                case "imul":
                    return new OpCode(new byte[] { 0xf7 }, null, new Addr32(op1, 5));
                case "div":
                    return new OpCode(new byte[] { 0xf7 }, null, new Addr32(op1, 6));
                case "idiv":
                    return new OpCode(new byte[] { 0xf7 }, null, new Addr32(op1, 7));
                default:
                    throw new Exception("invalid operator: " + op);
            }
        }

        #endregion

        #endregion

        #region Lea

        public static OpCode Lea(Reg32 op1, Addr32 op2)
        {
            return new OpCode(new byte[] { 0x8d }, null, new Addr32(op2, (byte)op1));
        }

        #endregion

        #region Jmp

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

        #endregion

        #region Call

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
            return new OpCode(new byte[] { 0xff }, null, new Addr32(op1, 2));
        }

        public static OpCode Call(Val32 op1)
        {
            return new OpCode(new byte[] { 0xe8 }, op1, true);
        }

        public static OpCode[] Call(CallType call, Addr32 func, object[] args)
        {
            List<OpCode> ret = new List<OpCode>();
            args = args.Clone() as object[];
            Array.Reverse(args);
            foreach (object arg in args)
            {
                if (arg is int) ret.Add(Push((uint)(int)arg));
                else if (arg is uint) ret.Add(Push((uint)arg));
                else if (arg is Val32) ret.Add(Push((Val32)arg));
                else if (arg is Addr32) ret.Add(Push((Addr32)arg));
                else throw new Exception("Unknown argument.");
            }
            ret.Add(Call(func));
            if (call == CallType.CDecl)
            {
                ret.Add(Add(Reg32.ESP, (byte)(args.Length * 4)));
            }
            return ret.ToArray();
        }

        #endregion

        #region Ret

        public static OpCode Ret()
        {
            return new OpCode(new byte[] { 0xc3 });
        }

        public static OpCode Ret(ushort op1)
        {
            return new OpCode(new byte[] { 0xc2 }, op1);
        }

        #endregion

        #region Shl, Shr, Sal, Sar

        public static OpCode Shl(Reg32 op1, byte op2) { return Shift("shl", op1, op2); }
        public static OpCode Shl(Reg32 op1, Reg8 op2) { return Shift("shl", op1, op2); }
        public static OpCode Shl(Addr32 op1, byte op2) { return Shift("shl", op1, op2); }
        public static OpCode Shl(Addr32 op1, Reg8 op2) { return Shift("shl", op1, op2); }

        public static OpCode Shr(Reg32 op1, byte op2) { return Shift("shr", op1, op2); }
        public static OpCode Shr(Reg32 op1, Reg8 op2) { return Shift("shr", op1, op2); }
        public static OpCode Shr(Addr32 op1, byte op2) { return Shift("shr", op1, op2); }
        public static OpCode Shr(Addr32 op1, Reg8 op2) { return Shift("shr", op1, op2); }

        public static OpCode Sal(Reg32 op1, byte op2) { return Shift("sal", op1, op2); }
        public static OpCode Sal(Reg32 op1, Reg8 op2) { return Shift("sal", op1, op2); }
        public static OpCode Sal(Addr32 op1, byte op2) { return Shift("sal", op1, op2); }
        public static OpCode Sal(Addr32 op1, Reg8 op2) { return Shift("sal", op1, op2); }

        public static OpCode Sar(Reg32 op1, byte op2) { return Shift("sar", op1, op2); }
        public static OpCode Sar(Reg32 op1, Reg8 op2) { return Shift("sar", op1, op2); }
        public static OpCode Sar(Addr32 op1, byte op2) { return Shift("sar", op1, op2); }
        public static OpCode Sar(Addr32 op1, Reg8 op2) { return Shift("sar", op1, op2); }

        #region Implementation

        public static OpCode Shift(string op, Reg32 op1, byte op2)
        {
            byte b;
            switch (op)
            {
                case "shl":
                case "sal":
                    b = (byte)(0xe0 + op1);
                    break;
                case "shr":
                    b = (byte)(0xe8 + op1);
                    break;
                case "sar":
                    b = (byte)(0xf8 + op1);
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            if (op2 == 1)
                return new OpCode(new byte[] { 0xd1, b });
            else
                return new OpCode(new byte[] { 0xc1, b }, op2);
        }

        public static OpCode Shift(string op, Reg32 op1, Reg8 op2)
        {
            byte b;
            switch (op)
            {
                case "shl":
                case "sal":
                    b = (byte)(0xe0 + op1);
                    break;
                case "shr":
                    b = (byte)(0xe8 + op1);
                    break;
                case "sar":
                    b = (byte)(0xf8 + op1);
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            if (op2 != Reg8.CL)
                throw new Exception("invalid register: " + op2);
            else
                return new OpCode(new byte[] { 0xd3, b });
        }

        public static OpCode Shift(string op, Addr32 op1, byte op2)
        {
            Addr32 ad;
            switch (op)
            {
                case "shl":
                case "sal":
                    ad = new Addr32(op1, 4);
                    break;
                case "shr":
                    ad = new Addr32(op1, 5);
                    break;
                case "sar":
                    ad = new Addr32(op1, 7);
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            if (op2 == 1)
                return new OpCode(new byte[] { 0xd1 }, null, ad);
            else
                return new OpCode(new byte[] { 0xc1 }, op2, ad);
        }

        public static OpCode Shift(string op, Addr32 op1, Reg8 op2)
        {
            Addr32 ad;
            switch (op)
            {
                case "shl":
                case "sal":
                    ad = new Addr32(op1, 4);
                    break;
                case "shr":
                    ad = new Addr32(op1, 5);
                    break;
                case "sar":
                    ad = new Addr32(op1, 7);
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            if (op2 != Reg8.CL)
                throw new Exception("invalid register: " + op2);
            else
                return new OpCode(new byte[] { 0xd3 }, null, ad);
        }

        #endregion

        #endregion

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
