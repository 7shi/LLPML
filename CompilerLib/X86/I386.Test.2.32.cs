using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        public static void Test2_32()
        {
            // Mov, Add, Or, Adc, Sbb, And, Sub, Xor, Cmp, Test, Xchg

            // Mov
            MovR(Reg32.EAX, Val32.New(4))
                .Test("mov eax, 4", "B8-04-00-00-00");
            MovR(Reg32.ESP, Val32.New(4))
                .Test("mov esp, 4", "BC-04-00-00-00");
            Mov(Reg32.EAX, Reg32.EBX)
                .Test("mov eax, ebx", "89-D8");
            MovRA(Reg32.EAX, Addr32.New(Reg32.EDX))
                .Test("mov eax, [edx]", "8B-02");
            MovRA(Reg32.EBX, Addr32.New(Reg32.EAX))
                .Test("mov ebx, [eax]", "8B-18");
            MovRA(Reg32.ECX, Addr32.New(Reg32.ESP))
                .Test("mov ecx, [esp]", "8B-0C-24");
            MovRA(Reg32.EBP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("mov ebp, [eax+0x1000]", "8B-A8-00-10-00-00");
            MovAR(Addr32.New(Reg32.EDX), Reg32.EAX)
                .Test("mov [edx], eax", "89-02");
            MovAR(Addr32.New(Reg32.EAX), Reg32.EBX)
                .Test("mov [eax], ebx", "89-18");
            MovAR(Addr32.New(Reg32.ESP), Reg32.ECX)
                .Test("mov [esp], ecx", "89-0C-24");
            MovAR(Addr32.NewRO(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("mov [eax+0x1000], ebp", "89-A8-00-10-00-00");
            MovA(Addr32.New(Reg32.EAX), Val32.New(1))
                .Test("mov dword [eax], 1", "C7-00-01-00-00-00");
            MovA(Addr32.NewRO(Reg32.EBP, -4), Val32.New(8))
                .Test("mov dword [ebp-4], 8", "C7-45-FC-08-00-00-00");
            MovRA(Reg32.EAX, Addr32.NewUInt(0x12345678))
                .Test("mov eax, [0x12345678]", "A1-78-56-34-12");
            MovAR(Addr32.NewUInt(0x12345678), Reg32.EAX)
                .Test("mov [0x12345678], eax", "A3-78-56-34-12");

            // Add
            AddR(Reg32.EAX, Val32.New(4))
                .Test("add eax, 4", "05-04-00-00-00");
            AddR(Reg32.ESP, Val32.New(4))
                .Test("add esp, 4", "81-C4-04-00-00-00");
            Add(Reg32.EAX, Reg32.EBX)
                .Test("add eax, ebx", "01-D8");
            AddRA(Reg32.EAX, Addr32.New(Reg32.EDX))
                .Test("add eax, [edx]", "03-02");
            AddRA(Reg32.EBX, Addr32.New(Reg32.EAX))
                .Test("add ebx, [eax]", "03-18");
            AddRA(Reg32.ECX, Addr32.New(Reg32.ESP))
                .Test("add ecx, [esp]", "03-0C-24");
            AddRA(Reg32.EBP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("add ebp, [eax+0x1000]", "03-A8-00-10-00-00");
            AddAR(Addr32.New(Reg32.EDX), Reg32.EAX)
                .Test("add [edx], eax", "01-02");
            AddAR(Addr32.New(Reg32.EAX), Reg32.EBX)
                .Test("add [eax], ebx", "01-18");
            AddAR(Addr32.New(Reg32.ESP), Reg32.ECX)
                .Test("add [esp], ecx", "01-0C-24");
            AddAR(Addr32.NewRO(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("add [eax+0x1000], ebp", "01-A8-00-10-00-00");
            AddA(Addr32.New(Reg32.EAX), Val32.New(1))
                .Test("add dword [eax], 1", "81-00-01-00-00-00");
            AddA(Addr32.NewRO(Reg32.EBP, -4), Val32.New(8))
                .Test("add dword [ebp-4], 8", "81-45-FC-08-00-00-00");

            // Or
            OrR(Reg32.EAX, Val32.New(4))
                .Test("or eax, 4", "0D-04-00-00-00");
            OrR(Reg32.ESP, Val32.New(4))
                .Test("or esp, 4", "81-CC-04-00-00-00");
            Or(Reg32.EAX, Reg32.EBX)
                .Test("or eax, ebx", "09-D8");
            OrRA(Reg32.EAX, Addr32.New(Reg32.EDX))
                .Test("or eax, [edx]", "0B-02");
            OrRA(Reg32.EBX, Addr32.New(Reg32.EAX))
                .Test("or ebx, [eax]", "0B-18");
            OrRA(Reg32.ECX, Addr32.New(Reg32.ESP))
                .Test("or ecx, [esp]", "0B-0C-24");
            OrRA(Reg32.EBP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("or ebp, [eax+0x1000]", "0B-A8-00-10-00-00");
            OrAR(Addr32.New(Reg32.EDX), Reg32.EAX)
                .Test("or [edx], eax", "09-02");
            OrAR(Addr32.New(Reg32.EAX), Reg32.EBX)
                .Test("or [eax], ebx", "09-18");
            OrAR(Addr32.New(Reg32.ESP), Reg32.ECX)
                .Test("or [esp], ecx", "09-0C-24");
            OrAR(Addr32.NewRO(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("or [eax+0x1000], ebp", "09-A8-00-10-00-00");
            OrA(Addr32.New(Reg32.EAX), Val32.New(1))
                .Test("or dword [eax], 1", "81-08-01-00-00-00");
            OrA(Addr32.NewRO(Reg32.EBP, -4), Val32.New(8))
                .Test("or dword [ebp-4], 8", "81-4D-FC-08-00-00-00");

            // Adc
            AdcR(Reg32.EAX, Val32.New(4))
                .Test("adc eax, 4", "15-04-00-00-00");
            AdcR(Reg32.ESP, Val32.New(4))
                .Test("adc esp, 4", "81-D4-04-00-00-00");
            Adc(Reg32.EAX, Reg32.EBX)
                .Test("adc eax, ebx", "11-D8");
            AdcRA(Reg32.EAX, Addr32.New(Reg32.EDX))
                .Test("adc eax, [edx]", "13-02");
            AdcRA(Reg32.EBX, Addr32.New(Reg32.EAX))
                .Test("adc ebx, [eax]", "13-18");
            AdcRA(Reg32.ECX, Addr32.New(Reg32.ESP))
                .Test("adc ecx, [esp]", "13-0C-24");
            AdcRA(Reg32.EBP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("adc ebp, [eax+0x1000]", "13-A8-00-10-00-00");
            AdcAR(Addr32.New(Reg32.EDX), Reg32.EAX)
                .Test("adc [edx], eax", "11-02");
            AdcAR(Addr32.New(Reg32.EAX), Reg32.EBX)
                .Test("adc [eax], ebx", "11-18");
            AdcAR(Addr32.New(Reg32.ESP), Reg32.ECX)
                .Test("adc [esp], ecx", "11-0C-24");
            AdcAR(Addr32.NewRO(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("adc [eax+0x1000], ebp", "11-A8-00-10-00-00");
            AdcA(Addr32.New(Reg32.EAX), Val32.New(1))
                .Test("adc dword [eax], 1", "81-10-01-00-00-00");
            AdcA(Addr32.NewRO(Reg32.EBP, -4), Val32.New(8))
                .Test("adc dword [ebp-4], 8", "81-55-FC-08-00-00-00");

            // Sbb
            SbbR(Reg32.EAX, Val32.New(4))
                .Test("sbb eax, 4", "1D-04-00-00-00");
            SbbR(Reg32.ESP, Val32.New(4))
                .Test("sbb esp, 4", "81-DC-04-00-00-00");
            Sbb(Reg32.EAX, Reg32.EBX)
                .Test("sbb eax, ebx", "19-D8");
            SbbRA(Reg32.EAX, Addr32.New(Reg32.EDX))
                .Test("sbb eax, [edx]", "1B-02");
            SbbRA(Reg32.EBX, Addr32.New(Reg32.EAX))
                .Test("sbb ebx, [eax]", "1B-18");
            SbbRA(Reg32.ECX, Addr32.New(Reg32.ESP))
                .Test("sbb ecx, [esp]", "1B-0C-24");
            SbbRA(Reg32.EBP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("sbb ebp, [eax+0x1000]", "1B-A8-00-10-00-00");
            SbbAR(Addr32.New(Reg32.EDX), Reg32.EAX)
                .Test("sbb [edx], eax", "19-02");
            SbbAR(Addr32.New(Reg32.EAX), Reg32.EBX)
                .Test("sbb [eax], ebx", "19-18");
            SbbAR(Addr32.New(Reg32.ESP), Reg32.ECX)
                .Test("sbb [esp], ecx", "19-0C-24");
            SbbAR(Addr32.NewRO(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("sbb [eax+0x1000], ebp", "19-A8-00-10-00-00");
            SbbA(Addr32.New(Reg32.EAX), Val32.New(1))
                .Test("sbb dword [eax], 1", "81-18-01-00-00-00");
            SbbA(Addr32.NewRO(Reg32.EBP, -4), Val32.New(8))
                .Test("sbb dword [ebp-4], 8", "81-5D-FC-08-00-00-00");

            // And
            AndR(Reg32.EAX, Val32.New(4))
                .Test("and eax, 4", "25-04-00-00-00");
            AndR(Reg32.ESP, Val32.New(4))
                .Test("and esp, 4", "81-E4-04-00-00-00");
            And(Reg32.EAX, Reg32.EBX)
                .Test("and eax, ebx", "21-D8");
            AndRA(Reg32.EAX, Addr32.New(Reg32.EDX))
                .Test("and eax, [edx]", "23-02");
            AndRA(Reg32.EBX, Addr32.New(Reg32.EAX))
                .Test("and ebx, [eax]", "23-18");
            AndRA(Reg32.ECX, Addr32.New(Reg32.ESP))
                .Test("and ecx, [esp]", "23-0C-24");
            AndRA(Reg32.EBP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("and ebp, [eax+0x1000]", "23-A8-00-10-00-00");
            AndAR(Addr32.New(Reg32.EDX), Reg32.EAX)
                .Test("and [edx], eax", "21-02");
            AndAR(Addr32.New(Reg32.EAX), Reg32.EBX)
                .Test("and [eax], ebx", "21-18");
            AndAR(Addr32.New(Reg32.ESP), Reg32.ECX)
                .Test("and [esp], ecx", "21-0C-24");
            AndAR(Addr32.NewRO(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("and [eax+0x1000], ebp", "21-A8-00-10-00-00");
            AndA(Addr32.New(Reg32.EAX), Val32.New(1))
                .Test("and dword [eax], 1", "81-20-01-00-00-00");
            AndA(Addr32.NewRO(Reg32.EBP, -4), Val32.New(8))
                .Test("and dword [ebp-4], 8", "81-65-FC-08-00-00-00");

            // Sub
            SubR(Reg32.EAX, Val32.New(4))
                .Test("sub eax, 4", "2D-04-00-00-00");
            SubR(Reg32.ESP, Val32.New(4))
                .Test("sub esp, 4", "81-EC-04-00-00-00");
            Sub(Reg32.EAX, Reg32.EBX)
                .Test("sub eax, ebx", "29-D8");
            SubRA(Reg32.EAX, Addr32.New(Reg32.EDX))
                .Test("sub eax, [edx]", "2B-02");
            SubRA(Reg32.EBX, Addr32.New(Reg32.EAX))
                .Test("sub ebx, [eax]", "2B-18");
            SubRA(Reg32.ECX, Addr32.New(Reg32.ESP))
                .Test("sub ecx, [esp]", "2B-0C-24");
            SubRA(Reg32.EBP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("sub ebp, [eax+0x1000]", "2B-A8-00-10-00-00");
            SubAR(Addr32.New(Reg32.EDX), Reg32.EAX)
                .Test("sub [edx], eax", "29-02");
            SubAR(Addr32.New(Reg32.EAX), Reg32.EBX)
                .Test("sub [eax], ebx", "29-18");
            SubAR(Addr32.New(Reg32.ESP), Reg32.ECX)
                .Test("sub [esp], ecx", "29-0C-24");
            SubAR(Addr32.NewRO(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("sub [eax+0x1000], ebp", "29-A8-00-10-00-00");
            SubA(Addr32.New(Reg32.EAX), Val32.New(1))
                .Test("sub dword [eax], 1", "81-28-01-00-00-00");
            SubA(Addr32.NewRO(Reg32.EBP, -4), Val32.New(8))
                .Test("sub dword [ebp-4], 8", "81-6D-FC-08-00-00-00");

            // Xor
            XorR(Reg32.EAX, Val32.New(4))
                .Test("xor eax, 4", "35-04-00-00-00");
            XorR(Reg32.ESP, Val32.New(4))
                .Test("xor esp, 4", "81-F4-04-00-00-00");
            Xor(Reg32.EAX, Reg32.EBX)
                .Test("xor eax, ebx", "31-D8");
            XorRA(Reg32.EAX, Addr32.New(Reg32.EDX))
                .Test("xor eax, [edx]", "33-02");
            XorRA(Reg32.EBX, Addr32.New(Reg32.EAX))
                .Test("xor ebx, [eax]", "33-18");
            XorRA(Reg32.ECX, Addr32.New(Reg32.ESP))
                .Test("xor ecx, [esp]", "33-0C-24");
            XorRA(Reg32.EBP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("xor ebp, [eax+0x1000]", "33-A8-00-10-00-00");
            XorAR(Addr32.New(Reg32.EDX), Reg32.EAX)
                .Test("xor [edx], eax", "31-02");
            XorAR(Addr32.New(Reg32.EAX), Reg32.EBX)
                .Test("xor [eax], ebx", "31-18");
            XorAR(Addr32.New(Reg32.ESP), Reg32.ECX)
                .Test("xor [esp], ecx", "31-0C-24");
            XorAR(Addr32.NewRO(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("xor [eax+0x1000], ebp", "31-A8-00-10-00-00");
            XorA(Addr32.New(Reg32.EAX), Val32.New(1))
                .Test("xor dword [eax], 1", "81-30-01-00-00-00");
            XorA(Addr32.NewRO(Reg32.EBP, -4), Val32.New(8))
                .Test("xor dword [ebp-4], 8", "81-75-FC-08-00-00-00");

            // Cmp
            CmpR(Reg32.EAX, Val32.New(4))
                .Test("cmp eax, 4", "3D-04-00-00-00");
            CmpR(Reg32.ESP, Val32.New(4))
                .Test("cmp esp, 4", "81-FC-04-00-00-00");
            Cmp(Reg32.EAX, Reg32.EBX)
                .Test("cmp eax, ebx", "39-D8");
            CmpRA(Reg32.EAX, Addr32.New(Reg32.EDX))
                .Test("cmp eax, [edx]", "3B-02");
            CmpRA(Reg32.EBX, Addr32.New(Reg32.EAX))
                .Test("cmp ebx, [eax]", "3B-18");
            CmpRA(Reg32.ECX, Addr32.New(Reg32.ESP))
                .Test("cmp ecx, [esp]", "3B-0C-24");
            CmpRA(Reg32.EBP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("cmp ebp, [eax+0x1000]", "3B-A8-00-10-00-00");
            CmpAR(Addr32.New(Reg32.EDX), Reg32.EAX)
                .Test("cmp [edx], eax", "39-02");
            CmpAR(Addr32.New(Reg32.EAX), Reg32.EBX)
                .Test("cmp [eax], ebx", "39-18");
            CmpAR(Addr32.New(Reg32.ESP), Reg32.ECX)
                .Test("cmp [esp], ecx", "39-0C-24");
            CmpAR(Addr32.NewRO(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("cmp [eax+0x1000], ebp", "39-A8-00-10-00-00");
            CmpA(Addr32.New(Reg32.EAX), Val32.New(1))
                .Test("cmp dword [eax], 1", "81-38-01-00-00-00");
            CmpA(Addr32.NewRO(Reg32.EBP, -4), Val32.New(8))
                .Test("cmp dword [ebp-4], 8", "81-7D-FC-08-00-00-00");

            // Test
            TestR(Reg32.EAX, Val32.New(4))
                .Test("test eax, 4", "A9-04-00-00-00");
            TestR(Reg32.ESP, Val32.New(4))
                .Test("test esp, 4", "F7-C4-04-00-00-00");
            Test(Reg32.EAX, Reg32.EBX)
                .Test("test eax, ebx", "85-D8");
            TestAR(Addr32.New(Reg32.EDX), Reg32.EAX)
                .Test("test [edx], eax", "85-02");
            TestAR(Addr32.New(Reg32.EAX), Reg32.EBX)
                .Test("test [eax], ebx", "85-18");
            TestAR(Addr32.New(Reg32.ESP), Reg32.ECX)
                .Test("test [esp], ecx", "85-0C-24");
            TestAR(Addr32.NewRO(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("test [eax+0x1000], ebp", "85-A8-00-10-00-00");
            TestA(Addr32.New(Reg32.EAX), Val32.New(1))
                .Test("test dword [eax], 1", "F7-00-01-00-00-00");
            TestA(Addr32.NewRO(Reg32.EBP, -4), Val32.New(8))
                .Test("test dword [ebp-4], 8", "F7-45-FC-08-00-00-00");

            // Xchg
            Xchg(Reg32.EAX, Reg32.EAX)
                .Test("xchg eax, eax", "90");
            Xchg(Reg32.EAX, Reg32.EBX)
                .Test("xchg eax, ebx", "93");
            Xchg(Reg32.EBX, Reg32.EAX)
                .Test("xchg ebx, eax", "93");
            XchgRA(Reg32.EAX, Addr32.New(Reg32.EDX))
                .Test("xchg eax, [edx]", "87-02");
            XchgRA(Reg32.EBX, Addr32.New(Reg32.EAX))
                .Test("xchg ebx, [eax]", "87-18");
            XchgRA(Reg32.ECX, Addr32.New(Reg32.ESP))
                .Test("xchg ecx, [esp]", "87-0C-24");
            XchgRA(Reg32.EBP, Addr32.NewRO(Reg32.EAX, 0x1000))
                .Test("xchg ebp, [eax+0x1000]", "87-A8-00-10-00-00");
            XchgAR(Addr32.New(Reg32.EDX), Reg32.EAX)
                .Test("xchg [edx], eax", "87-02");
        }
    }
}
