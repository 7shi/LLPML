using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        public static void TestMovx_16()
        {
            // Movzx

            MovzxW(Reg32.EAX, Reg16.BX)
                .Test("movzx eax, bx", "0F-B7-C3");
            MovzxWA(Reg32.EAX, Addr32.New(Reg32.EDX))
                .Test("movzx eax, word [edx]", "0F-B7-02");
            MovzxWA(Reg32.EBX, Addr32.New(Reg32.EAX))
                .Test("movzx ebx, word [eax]", "0F-B7-18");
            MovzxWA(Reg32.ECX, Addr32.New(Reg32.ESP))
                .Test("movzx ecx, word [esp]", "0F-B7-0C-24");
            MovzxWA(Reg32.EBP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("movzx ebp, word [eax+0x1000]", "0F-B7-A8-00-10-00-00");
            MovzxWA(Reg32.EAX, Addr32.NewUInt(0x12345678))
                .Test("movzx eax, word [0x12345678]", "0F-B7-05-78-56-34-12");

            // Movsx

            MovsxW(Reg32.EAX, Reg16.BX)
                .Test("movsx eax, bx", "0F-BF-C3");
            MovsxWA(Reg32.EAX, Addr32.New(Reg32.EDX))
                .Test("movsx eax, word [edx]", "0F-BF-02");
            MovsxWA(Reg32.EBX, Addr32.New(Reg32.EAX))
                .Test("movsx ebx, word [eax]", "0F-BF-18");
            MovsxWA(Reg32.ECX, Addr32.New(Reg32.ESP))
                .Test("movsx ecx, word [esp]", "0F-BF-0C-24");
            MovsxWA(Reg32.EBP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("movsx ebp, word [eax+0x1000]", "0F-BF-A8-00-10-00-00");
            MovsxWA(Reg32.EAX, Addr32.NewUInt(0x12345678))
                .Test("movsx eax, word [0x12345678]", "0F-BF-05-78-56-34-12");
        }
    }
}
