using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Call
    {
        public bool AddIntrinsicCodes(OpCodes codes)
        {
            var args = new List<IIntValue>();
            if (target != null) args.Add(target);
            args.AddRange(this.args);

            switch (name)
            {
                case "__stosb":
                case "__stosw":
                case "__stosd":
                    if (args.Count != 3)
                        throw Abort("{0}: argument mismatched", name);
                    Stos(codes, name.Substring(2), args[0], args[1], args[2]);
                    return true;
                case "__movsb":
                case "__movsw":
                case "__movsd":
                    if (args.Count != 3)
                        throw Abort("{0}: argument mismatched", name);
                    Movs(codes, name.Substring(2), args[0], args[1], args[2]);
                    return true;
                case "__memcpy":
                    if (args.Count != 3)
                        throw Abort("{0}: argument mismatched", name);
                    Memcpy(codes, args[0], args[1], args[2]);
                    return true;
                case "__cpuid":
                    if (args.Count != 2)
                        throw Abort("{0}: argument mismatched", name);
                    Cpuid(codes, args[0], args[1]);
                    return true;
            }
            return false;
        }

        public static void Stos(OpCodes codes, string op, IIntValue dest, IIntValue data, IIntValue count)
        {
            codes.Add(I386.Push(Reg32.EDI));
            dest.AddCodes(codes, "push", null);
            count.AddCodes(codes, "push", null);
            data.AddCodes(codes, "mov", null);
            codes.AddRange(new OpCode[]
            {
                I386.Pop(Reg32.ECX),
                I386.Pop(Reg32.EDI),
                I386.Cld(),
                I386.Rep(),
                I386.FromName(op),
                I386.Pop(Reg32.EDI)
            });
        }

        public static void Movs(OpCodes codes, string op, IIntValue dest, IIntValue src, IIntValue count)
        {
            codes.AddRange(new OpCode[]
            {
                I386.Push(Reg32.EDI),
                I386.Push(Reg32.ESI)
            });
            dest.AddCodes(codes, "push", null);
            src.AddCodes(codes, "push", null);
            count.AddCodes(codes, "mov", null);
            codes.AddRange(new OpCode[]
            {
                I386.Pop(Reg32.ESI),
                I386.Pop(Reg32.EDI),
                I386.Mov(Reg32.ECX, Reg32.EAX),
                I386.Cld(),
                I386.Rep(),
                I386.FromName(op),
                I386.Pop(Reg32.ESI),
                I386.Pop(Reg32.EDI)
            });
        }

        public static void Memcpy(OpCodes codes, IIntValue dest, IIntValue src, IIntValue count)
        {
            if (count is IntValue)
            {
                Memcpy(codes, dest, src, (count as IntValue).Value);
                return;
            }

            codes.AddRange(new OpCode[]
            {
                I386.Push(Reg32.EDI),
                I386.Push(Reg32.ESI)
            });
            dest.AddCodes(codes, "push", null);
            src.AddCodes(codes, "push", null);
            count.AddCodes(codes, "mov", null);
            codes.AddRange(new OpCode[]
            {
                I386.Pop(Reg32.ESI),
                I386.Pop(Reg32.EDI),
                I386.Mov(Reg32.ECX, Reg32.EAX),
                I386.Shr(Reg32.ECX, 2),
                I386.Cld(),
                I386.Rep(),
                I386.Movsd(),
                I386.Mov(Reg32.ECX, Reg32.EAX),
                I386.And(Reg32.ECX, 3),
                I386.Rep(),
                I386.Movsb(),
                I386.Pop(Reg32.ESI),
                I386.Pop(Reg32.EDI)
            });
        }

        public static void Memcpy(OpCodes codes, IIntValue dest, IIntValue src, int count)
        {
            switch (count)
            {
                case 0:
                    break;
                case 1:
                    dest.AddCodes(codes, "push", null);
                    src.AddCodes(codes, "mov", null);
                    codes.AddRange(new OpCode[]
                    {
                        I386.Pop(Reg32.EDX),
                        I386.MovB(Reg8.AL, new Addr32(Reg32.EAX)),
                        I386.MovB(new Addr32(Reg32.EDX), Reg8.AL)
                    });
                    break;
                case 2:
                    dest.AddCodes(codes, "push", null);
                    src.AddCodes(codes, "mov", null);
                    codes.AddRange(new OpCode[]
                    {
                        I386.Pop(Reg32.EDX),
                        I386.MovW(Reg16.AX, new Addr32(Reg32.EAX)),
                        I386.MovW(new Addr32(Reg32.EDX), Reg16.AX)
                    });
                    break;
                case 3:
                    dest.AddCodes(codes, "push", null);
                    src.AddCodes(codes, "mov", null);
                    codes.AddRange(new OpCode[]
                    {
                        I386.Pop(Reg32.EDX),
                        I386.MovW(Reg16.CX, new Addr32(Reg32.EAX)),
                        I386.MovB(Reg8.AL, new Addr32(Reg32.EAX, 2)),
                        I386.MovW(new Addr32(Reg32.EDX), Reg16.CX),
                        I386.MovB(new Addr32(Reg32.EDX, 2), Reg8.AL)
                    });
                    break;
                case 4:
                    dest.AddCodes(codes, "push", null);
                    src.AddCodes(codes, "mov", null);
                    codes.AddRange(new OpCode[]
                    {
                        I386.Pop(Reg32.EDX),
                        I386.Mov(Reg32.EAX, new Addr32(Reg32.EAX)),
                        I386.Mov(new Addr32(Reg32.EDX), Reg32.EAX)
                    });
                    break;
                case 8:
                    dest.AddCodes(codes, "push", null);
                    src.AddCodes(codes, "mov", null);
                    codes.AddRange(new OpCode[]
                    {
                        I386.Pop(Reg32.EDX),
                        I386.Mov(Reg32.ECX, new Addr32(Reg32.EAX)),
                        I386.Mov(Reg32.EAX, new Addr32(Reg32.EAX, 4)),
                        I386.Mov(new Addr32(Reg32.EDX), Reg32.ECX),
                        I386.Mov(new Addr32(Reg32.EDX, 4), Reg32.EAX)
                    });
                    break;
                case 12:
                case 16:
                    codes.AddRange(new OpCode[]
                    {
                        I386.Push(Reg32.EDI),
                        I386.Push(Reg32.ESI)
                    });
                    dest.AddCodes(codes, "push", null);
                    src.AddCodes(codes, "mov", null);
                    codes.AddRange(new OpCode[]
                    {
                        I386.Pop(Reg32.EDI),
                        I386.Mov(Reg32.ESI, Reg32.EAX),
                        I386.Cld()
                    });
                    for (int i = 0; i < count; i += 4)
                        codes.Add(I386.Movsd());
                    codes.AddRange(new OpCode[]
                    {
                        I386.Pop(Reg32.ESI),
                        I386.Pop(Reg32.EDI)
                    });
                    break;
                default:
                    codes.AddRange(new OpCode[]
                    {
                        I386.Push(Reg32.EDI),
                        I386.Push(Reg32.ESI)
                    });
                    dest.AddCodes(codes, "push", null);
                    src.AddCodes(codes, "mov", null);
                    codes.AddRange(new OpCode[]
                    {
                        I386.Pop(Reg32.EDI),
                        I386.Mov(Reg32.ESI, Reg32.EAX),
                        I386.Mov(Reg32.ECX, (uint)(count >> 2)),
                        I386.Cld(),
                        I386.Rep(),
                        I386.Movsd()
                    });
                    if ((count & 3) > 0)
                        codes.AddRange(new OpCode[]
                        {
                            I386.Mov(Reg32.ECX, (uint)(count & 3)),
                            I386.Rep(),
                            I386.Movsb()
                        });
                    codes.AddRange(new OpCode[]
                    {
                        I386.Pop(Reg32.ESI),
                        I386.Pop(Reg32.EDI)
                    });
                    break;
            }
        }

        public static void Cpuid(OpCodes codes, IIntValue result, IIntValue eax)
        {
            codes.AddRange(new OpCode[]
            {
                I386.Push(Reg32.EDI),
                I386.Push(Reg32.EBX)
            });
            result.AddCodes(codes, "push", null);
            eax.AddCodes(codes, "mov", null);
            codes.AddRange(new OpCode[]
            {
                I386.Pop(Reg32.EDI),
                I586.Cpuid(),
                I386.Cld(),
                I386.Stosd(),
                I386.Mov(Reg32.EAX, Reg32.EBX),
                I386.Stosd(),
                I386.Mov(Reg32.EAX, Reg32.ECX),
                I386.Stosd(),
                I386.Mov(Reg32.EAX, Reg32.EDX),
                I386.Stosd(),
                I386.Pop(Reg32.EBX),
                I386.Pop(Reg32.EDI)
            });
        }
    }
}
