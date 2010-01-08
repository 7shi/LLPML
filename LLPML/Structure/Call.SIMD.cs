using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Call
    {
        public bool AddSIMDCodes(OpModule codes, List<IIntValue> args)
        {
            switch (name)
            {
                case "__emms":
                    if (args.Count != 0)
                        throw Abort("{0}: argument mismatched", name);
                    __emms(codes);
                    return true;
                case "__movd":
                    if (args.Count != 2)
                        throw Abort("{0}: argument mismatched", name);
                    __movd(codes, args[0], args[1]);
                    return true;
                case "__movq":
                    if (args.Count != 2)
                        throw Abort("{0}: argument mismatched", name);
                    __movq(codes, args[0], args[1]);
                    return true;
                case "__movdqa":
                case "__movdqu":
                    if (args.Count != 2)
                        throw Abort("{0}: argument mismatched", name);
                    __movdq(codes, name, args[0], args[1]);
                    return true;
                case "__paddb":
                case "__paddw":
                case "__paddd":
                case "__paddq":
                case "__psubb":
                case "__psubw":
                case "__psubd":
                case "__psubq":
                case "__pmulhw":
                case "__pmullw":
                case "__punpckhbw":
                case "__punpckhwd":
                case "__punpckhdq":
                case "__punpcklbw":
                case "__punpcklwd":
                case "__punpckldq":
                case "__packsswb":
                case "__packssdw":
                case "__packuswb":
                    if (args.Count != 2)
                        throw Abort("{0}: argument mismatched", name);
                    __simd(codes, name, args[0], args[1]);
                    return true;
                case "__psllw":
                case "__pslld":
                case "__psllq":
                case "__psrlw":
                case "__psrld":
                case "__psrlq":
                case "__psraw":
                case "__psrad":
                    if (args.Count != 2)
                        throw Abort("{0}: argument mismatched", name);
                    __simd_shift(codes, name, args[0], args[1]);
                    return true;
            }
            return false;
        }

        private Mm? GetMm(IIntValue v)
        {
            var fp = v as Function.Ptr;
            if (fp == null || !fp.Name.StartsWith("__mm") || fp.Name.Length != 5)
                return null;
            var n = fp.Name[4];
            if (n < '0' || n > '9') return null;
            return (Mm)n - '0';
        }

        private Xmm? GetXmm(IIntValue v)
        {
            var fp = v as Function.Ptr;
            if (fp == null || !fp.Name.StartsWith("__xmm") || fp.Name.Length != 6)
                return null;
            var n = fp.Name[5];
            if (n < '0' || n > '9') return null;
            return (Xmm)n - '0';
        }

        public static void __emms(OpModule codes)
        {
            codes.Add(MMX.EMMS());
        }

        private void __movd(OpModule codes, IIntValue m1, IIntValue m2)
        {
            var m1m = GetMm(m1);
            var m2m = GetMm(m2);
            var m1x = GetXmm(m1);
            var m2x = GetXmm(m2);
            if (m1m != null)
            {
                if (m2 is Var && m2.Type is TypeIntBase)
                {
                    var ad = (m2 as Var).GetAddress(codes);
                    codes.Add(MMX.MovD((Mm)m1m, ad));
                }
                else if (m2 is IntValue)
                {
                    m2.AddCodes(codes, "mov", null);
                    codes.Add(MMX.MovD((Mm)m1m, Reg32.EAX));
                }
                else
                {
                    m2.AddCodes(codes, "mov", null);
                    codes.Add(MMX.MovD((Mm)m1m, new Addr32(Reg32.EAX)));
                }
                return;
            }
            else if (m1x != null)
            {
                if (m2 is Var && m2.Type is TypeIntBase)
                {
                    var m2a = (m2 as Var).GetAddress(codes);
                    codes.Add(SSE2.MovD((Xmm)m1x, m2a));
                }
                else if (m2 is IntValue)
                {
                    m2.AddCodes(codes, "mov", null);
                    codes.Add(SSE2.MovD((Xmm)m1x, Reg32.EAX));
                }
                else
                {
                    m2.AddCodes(codes, "mov", null);
                    codes.Add(SSE2.MovD((Xmm)m1x, new Addr32(Reg32.EAX)));
                }
                return;
            }

            var v = m1 as Var;
            if (v == null)
                throw Abort("__movd: invalid argument 1");
            if (m2m != null)
            {
                if (v.Type is TypeIntBase)
                {
                    var ad = v.GetAddress(codes);
                    codes.Add(MMX.MovD(ad, (Mm)m2m));
                }
                else
                {
                    v.AddCodes(codes, "mov", null);
                    codes.Add(MMX.MovD(new Addr32(Reg32.EAX), (Mm)m2m));
                }
            }
            else if (m2x != null)
            {
                if (v.Type is TypeIntBase)
                {
                    var ad = v.GetAddress(codes);
                    codes.Add(SSE2.MovD(ad, (Xmm)m2x));
                }
                else
                {
                    v.AddCodes(codes, "mov", null);
                    codes.Add(SSE2.MovD(new Addr32(Reg32.EAX), (Xmm)m2x));
                }
            }
            else
                throw Abort("__movd: invalid argument 2");
        }

        private void __movq(OpModule codes, IIntValue m1, IIntValue m2)
        {
            var m1m = GetMm(m1);
            var m2m = GetMm(m2);
            var m1x = GetXmm(m1);
            var m2x = GetXmm(m2);
            if (m1m != null && m2m != null)
                codes.Add(MMX.MovQ((Mm)m1m, (Mm)m2m));
            else if (m1x != null && m2x != null)
                codes.Add(SSE2.MovQ((Xmm)m1x, (Xmm)m2x));
            else if (m1m != null)
            {
                m2.AddCodes(codes, "mov", null);
                codes.Add(MMX.MovQ((Mm)m1m, new Addr32(Reg32.EAX)));
            }
            else if (m2m != null)
            {
                m1.AddCodes(codes, "mov", null);
                codes.Add(MMX.MovQ(new Addr32(Reg32.EAX), (Mm)m2m));
            }
            else if (m1x != null)
            {
                m2.AddCodes(codes, "mov", null);
                codes.Add(SSE2.MovQ((Xmm)m1x, new Addr32(Reg32.EAX)));
            }
            else if (m2x != null)
            {
                m1.AddCodes(codes, "mov", null);
                codes.Add(SSE2.MovQ(new Addr32(Reg32.EAX), (Xmm)m2x));
            }
            else
                throw Abort("__movq: invalid arguments");
        }

        private void __movdq(OpModule codes, string op, IIntValue m1, IIntValue m2)
        {
            var m1x = GetXmm(m1);
            var m2x = GetXmm(m2);
            var op2 = op.Substring(2);
            if (m1x != null && m2x != null)
                codes.Add(SSE2.FromName(op2, (Xmm)m1x, (Xmm)m2x));
            else if (m1x != null)
            {
                m2.AddCodes(codes, "mov", null);
                codes.Add(SSE2.FromName(op2, (Xmm)m1x, new Addr32(Reg32.EAX)));
            }
            else if (m2x != null)
            {
                m1.AddCodes(codes, "mov", null);
                codes.Add(SSE2.FromName(op2, new Addr32(Reg32.EAX), (Xmm)m2x));
            }
            else
                throw Abort("{0}: invalid arguments", op);
        }

        private void __simd(OpModule codes, string op, IIntValue m1, IIntValue m2)
        {
            var m1m = GetMm(m1);
            var m2m = GetMm(m2);
            var m1x = GetXmm(m1);
            var m2x = GetXmm(m2);
            var op2 = op.Substring(2);
            if (m1m != null)
            {
                if (m2m != null)
                    codes.Add(MMX.FromName(op2, (Mm)m1m, (Mm)m2m));
                else
                {
                    m2.AddCodes(codes, "mov", null);
                    codes.Add(MMX.FromName(op2, (Mm)m1m, new Addr32(Reg32.EAX)));
                }
            }
            else if (m1x != null)
            {
                if (m2x != null)
                    codes.Add(SSE2.FromName(op2, (Xmm)m1x, (Xmm)m2x));
                else
                {
                    m2.AddCodes(codes, "mov", null);
                    codes.Add(SSE2.FromName(op2, (Xmm)m1x, new Addr32(Reg32.EAX)));
                }
            }
            else
                throw Abort("{0}: invalid argument 1", op);
        }

        private void __simd_shift(OpModule codes, string op, IIntValue m1, IIntValue m2)
        {
            IntValue m2i = m2 as IntValue;
            if (m2i == null)
            {
                __simd(codes, op, m1, m2);
                return;
            }
            var m1m = GetMm(m1);
            var m1x = GetXmm(m1);
            var op2 = op.Substring(2);
            if (m1m != null)
                codes.Add(MMX.FromName(op2, (Mm)m1m, (byte)m2i.Value));
            else if (m1x != null)
                codes.Add(SSE2.FromName(op2, (Xmm)m1x, (byte)m2i.Value));
            else
                throw Abort("{0}: invalid argument 1", op);
        }
    }
}
