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
            Shl(Reg16.CX, 1)
                .Test("shl cx, 1", "66-D1-E1");
            Shl(Reg16.CX, Reg8.CL)
                .Test("shl cx, cl", "66-D3-E1");
            Shl(Reg16.DX, 2)
                .Test("shl dx, 2", "66-C1-E2-02");
            ShlW(new Addr32(Reg32.EBP, 4), 1)
                .Test("shl word [ebp+4], 1", "66-D1-65-04");
            ShlW(new Addr32(Reg32.EBP, 4), Reg8.CL)
                .Test("shl word [ebp+4], cl", "66-D3-65-04");
            ShlW(new Addr32(Reg32.EBP, 4), 8)
                .Test("shl word [ebp+4], 8", "66-C1-65-04-08");

            // Shr
            Shr(Reg16.CX, 1)
                .Test("shr cx, 1", "66-D1-E9");
            Shr(Reg16.CX, Reg8.CL)
                .Test("shr cx, cl", "66-D3-E9");
            Shr(Reg16.DX, 2)
                .Test("shr dx, 2", "66-C1-EA-02");
            ShrW(new Addr32(Reg32.EBP, 4), 1)
                .Test("shr word [ebp+4], 1", "66-D1-6D-04");
            ShrW(new Addr32(Reg32.EBP, 4), Reg8.CL)
                .Test("shr word [ebp+4], cl", "66-D3-6D-04");
            ShrW(new Addr32(Reg32.EBP, 4), 8)
                .Test("shr word [ebp+4], 8", "66-C1-6D-04-08");

            // Sal
            Sal(Reg16.CX, 1)
                .Test("sal cx, 1", "66-D1-E1");
            Sal(Reg16.CX, Reg8.CL)
                .Test("sal cx, cl", "66-D3-E1");
            Sal(Reg16.DX, 2)
                .Test("sal dx, 2", "66-C1-E2-02");
            SalW(new Addr32(Reg32.EBP, 4), 1)
                .Test("sal word [ebp+4], 1", "66-D1-65-04");
            SalW(new Addr32(Reg32.EBP, 4), Reg8.CL)
                .Test("sal word [ebp+4], cl", "66-D3-65-04");
            SalW(new Addr32(Reg32.EBP, 4), 8)
                .Test("sal word [ebp+4], 8", "66-C1-65-04-08");

            // Sar
            Sar(Reg16.CX, 1)
                .Test("sar cx, 1", "66-D1-F9");
            Sar(Reg16.CX, Reg8.CL)
                .Test("sar cx, cl", "66-D3-F9");
            Sar(Reg16.DX, 2)
                .Test("sar dx, 2", "66-C1-FA-02");
            SarW(new Addr32(Reg32.EBP, 4), 1)
                .Test("sar word [ebp+4], 1", "66-D1-7D-04");
            SarW(new Addr32(Reg32.EBP, 4), Reg8.CL)
                .Test("sar word [ebp+4], cl", "66-D3-7D-04");
            SarW(new Addr32(Reg32.EBP, 4), 8)
                .Test("sar word [ebp+4], 8", "66-C1-7D-04-08");
        }
    }
}
