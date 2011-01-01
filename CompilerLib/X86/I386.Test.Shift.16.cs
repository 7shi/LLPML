using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        public static void TestShift_16()
        {
            // Shl, Shr, Sal, Sar

            // Shl
            ShlW(Reg16.CX, 1)
                .Test("shl cx, 1", "66-D1-E1");
            ShlWR(Reg16.CX, Reg8.CL)
                .Test("shl cx, cl", "66-D3-E1");
            ShlW(Reg16.DX, 2)
                .Test("shl dx, 2", "66-C1-E2-02");
            ShlWA(Addr32.NewRO(Reg32.EBP, 4), 1)
                .Test("shl word [ebp+4], 1", "66-D1-65-04");
            ShlWAR(Addr32.NewRO(Reg32.EBP, 4), Reg8.CL)
                .Test("shl word [ebp+4], cl", "66-D3-65-04");
            ShlWA(Addr32.NewRO(Reg32.EBP, 4), 8)
                .Test("shl word [ebp+4], 8", "66-C1-65-04-08");

            // Shr
            ShrW(Reg16.CX, 1)
                .Test("shr cx, 1", "66-D1-E9");
            ShrWR(Reg16.CX, Reg8.CL)
                .Test("shr cx, cl", "66-D3-E9");
            ShrW(Reg16.DX, 2)
                .Test("shr dx, 2", "66-C1-EA-02");
            ShrWA(Addr32.NewRO(Reg32.EBP, 4), 1)
                .Test("shr word [ebp+4], 1", "66-D1-6D-04");
            ShrWAR(Addr32.NewRO(Reg32.EBP, 4), Reg8.CL)
                .Test("shr word [ebp+4], cl", "66-D3-6D-04");
            ShrWA(Addr32.NewRO(Reg32.EBP, 4), 8)
                .Test("shr word [ebp+4], 8", "66-C1-6D-04-08");

            // Sal
            SalW(Reg16.CX, 1)
                .Test("sal cx, 1", "66-D1-E1");
            SalWR(Reg16.CX, Reg8.CL)
                .Test("sal cx, cl", "66-D3-E1");
            SalW(Reg16.DX, 2)
                .Test("sal dx, 2", "66-C1-E2-02");
            SalWA(Addr32.NewRO(Reg32.EBP, 4), 1)
                .Test("sal word [ebp+4], 1", "66-D1-65-04");
            SalWAR(Addr32.NewRO(Reg32.EBP, 4), Reg8.CL)
                .Test("sal word [ebp+4], cl", "66-D3-65-04");
            SalWA(Addr32.NewRO(Reg32.EBP, 4), 8)
                .Test("sal word [ebp+4], 8", "66-C1-65-04-08");

            // Sar
            SarW(Reg16.CX, 1)
                .Test("sar cx, 1", "66-D1-F9");
            SarWR(Reg16.CX, Reg8.CL)
                .Test("sar cx, cl", "66-D3-F9");
            SarW(Reg16.DX, 2)
                .Test("sar dx, 2", "66-C1-FA-02");
            SarWA(Addr32.NewRO(Reg32.EBP, 4), 1)
                .Test("sar word [ebp+4], 1", "66-D1-7D-04");
            SarWAR(Addr32.NewRO(Reg32.EBP, 4), Reg8.CL)
                .Test("sar word [ebp+4], cl", "66-D3-7D-04");
            SarWA(Addr32.NewRO(Reg32.EBP, 4), 8)
                .Test("sar word [ebp+4], 8", "66-C1-7D-04-08");
        }
    }
}
