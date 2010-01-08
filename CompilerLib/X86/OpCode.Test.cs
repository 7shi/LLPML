using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class OpCode
    {
        private static void Test(string mnemonic, string data, OpCode op)
        {
            string datastr = BitConverter.ToString(op.GetCodes());
            if (data != datastr)
            {
                throw new Exception(string.Format(
                    "[Unit test failed] {0}\r\n\tOK: {1}\r\n\tNG: {2}",
                    mnemonic, data, datastr));
            }
            //Console.WriteLine("OK: {0}: {1}", datastr, mnemonic);
        }

        public static void Test()
        {
            Test("add eax, ebx", "01-D8",
                I386.Add(Reg32.EAX, Reg32.EBX));
            Test("add esp, 4", "81-C4-04-00-00-00",
                I386.Add(Reg32.ESP, 4));
            Test("add [edx], eax", "01-02",
                I386.Add(new Addr32(Reg32.EDX), Reg32.EAX));
            Test("add [eax], ebx", "01-18",
                I386.Add(new Addr32(Reg32.EAX), Reg32.EBX));
            Test("add [esp], ecx", "01-0C-24",
                I386.Add(new Addr32(Reg32.ESP), Reg32.ECX));
            Test("add [eax+0x1000], ebp", "01-A8-00-10-00-00",
                I386.Add(new Addr32(Reg32.EAX, 0x1000), Reg32.EBP));
            Test("dec dword [eax]", "FF-08",
                I386.Dec(new Addr32(Reg32.EAX)));
            Test("dec dword [ebp]", "FF-4D-00",
                I386.Dec(new Addr32(Reg32.EBP)));
            Test("dec dword [ebp-4]", "FF-4D-FC",
                I386.Dec(new Addr32(Reg32.EBP, -4)));
            Test("dec dword [esi+0x1000]", "FF-8E-00-10-00-00",
                I386.Dec(new Addr32(Reg32.ESI, 0x1000)));
            Test("mov dword [esp], 1", "C7-04-24-01-00-00-00",
                I386.Mov(new Addr32(Reg32.ESP), 1));
            Test("mov dword [esp+4], 2", "C7-44-24-04-02-00-00-00",
                I386.Mov(new Addr32(Reg32.ESP, 4), 2));
            Test("mov dword [0x12345678], 0xabcdef", "C7-05-78-56-34-12-EF-CD-AB-00",
                I386.Mov(new Addr32(0x12345678), 0xabcdef));
            Test("test dword [ebp], 0x1234", "F7-45-00-34-12-00-00",
                I386.Test(new Addr32(Reg32.EBP), 0x1234));
            Test("test dword [0x12345678], 0xabcdef", "F7-05-78-56-34-12-EF-CD-AB-00",
                I386.Test(new Addr32(0x12345678), 0xabcdef));
            Test("push dword [eax]", "FF-30",
                I386.Push(new Addr32(Reg32.EAX)));
            Test("push dword [0x12345678]", "FF-35-78-56-34-12",
                I386.Push(new Addr32(0x12345678)));
            Test("pop dword [eax]", "8F-00",
                I386.Pop(new Addr32(Reg32.EAX)));
            Test("pop dword [0x12345678]", "8F-05-78-56-34-12",
                I386.Pop(new Addr32(0x12345678)));
            Test("call [eax]", "FF-10",
                I386.Call(new Addr32(Reg32.EAX)));
            Test("call [0x12345678]", "FF-15-78-56-34-12",
                I386.Call(new Addr32(0x12345678)));
            Test("mov eax, [0x12345678]", "A1-78-56-34-12",
                I386.Mov(Reg32.EAX, new Addr32(0x12345678)));
            Test("mov ebx, [0x12345678]", "8B-1D-78-56-34-12",
                I386.Mov(Reg32.EBX, new Addr32(0x12345678)));
            Test("mov eax, [esp]", "8B-04-24",
                I386.Mov(Reg32.EAX, new Addr32(Reg32.ESP)));
            Test("mov [0x12345678], eax", "A3-78-56-34-12",
                I386.Mov(new Addr32(0x12345678), Reg32.EAX));
            Test("mov [0x12345678], ebx", "89-1D-78-56-34-12",
                I386.Mov(new Addr32(0x12345678), Reg32.EBX));
            Test("mov [esp], eax", "89-04-24",
                I386.Mov(new Addr32(Reg32.ESP), Reg32.EAX));
        }
    }
}
