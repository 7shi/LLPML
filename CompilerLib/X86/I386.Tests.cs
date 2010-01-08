using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        public static void Test()
        {
            #region  Mov, Add, Or, Adc, Sbb, And, Sub, Xor, Cmp, Test, Xchg

            #region Mov
            Mov(Reg32.EAX, 4)
                .Test("mov eax, 4", "B8-04-00-00-00");
            Mov(Reg32.ESP, 4)
                .Test("mov esp, 4", "BC-04-00-00-00");
            Mov(Reg32.EAX, Reg32.EBX)
                .Test("mov eax, ebx", "89-D8");
            Mov(Reg32.EAX, new Addr32(Reg32.EDX))
                .Test("mov eax, [edx]", "8B-02");
            Mov(Reg32.EBX, new Addr32(Reg32.EAX))
                .Test("mov ebx, [eax]", "8B-18");
            Mov(Reg32.ECX, new Addr32(Reg32.ESP))
                .Test("mov ecx, [esp]", "8B-0C-24");
            Mov(Reg32.EBP, new Addr32(Reg32.EAX, 0x1000))
                .Test("mov ebp, [eax+0x1000]", "8B-A8-00-10-00-00");
            Mov(new Addr32(Reg32.EDX), Reg32.EAX)
                .Test("mov [edx], eax", "89-02");
            Mov(new Addr32(Reg32.EAX), Reg32.EBX)
                .Test("mov [eax], ebx", "89-18");
            Mov(new Addr32(Reg32.ESP), Reg32.ECX)
                .Test("mov [esp], ecx", "89-0C-24");
            Mov(new Addr32(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("mov [eax+0x1000], ebp", "89-A8-00-10-00-00");
            Mov(new Addr32(Reg32.EAX), 1)
                .Test("mov dword [eax], 1", "C7-00-01-00-00-00");
            Mov(new Addr32(Reg32.EBP, -4), 8)
                .Test("mov dword [ebp-4], 8", "C7-45-FC-08-00-00-00");
            Mov(Reg32.EAX, new Addr32(0x12345678))
                .Test("mov eax, [0x12345678]", "A1-78-56-34-12");
            Mov(new Addr32(0x12345678), Reg32.EAX)
                .Test("mov [0x12345678], eax", "A3-78-56-34-12");
            #endregion

            #region Add
            Add(Reg32.EAX, 4)
                .Test("add eax, 4", "05-04-00-00-00");
            Add(Reg32.ESP, 4)
                .Test("add esp, 4", "81-C4-04-00-00-00");
            Add(Reg32.EAX, Reg32.EBX)
                .Test("add eax, ebx", "01-D8");
            Add(Reg32.EAX, new Addr32(Reg32.EDX))
                .Test("add eax, [edx]", "03-02");
            Add(Reg32.EBX, new Addr32(Reg32.EAX))
                .Test("add ebx, [eax]", "03-18");
            Add(Reg32.ECX, new Addr32(Reg32.ESP))
                .Test("add ecx, [esp]", "03-0C-24");
            Add(Reg32.EBP, new Addr32(Reg32.EAX, 0x1000))
                .Test("add ebp, [eax+0x1000]", "03-A8-00-10-00-00");
            Add(new Addr32(Reg32.EDX), Reg32.EAX)
                .Test("add [edx], eax", "01-02");
            Add(new Addr32(Reg32.EAX), Reg32.EBX)
                .Test("add [eax], ebx", "01-18");
            Add(new Addr32(Reg32.ESP), Reg32.ECX)
                .Test("add [esp], ecx", "01-0C-24");
            Add(new Addr32(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("add [eax+0x1000], ebp", "01-A8-00-10-00-00");
            Add(new Addr32(Reg32.EAX), 1)
                .Test("add dword [eax], 1", "81-00-01-00-00-00");
            Add(new Addr32(Reg32.EBP, -4), 8)
                .Test("add dword [ebp-4], 8", "81-45-FC-08-00-00-00");
            #endregion

            #region Or
            Or(Reg32.EAX, 4)
                .Test("or eax, 4", "0D-04-00-00-00");
            Or(Reg32.ESP, 4)
                .Test("or esp, 4", "81-CC-04-00-00-00");
            Or(Reg32.EAX, Reg32.EBX)
                .Test("or eax, ebx", "09-D8");
            Or(Reg32.EAX, new Addr32(Reg32.EDX))
                .Test("or eax, [edx]", "0B-02");
            Or(Reg32.EBX, new Addr32(Reg32.EAX))
                .Test("or ebx, [eax]", "0B-18");
            Or(Reg32.ECX, new Addr32(Reg32.ESP))
                .Test("or ecx, [esp]", "0B-0C-24");
            Or(Reg32.EBP, new Addr32(Reg32.EAX, 0x1000))
                .Test("or ebp, [eax+0x1000]", "0B-A8-00-10-00-00");
            Or(new Addr32(Reg32.EDX), Reg32.EAX)
                .Test("or [edx], eax", "09-02");
            Or(new Addr32(Reg32.EAX), Reg32.EBX)
                .Test("or [eax], ebx", "09-18");
            Or(new Addr32(Reg32.ESP), Reg32.ECX)
                .Test("or [esp], ecx", "09-0C-24");
            Or(new Addr32(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("or [eax+0x1000], ebp", "09-A8-00-10-00-00");
            Or(new Addr32(Reg32.EAX), 1)
                .Test("or dword [eax], 1", "81-08-01-00-00-00");
            Or(new Addr32(Reg32.EBP, -4), 8)
                .Test("or dword [ebp-4], 8", "81-4D-FC-08-00-00-00");
            #endregion

            #region Adc
            Adc(Reg32.EAX, 4)
                .Test("adc eax, 4", "15-04-00-00-00");
            Adc(Reg32.ESP, 4)
                .Test("adc esp, 4", "81-D4-04-00-00-00");
            Adc(Reg32.EAX, Reg32.EBX)
                .Test("adc eax, ebx", "11-D8");
            Adc(Reg32.EAX, new Addr32(Reg32.EDX))
                .Test("adc eax, [edx]", "13-02");
            Adc(Reg32.EBX, new Addr32(Reg32.EAX))
                .Test("adc ebx, [eax]", "13-18");
            Adc(Reg32.ECX, new Addr32(Reg32.ESP))
                .Test("adc ecx, [esp]", "13-0C-24");
            Adc(Reg32.EBP, new Addr32(Reg32.EAX, 0x1000))
                .Test("adc ebp, [eax+0x1000]", "13-A8-00-10-00-00");
            Adc(new Addr32(Reg32.EDX), Reg32.EAX)
                .Test("adc [edx], eax", "11-02");
            Adc(new Addr32(Reg32.EAX), Reg32.EBX)
                .Test("adc [eax], ebx", "11-18");
            Adc(new Addr32(Reg32.ESP), Reg32.ECX)
                .Test("adc [esp], ecx", "11-0C-24");
            Adc(new Addr32(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("adc [eax+0x1000], ebp", "11-A8-00-10-00-00");
            Adc(new Addr32(Reg32.EAX), 1)
                .Test("adc dword [eax], 1", "81-10-01-00-00-00");
            Adc(new Addr32(Reg32.EBP, -4), 8)
                .Test("adc dword [ebp-4], 8", "81-55-FC-08-00-00-00");
            #endregion

            #region Sbb
            Sbb(Reg32.EAX, 4)
                .Test("sbb eax, 4", "1D-04-00-00-00");
            Sbb(Reg32.ESP, 4)
                .Test("sbb esp, 4", "81-DC-04-00-00-00");
            Sbb(Reg32.EAX, Reg32.EBX)
                .Test("sbb eax, ebx", "19-D8");
            Sbb(Reg32.EAX, new Addr32(Reg32.EDX))
                .Test("sbb eax, [edx]", "1B-02");
            Sbb(Reg32.EBX, new Addr32(Reg32.EAX))
                .Test("sbb ebx, [eax]", "1B-18");
            Sbb(Reg32.ECX, new Addr32(Reg32.ESP))
                .Test("sbb ecx, [esp]", "1B-0C-24");
            Sbb(Reg32.EBP, new Addr32(Reg32.EAX, 0x1000))
                .Test("sbb ebp, [eax+0x1000]", "1B-A8-00-10-00-00");
            Sbb(new Addr32(Reg32.EDX), Reg32.EAX)
                .Test("sbb [edx], eax", "19-02");
            Sbb(new Addr32(Reg32.EAX), Reg32.EBX)
                .Test("sbb [eax], ebx", "19-18");
            Sbb(new Addr32(Reg32.ESP), Reg32.ECX)
                .Test("sbb [esp], ecx", "19-0C-24");
            Sbb(new Addr32(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("sbb [eax+0x1000], ebp", "19-A8-00-10-00-00");
            Sbb(new Addr32(Reg32.EAX), 1)
                .Test("sbb dword [eax], 1", "81-18-01-00-00-00");
            Sbb(new Addr32(Reg32.EBP, -4), 8)
                .Test("sbb dword [ebp-4], 8", "81-5D-FC-08-00-00-00");
            #endregion

            #region And
            And(Reg32.EAX, 4)
                .Test("and eax, 4", "25-04-00-00-00");
            And(Reg32.ESP, 4)
                .Test("and esp, 4", "81-E4-04-00-00-00");
            And(Reg32.EAX, Reg32.EBX)
                .Test("and eax, ebx", "21-D8");
            And(Reg32.EAX, new Addr32(Reg32.EDX))
                .Test("and eax, [edx]", "23-02");
            And(Reg32.EBX, new Addr32(Reg32.EAX))
                .Test("and ebx, [eax]", "23-18");
            And(Reg32.ECX, new Addr32(Reg32.ESP))
                .Test("and ecx, [esp]", "23-0C-24");
            And(Reg32.EBP, new Addr32(Reg32.EAX, 0x1000))
                .Test("and ebp, [eax+0x1000]", "23-A8-00-10-00-00");
            And(new Addr32(Reg32.EDX), Reg32.EAX)
                .Test("and [edx], eax", "21-02");
            And(new Addr32(Reg32.EAX), Reg32.EBX)
                .Test("and [eax], ebx", "21-18");
            And(new Addr32(Reg32.ESP), Reg32.ECX)
                .Test("and [esp], ecx", "21-0C-24");
            And(new Addr32(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("and [eax+0x1000], ebp", "21-A8-00-10-00-00");
            And(new Addr32(Reg32.EAX), 1)
                .Test("and dword [eax], 1", "81-20-01-00-00-00");
            And(new Addr32(Reg32.EBP, -4), 8)
                .Test("and dword [ebp-4], 8", "81-65-FC-08-00-00-00");
            #endregion

            #region Sub
            Sub(Reg32.EAX, 4)
                .Test("sub eax, 4", "2D-04-00-00-00");
            Sub(Reg32.ESP, 4)
                .Test("sub esp, 4", "81-EC-04-00-00-00");
            Sub(Reg32.EAX, Reg32.EBX)
                .Test("sub eax, ebx", "29-D8");
            Sub(Reg32.EAX, new Addr32(Reg32.EDX))
                .Test("sub eax, [edx]", "2B-02");
            Sub(Reg32.EBX, new Addr32(Reg32.EAX))
                .Test("sub ebx, [eax]", "2B-18");
            Sub(Reg32.ECX, new Addr32(Reg32.ESP))
                .Test("sub ecx, [esp]", "2B-0C-24");
            Sub(Reg32.EBP, new Addr32(Reg32.EAX, 0x1000))
                .Test("sub ebp, [eax+0x1000]", "2B-A8-00-10-00-00");
            Sub(new Addr32(Reg32.EDX), Reg32.EAX)
                .Test("sub [edx], eax", "29-02");
            Sub(new Addr32(Reg32.EAX), Reg32.EBX)
                .Test("sub [eax], ebx", "29-18");
            Sub(new Addr32(Reg32.ESP), Reg32.ECX)
                .Test("sub [esp], ecx", "29-0C-24");
            Sub(new Addr32(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("sub [eax+0x1000], ebp", "29-A8-00-10-00-00");
            Sub(new Addr32(Reg32.EAX), 1)
                .Test("sub dword [eax], 1", "81-28-01-00-00-00");
            Sub(new Addr32(Reg32.EBP, -4), 8)
                .Test("sub dword [ebp-4], 8", "81-6D-FC-08-00-00-00");
            #endregion

            #region Xor
            Xor(Reg32.EAX, 4)
                .Test("xor eax, 4", "35-04-00-00-00");
            Xor(Reg32.ESP, 4)
                .Test("xor esp, 4", "81-F4-04-00-00-00");
            Xor(Reg32.EAX, Reg32.EBX)
                .Test("xor eax, ebx", "31-D8");
            Xor(Reg32.EAX, new Addr32(Reg32.EDX))
                .Test("xor eax, [edx]", "33-02");
            Xor(Reg32.EBX, new Addr32(Reg32.EAX))
                .Test("xor ebx, [eax]", "33-18");
            Xor(Reg32.ECX, new Addr32(Reg32.ESP))
                .Test("xor ecx, [esp]", "33-0C-24");
            Xor(Reg32.EBP, new Addr32(Reg32.EAX, 0x1000))
                .Test("xor ebp, [eax+0x1000]", "33-A8-00-10-00-00");
            Xor(new Addr32(Reg32.EDX), Reg32.EAX)
                .Test("xor [edx], eax", "31-02");
            Xor(new Addr32(Reg32.EAX), Reg32.EBX)
                .Test("xor [eax], ebx", "31-18");
            Xor(new Addr32(Reg32.ESP), Reg32.ECX)
                .Test("xor [esp], ecx", "31-0C-24");
            Xor(new Addr32(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("xor [eax+0x1000], ebp", "31-A8-00-10-00-00");
            Xor(new Addr32(Reg32.EAX), 1)
                .Test("xor dword [eax], 1", "81-30-01-00-00-00");
            Xor(new Addr32(Reg32.EBP, -4), 8)
                .Test("xor dword [ebp-4], 8", "81-75-FC-08-00-00-00");
            #endregion

            #region Cmp
            Cmp(Reg32.EAX, 4)
                .Test("cmp eax, 4", "3D-04-00-00-00");
            Cmp(Reg32.ESP, 4)
                .Test("cmp esp, 4", "81-FC-04-00-00-00");
            Cmp(Reg32.EAX, Reg32.EBX)
                .Test("cmp eax, ebx", "39-D8");
            Cmp(Reg32.EAX, new Addr32(Reg32.EDX))
                .Test("cmp eax, [edx]", "3B-02");
            Cmp(Reg32.EBX, new Addr32(Reg32.EAX))
                .Test("cmp ebx, [eax]", "3B-18");
            Cmp(Reg32.ECX, new Addr32(Reg32.ESP))
                .Test("cmp ecx, [esp]", "3B-0C-24");
            Cmp(Reg32.EBP, new Addr32(Reg32.EAX, 0x1000))
                .Test("cmp ebp, [eax+0x1000]", "3B-A8-00-10-00-00");
            Cmp(new Addr32(Reg32.EDX), Reg32.EAX)
                .Test("cmp [edx], eax", "39-02");
            Cmp(new Addr32(Reg32.EAX), Reg32.EBX)
                .Test("cmp [eax], ebx", "39-18");
            Cmp(new Addr32(Reg32.ESP), Reg32.ECX)
                .Test("cmp [esp], ecx", "39-0C-24");
            Cmp(new Addr32(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("cmp [eax+0x1000], ebp", "39-A8-00-10-00-00");
            Cmp(new Addr32(Reg32.EAX), 1)
                .Test("cmp dword [eax], 1", "81-38-01-00-00-00");
            Cmp(new Addr32(Reg32.EBP, -4), 8)
                .Test("cmp dword [ebp-4], 8", "81-7D-FC-08-00-00-00");
            #endregion

            #region Test
            Test(Reg32.EAX, 4)
                .Test("test eax, 4", "A9-04-00-00-00");
            Test(Reg32.ESP, 4)
                .Test("test esp, 4", "F7-C4-04-00-00-00");
            Test(Reg32.EAX, Reg32.EBX)
                .Test("test eax, ebx", "85-D8");
            Test(new Addr32(Reg32.EDX), Reg32.EAX)
                .Test("test [edx], eax", "85-02");
            Test(new Addr32(Reg32.EAX), Reg32.EBX)
                .Test("test [eax], ebx", "85-18");
            Test(new Addr32(Reg32.ESP), Reg32.ECX)
                .Test("test [esp], ecx", "85-0C-24");
            Test(new Addr32(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("test [eax+0x1000], ebp", "85-A8-00-10-00-00");
            Test(new Addr32(Reg32.EAX), 1)
                .Test("test dword [eax], 1", "F7-00-01-00-00-00");
            Test(new Addr32(Reg32.EBP, -4), 8)
                .Test("test dword [ebp-4], 8", "F7-45-FC-08-00-00-00");
            #endregion

            #region Xchg
            Xchg(Reg32.EAX, Reg32.EAX)
                .Test("xchg eax, eax", "90");
            Xchg(Reg32.EAX, Reg32.EBX)
                .Test("xchg eax, ebx", "93");
            Xchg(Reg32.EBX, Reg32.EAX)
                .Test("xchg ebx, eax", "93");
            Xchg(Reg32.EAX, new Addr32(Reg32.EDX))
                .Test("xchg eax, [edx]", "87-02");
            Xchg(Reg32.EBX, new Addr32(Reg32.EAX))
                .Test("xchg ebx, [eax]", "87-18");
            Xchg(Reg32.ECX, new Addr32(Reg32.ESP))
                .Test("xchg ecx, [esp]", "87-0C-24");
            Xchg(Reg32.EBP, new Addr32(Reg32.EAX, 0x1000))
                .Test("xchg ebp, [eax+0x1000]", "87-A8-00-10-00-00");
            Xchg(new Addr32(Reg32.EDX), Reg32.EAX)
                .Test("xchg [edx], eax", "87-02");
            #endregion

            #endregion

            #region Push, Pop, Inc, Dec, Mul, Imul, Div, Idiv

            #region Push
            Push(Reg32.EAX)
                .Test("push eax", "50");
            Push(Reg32.ESP)
                .Test("push esp", "54");
            Push(new Addr32(Reg32.EAX))
                .Test("push dword [eax]", "FF-30");
            Push(new Addr32(Reg32.EBP))
                .Test("push dword [ebp]", "FF-75-00");
            Push(new Addr32(Reg32.EBP, -4))
                .Test("push dword [ebp-4]", "FF-75-FC");
            Push(new Addr32(Reg32.ESI, 0x1000))
                .Test("push dword [esi+0x1000]", "FF-B6-00-10-00-00");
            #endregion

            #region Pop
            Pop(Reg32.EAX)
                .Test("pop eax", "58");
            Pop(Reg32.ESP)
                .Test("pop esp", "5C");
            Pop(new Addr32(Reg32.EAX))
                .Test("pop dword [eax]", "8F-00");
            Pop(new Addr32(Reg32.EBP))
                .Test("pop dword [ebp]", "8F-45-00");
            Pop(new Addr32(Reg32.EBP, -4))
                .Test("pop dword [ebp-4]", "8F-45-FC");
            Pop(new Addr32(Reg32.ESI, 0x1000))
                .Test("pop dword [esi+0x1000]", "8F-86-00-10-00-00");
            #endregion

            #region Inc
            Inc(Reg32.EAX)
                .Test("inc eax", "40");
            Inc(Reg32.ESP)
                .Test("inc esp", "44");
            Inc(new Addr32(Reg32.EAX))
                .Test("inc dword [eax]", "FF-00");
            Inc(new Addr32(Reg32.EBP))
                .Test("inc dword [ebp]", "FF-45-00");
            Inc(new Addr32(Reg32.EBP, -4))
                .Test("inc dword [ebp-4]", "FF-45-FC");
            Inc(new Addr32(Reg32.ESI, 0x1000))
                .Test("inc dword [esi+0x1000]", "FF-86-00-10-00-00");
            #endregion

            #region Dec
            Dec(Reg32.EAX)
                .Test("dec eax", "48");
            Dec(Reg32.ESP)
                .Test("dec esp", "4C");
            Dec(new Addr32(Reg32.EAX))
                .Test("dec dword [eax]", "FF-08");
            Dec(new Addr32(Reg32.EBP))
                .Test("dec dword [ebp]", "FF-4D-00");
            Dec(new Addr32(Reg32.EBP, -4))
                .Test("dec dword [ebp-4]", "FF-4D-FC");
            Dec(new Addr32(Reg32.ESI, 0x1000))
                .Test("dec dword [esi+0x1000]", "FF-8E-00-10-00-00");
            #endregion

            #region Mul
            Mul(Reg32.EAX)
                .Test("mul eax", "F7-E0");
            Mul(Reg32.ESP)
                .Test("mul esp", "F7-E4");
            Mul(new Addr32(Reg32.EAX))
                .Test("mul dword [eax]", "F7-20");
            Mul(new Addr32(Reg32.EBP))
                .Test("mul dword [ebp]", "F7-65-00");
            Mul(new Addr32(Reg32.EBP, -4))
                .Test("mul dword [ebp-4]", "F7-65-FC");
            Mul(new Addr32(Reg32.ESI, 0x1000))
                .Test("mul dword [esi+0x1000]", "F7-A6-00-10-00-00");
            #endregion

            #region Imul
            Imul(Reg32.EAX)
                .Test("imul eax", "F7-E8");
            Imul(Reg32.ESP)
                .Test("imul esp", "F7-EC");
            Imul(new Addr32(Reg32.EAX))
                .Test("imul dword [eax]", "F7-28");
            Imul(new Addr32(Reg32.EBP))
                .Test("imul dword [ebp]", "F7-6D-00");
            Imul(new Addr32(Reg32.EBP, -4))
                .Test("imul dword [ebp-4]", "F7-6D-FC");
            Imul(new Addr32(Reg32.ESI, 0x1000))
                .Test("imul dword [esi+0x1000]", "F7-AE-00-10-00-00");
            #endregion

            #region Div
            Div(Reg32.EAX)
                .Test("div eax", "F7-F0");
            Div(Reg32.ESP)
                .Test("div esp", "F7-F4");
            Div(new Addr32(Reg32.EAX))
                .Test("div dword [eax]", "F7-30");
            Div(new Addr32(Reg32.EBP))
                .Test("div dword [ebp]", "F7-75-00");
            Div(new Addr32(Reg32.EBP, -4))
                .Test("div dword [ebp-4]", "F7-75-FC");
            Div(new Addr32(Reg32.ESI, 0x1000))
                .Test("div dword [esi+0x1000]", "F7-B6-00-10-00-00");
            #endregion

            #region Idiv
            Idiv(Reg32.EAX)
                .Test("idiv eax", "F7-F8");
            Idiv(Reg32.ESP)
                .Test("idiv esp", "F7-FC");
            Idiv(new Addr32(Reg32.EAX))
                .Test("idiv dword [eax]", "F7-38");
            Idiv(new Addr32(Reg32.EBP))
                .Test("idiv dword [ebp]", "F7-7D-00");
            Idiv(new Addr32(Reg32.EBP, -4))
                .Test("idiv dword [ebp-4]", "F7-7D-FC");
            Idiv(new Addr32(Reg32.ESI, 0x1000))
                .Test("idiv dword [esi+0x1000]", "F7-BE-00-10-00-00");
            #endregion

            #endregion

            #region Shl, Shr, Sal, Sar

            #region Shl
            Shl(Reg32.ECX, 1)
                .Test("shl ecx, 1", "D1-E1");
            Shl(Reg32.ECX, Reg8.CL)
                .Test("shl ecx, cl", "D3-E1");
            Shl(Reg32.EDX, 2)
                .Test("shl edx, 2", "C1-E2-02");
            Shl(new Addr32(Reg32.EBP, 4), 1)
                .Test("shl dword [ebp+4], 1", "D1-65-04");
            Shl(new Addr32(Reg32.EBP, 4), Reg8.CL)
                .Test("shl dword [ebp+4], cl", "D3-65-04");
            Shl(new Addr32(Reg32.EBP, 4), 8)
                .Test("shl dword [ebp+4], 8", "C1-65-04-08");
            #endregion

            #region Shr
            Shr(Reg32.ECX, 1)
                .Test("shr ecx, 1", "D1-E9");
            Shr(Reg32.ECX, Reg8.CL)
                .Test("shr ecx, cl", "D3-E9");
            Shr(Reg32.EDX, 2)
                .Test("shr edx, 2", "C1-EA-02");
            Shr(new Addr32(Reg32.EBP, 4), 1)
                .Test("shr dword [ebp+4], 1", "D1-6D-04");
            Shr(new Addr32(Reg32.EBP, 4), Reg8.CL)
                .Test("shr dword [ebp+4], cl", "D3-6D-04");
            Shr(new Addr32(Reg32.EBP, 4), 8)
                .Test("shr dword [ebp+4], 8", "C1-6D-04-08");
            #endregion

            #region Sal
            Sal(Reg32.ECX, 1)
                .Test("sal ecx, 1", "D1-E1");
            Sal(Reg32.ECX, Reg8.CL)
                .Test("sal ecx, cl", "D3-E1");
            Sal(Reg32.EDX, 2)
                .Test("sal edx, 2", "C1-E2-02");
            Sal(new Addr32(Reg32.EBP, 4), 1)
                .Test("sal dword [ebp+4], 1", "D1-65-04");
            Sal(new Addr32(Reg32.EBP, 4), Reg8.CL)
                .Test("sal dword [ebp+4], cl", "D3-65-04");
            Sal(new Addr32(Reg32.EBP, 4), 8)
                .Test("sal dword [ebp+4], 8", "C1-65-04-08");
            #endregion

            #region Sar
            Sar(Reg32.ECX, 1)
                .Test("sar ecx, 1", "D1-F9");
            Sar(Reg32.ECX, Reg8.CL)
                .Test("sar ecx, cl", "D3-F9");
            Sar(Reg32.EDX, 2)
                .Test("sar edx, 2", "C1-FA-02");
            Sar(new Addr32(Reg32.EBP, 4), 1)
                .Test("sar dword [ebp+4], 1", "D1-7D-04");
            Sar(new Addr32(Reg32.EBP, 4), Reg8.CL)
                .Test("sar dword [ebp+4], cl", "D3-7D-04");
            Sar(new Addr32(Reg32.EBP, 4), 8)
                .Test("sar dword [ebp+4], 8", "C1-7D-04-08");
            #endregion

            #endregion

            Call(new Addr32(Reg32.EAX))
                .Test("call [eax]", "FF-10");
            Call(new Addr32(0x12345678))
                .Test("call [0x12345678]", "FF-15-78-56-34-12");

            Jmp(new Addr32(Reg32.EAX))
                .Test("jmp [eax]", "FF-20");
            Jmp(new Addr32(0x12345678))
                .Test("jmp [0x12345678]", "FF-25-78-56-34-12");

            Lea(Reg32.EAX, new Addr32(0x12345678))
                .Test("lea eax, [0x12345678]", "8D-05-78-56-34-12");
            Lea(Reg32.EAX, new Addr32(Reg32.EDX, -4))
                .Test("lea eax, [edx-4]", "8D-42-FC");
            Lea(Reg32.EAX, new Addr32(Reg32.EBP, -4))
                .Test("lea eax, [ebp-4]", "8D-45-FC");

            Ret()
                .Test("ret", "C3");
            Ret(8)
                .Test("ret 8", "C2-08-00");

            Setcc(Cc.A, Reg8.AL)
                .Test("seta al", "0F-97-C0");
            Setcc(Cc.NZ, Reg8.AH)
                .Test("setnz ah", "0F-95-C4");
            Setcc(Cc.NG, Reg8.BH)
                .Test("setng bh", "0F-9E-C7");

            Enter(0x1234, 10)
                .Test("enter 0x1234, 10", "C8-34-12-0A");
            Leave()
                .Test("leave", "C9");
            Nop()
                .Test("nop", "90");
            Cdq()
                .Test("cdq", "99");
        }
    }
}
