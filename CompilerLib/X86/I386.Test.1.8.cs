using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        public static void Test1_8()
        {
            // Push, Pop, Inc, Dec, Not, Neg, Mul, Imul, Div, Idiv

            // Push
            PushB((byte)0x12)
                .Test("push byte 0x12", "6A-12");

            // Inc
            IncB(Reg8.AL)
                .Test("inc al", "FE-C0");
            IncB(Reg8.AH)
                .Test("inc ah", "FE-C4");
            IncBA(Addr32.New(Reg32.EAX))
                .Test("inc byte [eax]", "FE-00");
            IncBA(Addr32.New(Reg32.EBP))
                .Test("inc byte [ebp]", "FE-45-00");
            IncBA(Addr32.NewRO(Reg32.EBP, -4))
                .Test("inc byte [ebp-4]", "FE-45-FC");
            IncBA(Addr32.NewRO(Reg32.ESI, 0x1000))
                .Test("inc byte [esi+0x1000]", "FE-86-00-10-00-00");

            // Dec
            DecB(Reg8.AL)
                .Test("dec al", "FE-C8");
            DecB(Reg8.AH)
                .Test("dec ah", "FE-CC");
            DecBA(Addr32.New(Reg32.EAX))
                .Test("dec byte [eax]", "FE-08");
            DecBA(Addr32.New(Reg32.EBP))
                .Test("dec byte [ebp]", "FE-4D-00");
            DecBA(Addr32.NewRO(Reg32.EBP, -4))
                .Test("dec byte [ebp-4]", "FE-4D-FC");
            DecBA(Addr32.NewRO(Reg32.ESI, 0x1000))
                .Test("dec byte [esi+0x1000]", "FE-8E-00-10-00-00");

            // Not
            NotB(Reg8.AL)
                .Test("not al", "F6-D0");
            NotB(Reg8.AH)
                .Test("not ah", "F6-D4");
            NotBA(Addr32.New(Reg32.EAX))
                .Test("not byte [eax]", "F6-10");
            NotBA(Addr32.New(Reg32.EBP))
                .Test("not byte [ebp]", "F6-55-00");
            NotBA(Addr32.NewRO(Reg32.EBP, -4))
                .Test("not byte [ebp-4]", "F6-55-FC");
            NotBA(Addr32.NewRO(Reg32.ESI, 0x1000))
                .Test("not byte [esi+0x1000]", "F6-96-00-10-00-00");

            // Neg
            NegB(Reg8.AL)
                .Test("neg al", "F6-D8");
            NegB(Reg8.AH)
                .Test("neg ah", "F6-DC");
            NegBA(Addr32.New(Reg32.EAX))
                .Test("neg byte [eax]", "F6-18");
            NegBA(Addr32.New(Reg32.EBP))
                .Test("neg byte [ebp]", "F6-5D-00");
            NegBA(Addr32.NewRO(Reg32.EBP, -4))
                .Test("neg byte [ebp-4]", "F6-5D-FC");
            NegBA(Addr32.NewRO(Reg32.ESI, 0x1000))
                .Test("neg byte [esi+0x1000]", "F6-9E-00-10-00-00");

            // Mul
            MulB(Reg8.AL)
                .Test("mul al", "F6-E0");
            MulB(Reg8.AH)
                .Test("mul ah", "F6-E4");
            MulBA(Addr32.New(Reg32.EAX))
                .Test("mul byte [eax]", "F6-20");
            MulBA(Addr32.New(Reg32.EBP))
                .Test("mul byte [ebp]", "F6-65-00");
            MulBA(Addr32.NewRO(Reg32.EBP, -4))
                .Test("mul byte [ebp-4]", "F6-65-FC");
            MulBA(Addr32.NewRO(Reg32.ESI, 0x1000))
                .Test("mul byte [esi+0x1000]", "F6-A6-00-10-00-00");

            // Imul
            ImulB(Reg8.AL)
                .Test("imul al", "F6-E8");
            ImulB(Reg8.AH)
                .Test("imul ah", "F6-EC");
            ImulBA(Addr32.New(Reg32.EAX))
                .Test("imul byte [eax]", "F6-28");
            ImulBA(Addr32.New(Reg32.EBP))
                .Test("imul byte [ebp]", "F6-6D-00");
            ImulBA(Addr32.NewRO(Reg32.EBP, -4))
                .Test("imul byte [ebp-4]", "F6-6D-FC");
            ImulBA(Addr32.NewRO(Reg32.ESI, 0x1000))
                .Test("imul byte [esi+0x1000]", "F6-AE-00-10-00-00");

            // Div
            DivB(Reg8.AL)
                .Test("div al", "F6-F0");
            DivB(Reg8.AH)
                .Test("div ah", "F6-F4");
            DivBA(Addr32.New(Reg32.EAX))
                .Test("div byte [eax]", "F6-30");
            DivBA(Addr32.New(Reg32.EBP))
                .Test("div byte [ebp]", "F6-75-00");
            DivBA(Addr32.NewRO(Reg32.EBP, -4))
                .Test("div byte [ebp-4]", "F6-75-FC");
            DivBA(Addr32.NewRO(Reg32.ESI, 0x1000))
                .Test("div byte [esi+0x1000]", "F6-B6-00-10-00-00");

            // Idiv
            IdivB(Reg8.AL)
                .Test("idiv al", "F6-F8");
            IdivB(Reg8.AH)
                .Test("idiv ah", "F6-FC");
            IdivBA(Addr32.New(Reg32.EAX))
                .Test("idiv byte [eax]", "F6-38");
            IdivBA(Addr32.New(Reg32.EBP))
                .Test("idiv byte [ebp]", "F6-7D-00");
            IdivBA(Addr32.NewRO(Reg32.EBP, -4))
                .Test("idiv byte [ebp-4]", "F6-7D-FC");
            IdivBA(Addr32.NewRO(Reg32.ESI, 0x1000))
                .Test("idiv byte [esi+0x1000]", "F6-BE-00-10-00-00");
        }
    }
}
