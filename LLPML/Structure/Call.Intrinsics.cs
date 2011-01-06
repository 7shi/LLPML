using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Call
    {
        public bool AddIntrinsicCodes(OpModule codes, ArrayList args)
        {
            switch (name)
            {
                case "__stosb":
                case "__stosw":
                case "__stosd":
                    if (codes != null)
                    {
                        if (args.Count != 3)
                            throw Abort("{0}: argument mismatched", name);
                        Stos(codes, name.Substring(2),
                            args[0] as NodeBase, args[1] as NodeBase, args[2] as NodeBase);
                    }
                    return true;
                case "__movsb":
                case "__movsw":
                case "__movsd":
                case "__movsb_rev":
                case "__movsw_rev":
                case "__movsd_rev":
                    if (codes != null)
                    {
                        if (args.Count != 3)
                            throw Abort("{0}: argument mismatched", name);
                        Movs(codes, name.Substring(2),
                            args[0] as NodeBase, args[1] as NodeBase, args[2] as NodeBase);
                    }
                    return true;
                case "__memcpy":
                    if (codes != null)
                    {
                        if (args.Count != 3)
                            throw Abort("{0}: argument mismatched", name);
                        Memcpy(codes,
                            args[0] as NodeBase, args[1] as NodeBase, args[2] as NodeBase);
                    }
                    return true;
                case "__memcpy_rev":
                    if (codes != null)
                    {
                        if (args.Count != 3)
                            throw Abort("{0}: argument mismatched", name);
                        MemcpyRev(codes,
                            args[0] as NodeBase, args[1] as NodeBase, args[2] as NodeBase);
                    }
                    return true;
                case "__cpuid":
                    if (codes != null)
                    {
                        if (args.Count != 2)
                            throw Abort("{0}: argument mismatched", name);
                        Cpuid(codes, args[0] as NodeBase, args[1] as NodeBase);
                    }
                    return true;
            }
            return false;
        }

        public static void Stos(OpModule codes, string op, NodeBase dest, NodeBase data, NodeBase count)
        {
            codes.Add(I386.Pushf());
            codes.Add(I386.Push(Reg32.EDI));
            dest.AddCodesV(codes, "push", null);
            count.AddCodesV(codes, "push", null);
            data.AddCodesV(codes, "mov", null);
            codes.Add(I386.Pop(Reg32.ECX));
            codes.Add(I386.Pop(Reg32.EDI));
            codes.Add(I386.Cld());
            codes.Add(I386.Rep());
            codes.Add(I386.FromName(op));
            codes.Add(I386.Pop(Reg32.EDI));
            codes.Add(I386.Popf());
        }

        public static void Movs(OpModule codes, string op, NodeBase dest, NodeBase src, NodeBase count)
        {
            var direction = I386.Cld();
            if (op.EndsWith("_rev"))
            {
                op = op.Substring(0, op.Length - 4);
                direction = I386.Std();
            }
            codes.Add(I386.Pushf());
            codes.Add(I386.Push(Reg32.EDI));
            codes.Add(I386.Push(Reg32.ESI));
            dest.AddCodesV(codes, "push", null);
            src.AddCodesV(codes, "push", null);
            count.AddCodesV(codes, "mov", null);
            codes.Add(I386.Pop(Reg32.ESI));
            codes.Add(I386.Pop(Reg32.EDI));
            codes.Add(I386.Mov(Reg32.ECX, Reg32.EAX));
            codes.Add(direction);
            codes.Add(I386.Rep());
            codes.Add(I386.FromName(op));
            codes.Add(I386.Pop(Reg32.ESI));
            codes.Add(I386.Pop(Reg32.EDI));
            codes.Add(I386.Popf());
        }

        public static void Memcpy(OpModule codes, NodeBase dest, NodeBase src, NodeBase count)
        {
            if (count is IntValue)
            {
                MemcpyInt(codes, dest, src, (count as IntValue).Value);
                return;
            }

            codes.Add(I386.Pushf());
            codes.Add(I386.Push(Reg32.EDI));
            codes.Add(I386.Push(Reg32.ESI));
            dest.AddCodesV(codes, "push", null);
            src.AddCodesV(codes, "push", null);
            count.AddCodesV(codes, "mov", null);
            codes.Add(I386.Pop(Reg32.ESI));
            codes.Add(I386.Pop(Reg32.EDI));
            codes.Add(I386.Mov(Reg32.ECX, Reg32.EAX));
            codes.Add(I386.Shr(Reg32.ECX, 2));
            codes.Add(I386.Cld());
            codes.Add(I386.Rep());
            codes.Add(I386.Movsd());
            codes.Add(I386.Mov(Reg32.ECX, Reg32.EAX));
            codes.Add(I386.AndR(Reg32.ECX, Val32.New(3)));
            codes.Add(I386.Rep());
            codes.Add(I386.Movsb());
            codes.Add(I386.Pop(Reg32.ESI));
            codes.Add(I386.Pop(Reg32.EDI));
            codes.Add(I386.Popf());
        }

        public static void MemcpyInt(OpModule codes, NodeBase dest, NodeBase src, int count)
        {
            switch (count)
            {
                case 0:
                    break;
                case 1:
                    dest.AddCodesV(codes, "push", null);
                    src.AddCodesV(codes, "mov", null);
                    codes.Add(I386.Pop(Reg32.EDX));
                    codes.Add(I386.MovBRA(Reg8.AL, Addr32.New(Reg32.EAX)));
                    codes.Add(I386.MovBAR(Addr32.New(Reg32.EDX), Reg8.AL));
                    break;
                case 2:
                    dest.AddCodesV(codes, "push", null);
                    src.AddCodesV(codes, "mov", null);
                    codes.Add(I386.Pop(Reg32.EDX));
                    codes.Add(I386.MovWRA(Reg16.AX, Addr32.New(Reg32.EAX)));
                    codes.Add(I386.MovWAR(Addr32.New(Reg32.EDX), Reg16.AX));
                    break;
                case 3:
                    dest.AddCodesV(codes, "push", null);
                    src.AddCodesV(codes, "mov", null);
                    codes.Add(I386.Pop(Reg32.EDX));
                    codes.Add(I386.MovWRA(Reg16.CX, Addr32.New(Reg32.EAX)));
                    codes.Add(I386.MovBRA(Reg8.AL, Addr32.NewRO(Reg32.EAX, 2)));
                    codes.Add(I386.MovWAR(Addr32.New(Reg32.EDX), Reg16.CX));
                    codes.Add(I386.MovBAR(Addr32.NewRO(Reg32.EDX, 2), Reg8.AL));
                    break;
                case 4:
                    dest.AddCodesV(codes, "push", null);
                    src.AddCodesV(codes, "mov", null);
                    codes.Add(I386.Pop(Reg32.EDX));
                    codes.Add(I386.MovRA(Reg32.EAX, Addr32.New(Reg32.EAX)));
                    codes.Add(I386.MovAR(Addr32.New(Reg32.EDX), Reg32.EAX));
                    break;
                case 8:
                    dest.AddCodesV(codes, "push", null);
                    src.AddCodesV(codes, "mov", null);
                    codes.Add(I386.Pop(Reg32.EDX));
                    codes.Add(I386.MovRA(Reg32.ECX, Addr32.New(Reg32.EAX)));
                    codes.Add(I386.MovRA(Reg32.EAX, Addr32.NewRO(Reg32.EAX, 4)));
                    codes.Add(I386.MovAR(Addr32.New(Reg32.EDX), Reg32.ECX));
                    codes.Add(I386.MovAR(Addr32.NewRO(Reg32.EDX, 4), Reg32.EAX));
                    break;
                case 12:
                case 16:
                    codes.Add(I386.Pushf());
                    codes.Add(I386.Push(Reg32.EDI));
                    codes.Add(I386.Push(Reg32.ESI));
                    dest.AddCodesV(codes, "push", null);
                    src.AddCodesV(codes, "mov", null);
                    codes.Add(I386.Pop(Reg32.EDI));
                    codes.Add(I386.Mov(Reg32.ESI, Reg32.EAX));
                    codes.Add(I386.Cld());
                    for (int i = 0; i < count; i += 4)
                        codes.Add(I386.Movsd());
                    codes.Add(I386.Pop(Reg32.ESI));
                    codes.Add(I386.Pop(Reg32.EDI));
                    codes.Add(I386.Popf());
                    break;
                default:
                    codes.Add(I386.Pushf());
                    codes.Add(I386.Push(Reg32.EDI));
                    codes.Add(I386.Push(Reg32.ESI));
                    dest.AddCodesV(codes, "push", null);
                    src.AddCodesV(codes, "mov", null);
                    codes.Add(I386.Pop(Reg32.EDI));
                    codes.Add(I386.Mov(Reg32.ESI, Reg32.EAX));
                    codes.Add(I386.MovR(Reg32.ECX, Val32.NewI((count >> 2))));
                    codes.Add(I386.Cld());
                    codes.Add(I386.Rep());
                    codes.Add(I386.Movsd());
                    if ((count & 3) > 0)
                    {
                        codes.Add(I386.MovR(Reg32.ECX, Val32.NewI((count & 3))));
                        codes.Add(I386.Rep());
                        codes.Add(I386.Movsb());
                    }
                    codes.Add(I386.Pop(Reg32.ESI));
                    codes.Add(I386.Pop(Reg32.EDI));
                    codes.Add(I386.Popf());
                    break;
            }
        }

        public static void MemcpyRev(OpModule codes, NodeBase dest, NodeBase src, NodeBase count)
        {
            codes.Add(I386.Pushf());
            codes.Add(I386.Push(Reg32.EDI));
            codes.Add(I386.Push(Reg32.ESI));
            dest.AddCodesV(codes, "push", null);
            src.AddCodesV(codes, "push", null);
            count.AddCodesV(codes, "mov", null);
            codes.Add(I386.Pop(Reg32.ESI));
            codes.Add(I386.Pop(Reg32.EDI));
            codes.Add(I386.Mov(Reg32.ECX, Reg32.EAX));
            codes.Add(I386.AndR(Reg32.ECX, Val32.New(3)));
            codes.Add(I386.Std());
            codes.Add(I386.Rep());
            codes.Add(I386.Movsb());
            codes.Add(I386.SubR(Reg32.ESI, Val32.New(3)));
            codes.Add(I386.SubR(Reg32.EDI, Val32.New(3)));
            codes.Add(I386.Mov(Reg32.ECX, Reg32.EAX));
            codes.Add(I386.Shr(Reg32.ECX, 2));
            codes.Add(I386.Rep());
            codes.Add(I386.Movsd());
            codes.Add(I386.Pop(Reg32.ESI));
            codes.Add(I386.Pop(Reg32.EDI));
            codes.Add(I386.Popf());
        }

        public static void Cpuid(OpModule codes, NodeBase result, NodeBase eax)
        {
            codes.Add(I386.Pushf());
            codes.Add(I386.Push(Reg32.EDI));
            codes.Add(I386.Push(Reg32.EBX));
            result.AddCodesV(codes, "push", null);
            eax.AddCodesV(codes, "mov", null);
            codes.Add(I386.Pop(Reg32.EDI));
            codes.Add(I586.Cpuid());
            codes.Add(I386.Cld());
            codes.Add(I386.Stosd());
            codes.Add(I386.Mov(Reg32.EAX, Reg32.EBX));
            codes.Add(I386.Stosd());
            codes.Add(I386.Mov(Reg32.EAX, Reg32.ECX));
            codes.Add(I386.Stosd());
            codes.Add(I386.Mov(Reg32.EAX, Reg32.EDX));
            codes.Add(I386.Stosd());
            codes.Add(I386.Pop(Reg32.EBX));
            codes.Add(I386.Pop(Reg32.EDI));
            codes.Add(I386.Popf());
        }
    }
}
