using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        public static void TestMovx_8()
        {
            // Movzx

            MovzxB(Reg32.EAX, Reg8.BL)
                .Test("movzx eax, bl", "0F-B6-C3");
            MovzxBA(Reg32.EAX, Addr32.New(Reg32.EDX))
                .Test("movzx eax, byte [edx]", "0F-B6-02");
            MovzxBA(Reg32.EBX, Addr32.New(Reg32.EAX))
                .Test("movzx ebx, byte [eax]", "0F-B6-18");
            MovzxBA(Reg32.ECX, Addr32.New(Reg32.ESP))
                .Test("movzx ecx, byte [esp]", "0F-B6-0C-24");
            MovzxBA(Reg32.EBP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("movzx ebp, byte [eax+0x1000]", "0F-B6-A8-00-10-00-00");
            MovzxBA(Reg32.EAX, Addr32.NewUInt(0x12345678))
                .Test("movzx eax, byte [0x12345678]", "0F-B6-05-78-56-34-12");

            MovzxWB(Reg16.AX, Reg8.BL)
                .Test("movzx ax, bl", "66-0F-B6-C3");
            MovzxWBA(Reg16.AX, Addr32.New(Reg32.EDX))
                .Test("movzx ax, byte [edx]", "66-0F-B6-02");
            MovzxWBA(Reg16.BX, Addr32.New(Reg32.EAX))
                .Test("movzx bx, byte [eax]", "66-0F-B6-18");
            MovzxWBA(Reg16.CX, Addr32.New(Reg32.ESP))
                .Test("movzx cx, byte [esp]", "66-0F-B6-0C-24");
            MovzxWBA(Reg16.BP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("movzx bp, byte [eax+0x1000]", "66-0F-B6-A8-00-10-00-00");
            MovzxWBA(Reg16.AX, Addr32.NewUInt(0x12345678))
                .Test("movzx ax, byte [0x12345678]", "66-0F-B6-05-78-56-34-12");

            // Movsx

            MovsxB(Reg32.EAX, Reg8.BL)
                .Test("movsx eax, bl", "0F-BE-C3");
            MovsxBA(Reg32.EAX, Addr32.New(Reg32.EDX))
                .Test("movsx eax, byte [edx]", "0F-BE-02");
            MovsxBA(Reg32.EBX, Addr32.New(Reg32.EAX))
                .Test("movsx ebx, byte [eax]", "0F-BE-18");
            MovsxBA(Reg32.ECX, Addr32.New(Reg32.ESP))
                .Test("movsx ecx, byte [esp]", "0F-BE-0C-24");
            MovsxBA(Reg32.EBP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("movsx ebp, byte [eax+0x1000]", "0F-BE-A8-00-10-00-00");
            MovsxBA(Reg32.EAX, Addr32.NewUInt(0x12345678))
                .Test("movsx eax, byte [0x12345678]", "0F-BE-05-78-56-34-12");

            MovsxWB(Reg16.AX, Reg8.BL)
                .Test("movsx ax, bl", "66-0F-BE-C3");
            MovsxWBA(Reg16.AX, Addr32.New(Reg32.EDX))
                .Test("movsx ax, byte [edx]", "66-0F-BE-02");
            MovsxWBA(Reg16.BX, Addr32.New(Reg32.EAX))
                .Test("movsx bx, byte [eax]", "66-0F-BE-18");
            MovsxWBA(Reg16.CX, Addr32.New(Reg32.ESP))
                .Test("movsx cx, byte [esp]", "66-0F-BE-0C-24");
            MovsxWBA(Reg16.BP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("movsx bp, byte [eax+0x1000]", "66-0F-BE-A8-00-10-00-00");
            MovsxWBA(Reg16.AX, Addr32.NewUInt(0x12345678))
                .Test("movsx ax, byte [0x12345678]", "66-0F-BE-05-78-56-34-12");
        }
    }
}
