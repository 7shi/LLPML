using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        public static void TestShift_32()
        {
            // Shl, Shr, Sal, Sar

            // Shl
            Shl(Reg32.ECX, 1)
                .Test("shl ecx, 1", "D1-E1");
            ShlR(Reg32.ECX, Reg8.CL)
                .Test("shl ecx, cl", "D3-E1");
            Shl(Reg32.EDX, 2)
                .Test("shl edx, 2", "C1-E2-02");
            ShlA(Addr32.NewRO(Reg32.EBP, 4), 1)
                .Test("shl dword [ebp+4], 1", "D1-65-04");
            ShlAR(Addr32.NewRO(Reg32.EBP, 4), Reg8.CL)
                .Test("shl dword [ebp+4], cl", "D3-65-04");
            ShlA(Addr32.NewRO(Reg32.EBP, 4), 8)
                .Test("shl dword [ebp+4], 8", "C1-65-04-08");

            // Shr
            Shr(Reg32.ECX, 1)
                .Test("shr ecx, 1", "D1-E9");
            ShrR(Reg32.ECX, Reg8.CL)
                .Test("shr ecx, cl", "D3-E9");
            Shr(Reg32.EDX, 2)
                .Test("shr edx, 2", "C1-EA-02");
            ShrA(Addr32.NewRO(Reg32.EBP, 4), 1)
                .Test("shr dword [ebp+4], 1", "D1-6D-04");
            ShrAR(Addr32.NewRO(Reg32.EBP, 4), Reg8.CL)
                .Test("shr dword [ebp+4], cl", "D3-6D-04");
            ShrA(Addr32.NewRO(Reg32.EBP, 4), 8)
                .Test("shr dword [ebp+4], 8", "C1-6D-04-08");

            // Sal
            Sal(Reg32.ECX, 1)
                .Test("sal ecx, 1", "D1-E1");
            SalR(Reg32.ECX, Reg8.CL)
                .Test("sal ecx, cl", "D3-E1");
            Sal(Reg32.EDX, 2)
                .Test("sal edx, 2", "C1-E2-02");
            SalA(Addr32.NewRO(Reg32.EBP, 4), 1)
                .Test("sal dword [ebp+4], 1", "D1-65-04");
            SalAR(Addr32.NewRO(Reg32.EBP, 4), Reg8.CL)
                .Test("sal dword [ebp+4], cl", "D3-65-04");
            SalA(Addr32.NewRO(Reg32.EBP, 4), 8)
                .Test("sal dword [ebp+4], 8", "C1-65-04-08");

            // Sar
            Sar(Reg32.ECX, 1)
                .Test("sar ecx, 1", "D1-F9");
            SarR(Reg32.ECX, Reg8.CL)
                .Test("sar ecx, cl", "D3-F9");
            Sar(Reg32.EDX, 2)
                .Test("sar edx, 2", "C1-FA-02");
            SarA(Addr32.NewRO(Reg32.EBP, 4), 1)
                .Test("sar dword [ebp+4], 1", "D1-7D-04");
            SarAR(Addr32.NewRO(Reg32.EBP, 4), Reg8.CL)
                .Test("sar dword [ebp+4], cl", "D3-7D-04");
            SarA(Addr32.NewRO(Reg32.EBP, 4), 8)
                .Test("sar dword [ebp+4], 8", "C1-7D-04-08");
        }
    }
}
