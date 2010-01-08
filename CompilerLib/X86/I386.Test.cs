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
            I386.Add(Reg32.EAX, Reg32.EBX)
                .Test("add eax, ebx", "01-D8");
            I386.Add(Reg32.ESP, 4)
                .Test("add esp, 4", "81-C4-04-00-00-00");
            I386.Add(new Addr32(Reg32.EDX), Reg32.EAX)
                .Test("add [edx], eax", "01-02");
            I386.Add(new Addr32(Reg32.EAX), Reg32.EBX)
                .Test("add [eax], ebx", "01-18");
            I386.Add(new Addr32(Reg32.ESP), Reg32.ECX)
                .Test("add [esp], ecx", "01-0C-24");
            I386.Add(new Addr32(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("add [eax+0x1000], ebp", "01-A8-00-10-00-00");
            I386.Sub(Reg32.EAX, Reg32.EBX)
                .Test("sub eax, ebx", "29-D8");
            I386.Sub(Reg32.ESP, 4)
                .Test("sub esp, 4", "81-EC-04-00-00-00");
            I386.Sub(new Addr32(Reg32.EDX), Reg32.EAX)
                .Test("sub [edx], eax", "29-02");
            I386.Sub(new Addr32(Reg32.EAX), Reg32.EBX)
                .Test("sub [eax], ebx", "29-18");
            I386.Sub(new Addr32(Reg32.ESP), Reg32.ECX)
                .Test("sub [esp], ecx", "29-0C-24");
            I386.Sub(new Addr32(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("sub [eax+0x1000], ebp", "29-A8-00-10-00-00");
            I386.Dec(new Addr32(Reg32.EAX))
                .Test("dec dword [eax]", "FF-08");
            I386.Dec(new Addr32(Reg32.EBP))
                .Test("dec dword [ebp]", "FF-4D-00");
            I386.Dec(new Addr32(Reg32.EBP, -4))
                .Test("dec dword [ebp-4]", "FF-4D-FC");
            I386.Dec(new Addr32(Reg32.ESI, 0x1000))
                .Test("dec dword [esi+0x1000]", "FF-8E-00-10-00-00");
            I386.Mov(new Addr32(Reg32.ESP), 1)
                .Test("mov dword [esp], 1", "C7-04-24-01-00-00-00");
            I386.Mov(new Addr32(Reg32.ESP, 4), 2)
                .Test("mov dword [esp+4], 2", "C7-44-24-04-02-00-00-00");
            I386.Mov(new Addr32(0x12345678), 0xabcdef)
                .Test("mov dword [0x12345678], 0xabcdef", "C7-05-78-56-34-12-EF-CD-AB-00");
            I386.Test(new Addr32(Reg32.EBP), 0x1234)
                .Test("test dword [ebp], 0x1234", "F7-45-00-34-12-00-00");
            I386.Test(new Addr32(0x12345678), 0xabcdef)
                .Test("test dword [0x12345678], 0xabcdef", "F7-05-78-56-34-12-EF-CD-AB-00");
            I386.Push(new Addr32(Reg32.EAX))
                .Test("push dword [eax]", "FF-30");
            I386.Push(new Addr32(0x12345678))
                .Test("push dword [0x12345678]", "FF-35-78-56-34-12");
            I386.Pop(new Addr32(Reg32.EAX))
                .Test("pop dword [eax]", "8F-00");
            I386.Pop(new Addr32(0x12345678))
                .Test("pop dword [0x12345678]", "8F-05-78-56-34-12");
            I386.Call(new Addr32(Reg32.EAX))
                .Test("call [eax]", "FF-10");
            I386.Call(new Addr32(0x12345678))
                .Test("call [0x12345678]", "FF-15-78-56-34-12");
            I386.Mov(Reg32.EAX, new Addr32(0x12345678))
                .Test("mov eax, [0x12345678]", "A1-78-56-34-12");
            I386.Mov(Reg32.EBX, new Addr32(0x12345678))
                .Test("mov ebx, [0x12345678]", "8B-1D-78-56-34-12");
            I386.Mov(Reg32.EAX, new Addr32(Reg32.ESP))
                .Test("mov eax, [esp]", "8B-04-24");
            I386.Mov(new Addr32(0x12345678), Reg32.EAX)
                .Test("mov [0x12345678], eax", "A3-78-56-34-12");
            I386.Mov(new Addr32(0x12345678), Reg32.EBX)
                .Test("mov [0x12345678], ebx", "89-1D-78-56-34-12");
            I386.Mov(new Addr32(Reg32.ESP), Reg32.EAX)
                .Test("mov [esp], eax", "89-04-24");
            I386.Enter(0x1234, 10)
                .Test("enter 0x1234, 10", "C8-34-12-0A");
            I386.Leave()
                .Test("leave", "C9");
            I386.Nop()
                .Test("nop", "90");
            I386.Mov(new Addr32(Reg32.EDX, -4), Reg32.EAX)
                .Test("mov [edx-4], eax", "89-42-FC");
            I386.Mov(new Addr32(Reg32.EBP, -4), Reg32.EAX)
                .Test("mov [ebp-4], eax", "89-45-FC");
            I386.Mov(Reg32.EAX, new Addr32(Reg32.EDX, -4))
                .Test("mov eax, [edx-4]", "8B-42-FC");
            I386.Mov(Reg32.EAX, new Addr32(Reg32.EBP, -4))
                .Test("mov eax, [ebp-4]", "8B-45-FC");
            I386.Lea(Reg32.EAX, new Addr32(0x12345678))
                .Test("lea eax, [0x12345678]", "8D-05-78-56-34-12");
            I386.Lea(Reg32.EAX, new Addr32(Reg32.EDX, -4))
                .Test("lea eax, [edx-4]", "8D-42-FC");
            I386.Lea(Reg32.EAX, new Addr32(Reg32.EBP, -4))
                .Test("lea eax, [ebp-4]", "8D-45-FC");
            I386.Inc(new Addr32(Reg32.EAX))
                .Test("inc dword [eax]", "FF-00");
            I386.Dec(new Addr32(Reg32.EAX))
                .Test("dec dword [eax]", "FF-08");
            I386.Add(new Addr32(Reg32.EAX), 1)
                .Test("add dword [eax], 1", "81-00-01-00-00-00");
            I386.Add(new Addr32(Reg32.EBP, -4), 8)
                .Test("add dword [ebp-4], 8", "81-45-FC-08-00-00-00");
            I386.Sub(new Addr32(Reg32.EAX), 1)
                .Test("sub dword [eax], 1", "81-28-01-00-00-00");
            I386.Sub(new Addr32(Reg32.EBP, -4), 8)
                .Test("sub dword [ebp-4], 8", "81-6D-FC-08-00-00-00");
            I386.Cmp(Reg32.EAX, 4)
                .Test("cmp eax, 4", "3D-04-00-00-00");
            I386.Cmp(Reg32.ESP, 4)
                .Test("cmp esp, 4", "81-FC-04-00-00-00");
            I386.Cmp(Reg32.EAX, Reg32.EBX)
                .Test("cmp eax, ebx", "39-D8");
            I386.Cmp(new Addr32(Reg32.EDX), Reg32.EAX)
                .Test("cmp [edx], eax", "39-02");
            I386.Cmp(new Addr32(Reg32.EAX), Reg32.EBX)
                .Test("cmp [eax], ebx", "39-18");
            I386.Cmp(new Addr32(Reg32.ESP), Reg32.ECX)
                .Test("cmp [esp], ecx", "39-0C-24");
            I386.Cmp(new Addr32(Reg32.EAX, 0x1000), Reg32.EBP)
                .Test("cmp [eax+0x1000], ebp", "39-A8-00-10-00-00");
            I386.Cmp(new Addr32(Reg32.EAX), 1)
                .Test("cmp dword [eax], 1", "81-38-01-00-00-00");
            I386.Cmp(new Addr32(Reg32.EBP, -4), 8)
                .Test("cmp dword [ebp-4], 8", "81-7D-FC-08-00-00-00");
            I386.Add(Reg32.EAX, 4)
                .Test("add eax, 4", "05-04-00-00-00");
            I386.Sub(Reg32.EAX, 4)
                .Test("sub eax, 4", "2D-04-00-00-00");
        }
    }
}
