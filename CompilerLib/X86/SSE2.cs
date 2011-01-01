using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class SSE2
    {
        public static OpCode MovD(Xmm op1, Reg32 op2) { return FromNameD("movd", op1, op2); }
        public static OpCode MovDA(Xmm op1, Addr32 op2) { return FromNameA("movd", op1, op2); }
        public static OpCode MovDR(Reg32 op1, Xmm op2) { return FromNameR("movd", op1, op2); }
        public static OpCode MovDAX(Addr32 op1, Xmm op2) { return FromNameAX("movd", op1, op2); }

        public static OpCode MovQ(Xmm op1, Xmm op2) { return FromName("movq", op1, op2); }
        public static OpCode MovQA(Xmm op1, Addr32 op2) { return FromNameA("movq", op1, op2); }
        public static OpCode MovQAX(Addr32 op1, Xmm op2) { return FromNameAX("movq", op1, op2); }

        public static OpCode MovDQA(Xmm op1, Xmm op2) { return FromName("movdqa", op1, op2); }
        public static OpCode MovDQAA(Xmm op1, Addr32 op2) { return FromNameA("movdqa", op1, op2); }
        public static OpCode MovDQAAX(Addr32 op1, Xmm op2) { return FromNameAX("movdqa", op1, op2); }

        public static OpCode MovDQU(Xmm op1, Xmm op2) { return FromName("movdqu", op1, op2); }
        public static OpCode MovDQUA(Xmm op1, Addr32 op2) { return FromNameA("movdqu", op1, op2); }
        public static OpCode MovDQUAX(Addr32 op1, Xmm op2) { return FromNameAX("movdqu", op1, op2); }

        public static OpCode PAddB(Xmm op1, Xmm op2) { return FromName("paddb", op1, op2); }
        public static OpCode PAddBA(Xmm op1, Addr32 op2) { return FromNameA("paddb", op1, op2); }
        public static OpCode PAddW(Xmm op1, Xmm op2) { return FromName("paddw", op1, op2); }
        public static OpCode PAddWA(Xmm op1, Addr32 op2) { return FromNameA("paddw", op1, op2); }
        public static OpCode PAddD(Xmm op1, Xmm op2) { return FromName("paddd", op1, op2); }
        public static OpCode PAddDA(Xmm op1, Addr32 op2) { return FromNameA("paddd", op1, op2); }
        public static OpCode PAddQ(Xmm op1, Xmm op2) { return FromName("paddq", op1, op2); }
        public static OpCode PAddQA(Xmm op1, Addr32 op2) { return FromNameA("paddq", op1, op2); }

        public static OpCode PSubB(Xmm op1, Xmm op2) { return FromName("psubb", op1, op2); }
        public static OpCode PSubBA(Xmm op1, Addr32 op2) { return FromNameA("psubb", op1, op2); }
        public static OpCode PSubW(Xmm op1, Xmm op2) { return FromName("psubw", op1, op2); }
        public static OpCode PSubWA(Xmm op1, Addr32 op2) { return FromNameA("psubw", op1, op2); }
        public static OpCode PSubD(Xmm op1, Xmm op2) { return FromName("psubd", op1, op2); }
        public static OpCode PSubDA(Xmm op1, Addr32 op2) { return FromNameA("psubd", op1, op2); }
        public static OpCode PSubQ(Xmm op1, Xmm op2) { return FromName("psubq", op1, op2); }
        public static OpCode PSubQA(Xmm op1, Addr32 op2) { return FromNameA("psubq", op1, op2); }

        public static OpCode PMulHW(Xmm op1, Xmm op2) { return FromName("pmulhw", op1, op2); }
        public static OpCode PMulHWA(Xmm op1, Addr32 op2) { return FromNameA("pmulhw", op1, op2); }
        public static OpCode PMulLW(Xmm op1, Xmm op2) { return FromName("pmullw", op1, op2); }
        public static OpCode PMulLWA(Xmm op1, Addr32 op2) { return FromNameA("pmullw", op1, op2); }

        public static OpCode PSLLW(Xmm op1, Xmm op2) { return FromName("psllw", op1, op2); }
        public static OpCode PSLLWA(Xmm op1, Addr32 op2) { return FromNameA("psllw", op1, op2); }
        public static OpCode PSLLWB(Xmm op1, byte op2) { return FromNameB("psllw", op1, op2); }
        public static OpCode PSLLD(Xmm op1, Xmm op2) { return FromName("pslld", op1, op2); }
        public static OpCode PSLLDA(Xmm op1, Addr32 op2) { return FromNameA("pslld", op1, op2); }
        public static OpCode PSLLDB(Xmm op1, byte op2) { return FromNameB("pslld", op1, op2); }
        public static OpCode PSLLQ(Xmm op1, Xmm op2) { return FromName("psllq", op1, op2); }
        public static OpCode PSLLQA(Xmm op1, Addr32 op2) { return FromNameA("psllq", op1, op2); }
        public static OpCode PSLLQB(Xmm op1, byte op2) { return FromNameB("psllq", op1, op2); }

        public static OpCode PSRLW(Xmm op1, Xmm op2) { return FromName("psrlw", op1, op2); }
        public static OpCode PSRLWA(Xmm op1, Addr32 op2) { return FromNameA("psrlw", op1, op2); }
        public static OpCode PSRLWB(Xmm op1, byte op2) { return FromNameB("psrlw", op1, op2); }
        public static OpCode PSRLD(Xmm op1, Xmm op2) { return FromName("psrld", op1, op2); }
        public static OpCode PSRLDA(Xmm op1, Addr32 op2) { return FromNameA("psrld", op1, op2); }
        public static OpCode PSRLDB(Xmm op1, byte op2) { return FromNameB("psrld", op1, op2); }
        public static OpCode PSRLQ(Xmm op1, Xmm op2) { return FromName("psrlq", op1, op2); }
        public static OpCode PSRLQA(Xmm op1, Addr32 op2) { return FromNameA("psrlq", op1, op2); }
        public static OpCode PSRLQB(Xmm op1, byte op2) { return FromNameB("psrlq", op1, op2); }

        public static OpCode PSRAW(Xmm op1, Xmm op2) { return FromName("psraw", op1, op2); }
        public static OpCode PSRAWA(Xmm op1, Addr32 op2) { return FromNameA("psraw", op1, op2); }
        public static OpCode PSRAWB(Xmm op1, byte op2) { return FromNameB("psraw", op1, op2); }
        public static OpCode PSRAD(Xmm op1, Xmm op2) { return FromName("psrad", op1, op2); }
        public static OpCode PSRADA(Xmm op1, Addr32 op2) { return FromNameA("psrad", op1, op2); }
        public static OpCode PSRADB(Xmm op1, byte op2) { return FromNameB("psrad", op1, op2); }

        public static OpCode PUnpckHBW(Xmm op1, Xmm op2) { return FromName("punpckhbw", op1, op2); }
        public static OpCode PUnpckHBWA(Xmm op1, Addr32 op2) { return FromNameA("punpckhbw", op1, op2); }
        public static OpCode PUnpckHWD(Xmm op1, Xmm op2) { return FromName("punpckhwd", op1, op2); }
        public static OpCode PUnpckHWDA(Xmm op1, Addr32 op2) { return FromNameA("punpckhwd", op1, op2); }
        public static OpCode PUnpckHDQ(Xmm op1, Xmm op2) { return FromName("punpckhdq", op1, op2); }
        public static OpCode PUnpckHDQA(Xmm op1, Addr32 op2) { return FromNameA("punpckhdq", op1, op2); }

        public static OpCode PUnpckLBW(Xmm op1, Xmm op2) { return FromName("punpcklbw", op1, op2); }
        public static OpCode PUnpckLBWA(Xmm op1, Addr32 op2) { return FromNameA("punpcklbw", op1, op2); }
        public static OpCode PUnpckLWD(Xmm op1, Xmm op2) { return FromName("punpcklwd", op1, op2); }
        public static OpCode PUnpckLWDA(Xmm op1, Addr32 op2) { return FromNameA("punpcklwd", op1, op2); }
        public static OpCode PUnpckLDQ(Xmm op1, Xmm op2) { return FromName("punpckldq", op1, op2); }
        public static OpCode PUnpckLDQA(Xmm op1, Addr32 op2) { return FromNameA("punpckldq", op1, op2); }

        public static OpCode PackSSWB(Xmm op1, Xmm op2) { return FromName("packsswb", op1, op2); }
        public static OpCode PackSSWBA(Xmm op1, Addr32 op2) { return FromNameA("packsswb", op1, op2); }
        public static OpCode PackSSDW(Xmm op1, Xmm op2) { return FromName("packssdw", op1, op2); }
        public static OpCode PackSSDWA(Xmm op1, Addr32 op2) { return FromNameA("packssdw", op1, op2); }
        public static OpCode PackUSWB(Xmm op1, Xmm op2) { return FromName("packuswb", op1, op2); }
        public static OpCode PackUSWBA(Xmm op1, Addr32 op2) { return FromNameA("packuswb", op1, op2); }

        private static byte GetCode(string op)
        {
            switch (op)
            {
                case "movq":
                    return 0x7e;
                case "movdqa":
                case "movdqu":
                    return 0x6f;
                case "paddb":
                    return 0xfc;
                case "paddw":
                    return 0xfd;
                case "paddd":
                    return 0xfe;
                case "paddq":
                    return 0xd4;
                case "psubb":
                    return 0xf8;
                case "psubw":
                    return 0xf9;
                case "psubd":
                    return 0xfa;
                case "psubq":
                    return 0xfb;
                case "pmulhw":
                    return 0xe5;
                case "pmullw":
                    return 0xd5;
                case "psllw":
                    return 0xf1;
                case "pslld":
                    return 0xf2;
                case "psllq":
                    return 0xf3;
                case "psrlw":
                    return 0xd1;
                case "psrld":
                    return 0xd2;
                case "psrlq":
                    return 0xd3;
                case "psraw":
                    return 0xe1;
                case "psrad":
                    return 0xe2;
                case "punpckhbw":
                    return 0x68;
                case "punpckhwd":
                    return 0x69;
                case "punpckhdq":
                    return 0x6a;
                case "punpcklbw":
                    return 0x60;
                case "punpcklwd":
                    return 0x61;
                case "punpckldq":
                    return 0x62;
                case "packsswb":
                    return 0x63;
                case "packssdw":
                    return 0x6b;
                case "packuswb":
                    return 0x67;
                default:
                    throw new Exception("invalid operator: " + op);
            }
        }

        public static OpCode FromName(string op, Xmm op1, Xmm op2)
        {
            byte b1 = 0x66, b2 = GetCode(op);
            if (op == "movq" || op == "movdqu") b1 = 0xf3;
            return OpCode.NewBytes(Util.GetBytes4(b1, 0x0f, b2, (byte)(0xc0 + (((int)op1) << 3) + op2)));
        }

        public static OpCode FromNameA(string op, Xmm op1, Addr32 op2)
        {
            byte b1 = 0x66, b2;
            if (op == "movq" || op == "movdqu") b1 = 0xf3;
            switch (op)
            {
                case "movd":
                    b2 = 0x6e;
                    break;
                default:
                    b2 = GetCode(op);
                    break;
            }
            return OpCode.NewA(Util.GetBytes3(b1, 0x0f, b2), Addr32.NewAdM(op2, (byte)op1));
        }

        public static OpCode FromNameAX(string op, Addr32 op1, Xmm op2)
        {
            byte b1 = 0x66, b2;
            if (op == "movdqu") b1 = 0xf3;
            switch (op)
            {
                case "movd":
                    b2 = 0x7e;
                    break;
                case "movq":
                    b2 = 0xd6;
                    break;
                case "movdqa":
                case "movdqu":
                    b2 = 0x7f;
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            return OpCode.NewA(Util.GetBytes3(b1, 0x0f, b2), Addr32.NewAdM(op1, (byte)op2));
        }

        public static OpCode FromNameD(string op, Xmm op1, Reg32 op2)
        {
            byte b;
            switch (op)
            {
                case "movd":
                    b = 0x6e;
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            return OpCode.NewBytes(Util.GetBytes4(0x66, 0x0f, b, (byte)(0xc0 + (((int)op1) << 3) + op2)));
        }

        public static OpCode FromNameR(string op, Reg32 op1, Xmm op2)
        {
            byte b;
            switch (op)
            {
                case "movd":
                    b = 0x7e;
                    break;
                default:
                    throw new Exception("invalid operator: " + op);
            }
            return OpCode.NewBytes(Util.GetBytes4(0x66, 0x0f, b, (byte)(0xc0 + (((int)op2) << 3) + op1)));
        }

        public static OpCode FromNameB(string op, Xmm op1, byte op2)
        {
            byte b1 = 0, b2 = 0;
            switch (op)
            {
                case "psllw":
                case "psrlw":
                case "psraw":
                    b1 = 0x71;
                    break;
                case "pslld":
                case "psrld":
                case "psrad":
                    b1 = 0x72;
                    break;
                case "psllq":
                case "psrlq":
                    b1 = 0x73;
                    break;
            }
            switch (op)
            {
                case "psllw":
                case "pslld":
                case "psllq":
                    b2 = 0xf0;
                    break;
                case "psrlw":
                case "psrld":
                case "psrlq":
                    b2 = 0xd0;
                    break;
                case "psraw":
                case "psrad":
                    b2 = 0xe0;
                    break;
            }
            if (b1 == 0 || b2 == 0)
                throw new Exception("invalid operator: " + op);
            return OpCode.NewB(Util.GetBytes4(0x66, 0x0f, b1, (byte)(b2 + op1)), op2);
        }
    }
}
