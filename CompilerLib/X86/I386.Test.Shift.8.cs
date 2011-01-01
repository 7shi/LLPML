using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        public static void TestShift_8()
        {
            // Shl, Shr, Sal, Sar

            // Shl
            ShlB(Reg8.CL, 1)
                .Test("shl cl, 1", "D0-E1");
            ShlBR(Reg8.CL, Reg8.CL)
                .Test("shl cl, cl", "D2-E1");
            ShlB(Reg8.DL, 2)
                .Test("shl dl, 2", "C0-E2-02");
            ShlBA(Addr32.NewRO(Reg32.EBP, 4), 1)
                .Test("shl byte [ebp+4], 1", "D0-65-04");
            ShlBAR(Addr32.NewRO(Reg32.EBP, 4), Reg8.CL)
                .Test("shl byte [ebp+4], cl", "D2-65-04");
            ShlBA(Addr32.NewRO(Reg32.EBP, 4), 8)
                .Test("shl byte [ebp+4], 8", "C0-65-04-08");

            // Shr
            ShrB(Reg8.CL, 1)
                .Test("shr cl, 1", "D0-E9");
            ShrBR(Reg8.CL, Reg8.CL)
                .Test("shr cl, cl", "D2-E9");
            ShrB(Reg8.DL, 2)
                .Test("shr dl, 2", "C0-EA-02");
            ShrBA(Addr32.NewRO(Reg32.EBP, 4), 1)
                .Test("shr byte [ebp+4], 1", "D0-6D-04");
            ShrBAR(Addr32.NewRO(Reg32.EBP, 4), Reg8.CL)
                .Test("shr byte [ebp+4], cl", "D2-6D-04");
            ShrBA(Addr32.NewRO(Reg32.EBP, 4), 8)
                .Test("shr byte [ebp+4], 8", "C0-6D-04-08");

            // Sal
            SalB(Reg8.CL, 1)
                .Test("sal cl, 1", "D0-E1");
            SalBR(Reg8.CL, Reg8.CL)
                .Test("sal cl, cl", "D2-E1");
            SalB(Reg8.DL, 2)
                .Test("sal dl, 2", "C0-E2-02");
            SalBA(Addr32.NewRO(Reg32.EBP, 4), 1)
                .Test("sal byte [ebp+4], 1", "D0-65-04");
            SalBAR(Addr32.NewRO(Reg32.EBP, 4), Reg8.CL)
                .Test("sal byte [ebp+4], cl", "D2-65-04");
            SalBA(Addr32.NewRO(Reg32.EBP, 4), 8)
                .Test("sal byte [ebp+4], 8", "C0-65-04-08");

            // Sar
            SarB(Reg8.CL, 1)
                .Test("sar cl, 1", "D0-F9");
            SarBR(Reg8.CL, Reg8.CL)
                .Test("sar cl, cl", "D2-F9");
            SarB(Reg8.DL, 2)
                .Test("sar dl, 2", "C0-FA-02");
            SarBA(Addr32.NewRO(Reg32.EBP, 4), 1)
                .Test("sar byte [ebp+4], 1", "D0-7D-04");
            SarBAR(Addr32.NewRO(Reg32.EBP, 4), Reg8.CL)
                .Test("sar byte [ebp+4], cl", "D2-7D-04");
            SarBA(Addr32.NewRO(Reg32.EBP, 4), 8)
                .Test("sar byte [ebp+4], 8", "C0-7D-04-08");
        }
    }
}
