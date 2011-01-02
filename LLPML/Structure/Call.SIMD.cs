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
        public bool AddSIMDCodes(OpModule codes, List<NodeBase> args)
        {
            switch (name)
            {
                case "__emms":
                    if (codes != null)
                    {
                        if (args.Count != 0)
                            throw Abort("{0}: argument mismatched", name);
                        __emms(codes);
                    }
                    return true;
                case "__movd":
                    if (codes != null)
                    {
                        if (args.Count != 2)
                            throw Abort("{0}: argument mismatched", name);
                        __movd(codes, args[0], args[1]);
                    }
                    return true;
                case "__movq":
                    if (codes != null)
                    {
                        if (args.Count != 2)
                            throw Abort("{0}: argument mismatched", name);
                        __movq(codes, args[0], args[1]);
                    }
                    return true;
                case "__movdqa":
                case "__movdqu":
                    if (codes != null)
                    {
                        if (args.Count != 2)
                            throw Abort("{0}: argument mismatched", name);
                        __movdq(codes, name, args[0], args[1]);
                    }
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
                    if (codes != null)
                    {
                        if (args.Count != 2)
                            throw Abort("{0}: argument mismatched", name);
                        __simd(codes, name, args[0], args[1]);
                    }
                    return true;
                case "__psllw":
                case "__pslld":
                case "__psllq":
                case "__psrlw":
                case "__psrld":
                case "__psrlq":
                case "__psraw":
                case "__psrad":
                    if (codes != null)
                    {
                        if (args.Count != 2)
                            throw Abort("{0}: argument mismatched", name);
                        __simd_shift(codes, name, args[0], args[1]);
                    }
                    return true;
            }
            return false;
        }

        private Mm? GetMm(NodeBase v)
        {
            var fp = v as Variant;
            if (fp == null || !fp.Name.StartsWith("__mm") || fp.Name.Length != 5)
                return null;
            var n = fp.Name[4];
            if (n < '0' || n > '9') return null;
            return (Mm)n - '0';
        }

        private Xmm? GetXmm(NodeBase v)
        {
            var fp = v as Variant;
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

        private void __movd(OpModule codes, NodeBase m1, NodeBase m2)
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
                    codes.Add(MMX.MovDA((Mm)m1m, ad));
                }
                else if (m2 is IntValue)
                {
                    m2.AddCodesValue(codes, "mov", null);
                    codes.Add(MMX.MovD((Mm)m1m, Reg32.EAX));
                }
                else
                {
                    m2.AddCodesValue(codes, "mov", null);
                    codes.Add(MMX.MovDA((Mm)m1m, Addr32.New(Reg32.EAX)));
                }
                return;
            }
            else if (m1x != null)
            {
                if (m2 is Var && m2.Type is TypeIntBase)
                {
                    var m2a = (m2 as Var).GetAddress(codes);
                    codes.Add(SSE2.MovDA((Xmm)m1x, m2a));
                }
                else if (m2 is IntValue)
                {
                    m2.AddCodesValue(codes, "mov", null);
                    codes.Add(SSE2.MovD((Xmm)m1x, Reg32.EAX));
                }
                else
                {
                    m2.AddCodesValue(codes, "mov", null);
                    codes.Add(SSE2.MovDA((Xmm)m1x, Addr32.New(Reg32.EAX)));
                }
                return;
            }

            var v = Var.Get(m1);
            if (v == null)
                throw Abort("__movd: invalid argument 1");
            if (m2m != null)
            {
                if (v.Type is TypeIntBase)
                {
                    var ad = v.GetAddress(codes);
                    codes.Add(MMX.MovDAM(ad, (Mm)m2m));
                }
                else
                {
                    v.AddCodesValue(codes, "mov", null);
                    codes.Add(MMX.MovDAM(Addr32.New(Reg32.EAX), (Mm)m2m));
                }
            }
            else if (m2x != null)
            {
                if (v.Type is TypeIntBase)
                {
                    var ad = v.GetAddress(codes);
                    codes.Add(SSE2.MovDAX(ad, (Xmm)m2x));
                }
                else
                {
                    v.AddCodesValue(codes, "mov", null);
                    codes.Add(SSE2.MovDAX(Addr32.New(Reg32.EAX), (Xmm)m2x));
                }
            }
            else
                throw Abort("__movd: invalid argument 2");
        }

        private void __movq(OpModule codes, NodeBase m1, NodeBase m2)
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
                m2.AddCodesValue(codes, "mov", null);
                codes.Add(MMX.MovQA((Mm)m1m, Addr32.New(Reg32.EAX)));
            }
            else if (m2m != null)
            {
                m1.AddCodesValue(codes, "mov", null);
                codes.Add(MMX.MovQAM(Addr32.New(Reg32.EAX), (Mm)m2m));
            }
            else if (m1x != null)
            {
                m2.AddCodesValue(codes, "mov", null);
                codes.Add(SSE2.MovQA((Xmm)m1x, Addr32.New(Reg32.EAX)));
            }
            else if (m2x != null)
            {
                m1.AddCodesValue(codes, "mov", null);
                codes.Add(SSE2.MovQAX(Addr32.New(Reg32.EAX), (Xmm)m2x));
            }
            else
                throw Abort("__movq: invalid arguments");
        }

        private void __movdq(OpModule codes, string op, NodeBase m1, NodeBase m2)
        {
            var m1x = GetXmm(m1);
            var m2x = GetXmm(m2);
            var op2 = op.Substring(2);
            if (m1x != null && m2x != null)
                codes.Add(SSE2.FromName(op2, (Xmm)m1x, (Xmm)m2x));
            else if (m1x != null)
            {
                m2.AddCodesValue(codes, "mov", null);
                codes.Add(SSE2.FromNameA(op2, (Xmm)m1x, Addr32.New(Reg32.EAX)));
            }
            else if (m2x != null)
            {
                m1.AddCodesValue(codes, "mov", null);
                codes.Add(SSE2.FromNameAX(op2, Addr32.New(Reg32.EAX), (Xmm)m2x));
            }
            else
                throw Abort("{0}: invalid arguments", op);
        }

        private void __simd(OpModule codes, string op, NodeBase m1, NodeBase m2)
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
                    m2.AddCodesValue(codes, "mov", null);
                    codes.Add(MMX.FromNameA(op2, (Mm)m1m, Addr32.New(Reg32.EAX)));
                }
            }
            else if (m1x != null)
            {
                if (m2x != null)
                    codes.Add(SSE2.FromName(op2, (Xmm)m1x, (Xmm)m2x));
                else
                {
                    m2.AddCodesValue(codes, "mov", null);
                    codes.Add(SSE2.FromNameA(op2, (Xmm)m1x, Addr32.New(Reg32.EAX)));
                }
            }
            else
                throw Abort("{0}: invalid argument 1", op);
        }

        private void __simd_shift(OpModule codes, string op, NodeBase m1, NodeBase m2)
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
                codes.Add(MMX.FromNameB(op2, (Mm)m1m, (byte)m2i.Value));
            else if (m1x != null)
                codes.Add(SSE2.FromNameB(op2, (Xmm)m1x, (byte)m2i.Value));
            else
                throw Abort("{0}: invalid argument 1", op);
        }
    }
}
