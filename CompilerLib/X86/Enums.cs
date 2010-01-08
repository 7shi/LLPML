using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.X86
{
    public enum CallType { CDecl, Std };

    public enum Reg32
    {
        EAX = 0, ECX, EDX, EBX, ESP, EBP, ESI, EDI
    }

    public enum Reg16
    {
        AX = 0, CX, DX, BX, SP, BP, SI, DI
    }

    public enum Reg8
    {
        AL = 0, CL, DL, BL, AH, CH, DH, BH
    }

    public enum SegReg
    {
        SS, CS, DS, ES
    }
}
