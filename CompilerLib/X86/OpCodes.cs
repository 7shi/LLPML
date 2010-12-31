using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;

namespace Girl.X86
{
    public class OpCodes
    {
        private ArrayList list;

        public OpCodes()
        {
            list = new ArrayList();
        }

        public void Add(OpCode op)
        {
            list.Add(op);
        }

        public void AddCodes(string op, Addr32 dest)
        {
            AddCodesA(op, dest, (Addr32)null);
        }

        public void AddCodesA(string op, Addr32 dest, Addr32 ad)
        {
            switch (op)
            {
                case "push":
                    if (ad != null)
                        Add(I386.PushA(ad));
                    else
                        Add(I386.Push(Reg32.EAX));
                    break;
                default:
                    if (ad != null) Add(I386.MovRA(Reg32.EAX, ad));
                    if (dest != null) Add(I386.FromName2AR(op, dest, Reg32.EAX));
                    break;
            }
        }

        public void AddCodesV(string op, Addr32 dest, Val32 v)
        {
            switch (op)
            {
                case "push":
                    Add(I386.PushD(v));
                    break;
                default:
                    if (dest != null)
                        Add(I386.FromName2A(op, dest, v));
                    else
                        Add(I386.FromName2R(op, Reg32.EAX, v));
                    break;
            }
        }

        public void AddCodesSW(string op, Addr32 dest)
        {
            AddCodesSWA(op, dest, null);
        }

        public void AddCodesSWA(string op, Addr32 dest, Addr32 ad)
        {
            switch (op)
            {
                case "push":
                    if (ad != null)
                        Add(I386.MovsxWA(Reg32.EAX, ad));
                    else
                        Add(I386.MovsxW(Reg32.EAX, Reg16.AX));
                    Add(I386.Push(Reg32.EAX));
                    break;
                default:
                    if (ad != null) Add(I386.MovWRA(Reg16.AX, ad));
                    if (dest != null)
                        Add(I386.FromName2WAR(op, dest, Reg16.AX));
                    else
                        Add(I386.MovsxW(Reg32.EAX, Reg16.AX));
                    break;
            }
        }

        public void AddCodesUW(string op, Addr32 dest)
        {
            AddCodesUWA(op, dest, null);
        }

        public void AddCodesUWA(string op, Addr32 dest, Addr32 ad)
        {
            switch (op)
            {
                case "push":
                    if (ad != null)
                        Add(I386.MovzxWA(Reg32.EAX, ad));
                    else
                        Add(I386.MovzxW(Reg32.EAX, Reg16.AX));
                    Add(I386.Push(Reg32.EAX));
                    break;
                default:
                    if (ad != null) Add(I386.MovWRA(Reg16.AX, ad));
                    if (dest != null)
                        Add(I386.FromName2WAR(op, dest, Reg16.AX));
                    else
                        Add(I386.MovzxW(Reg32.EAX, Reg16.AX));
                    break;
            }
        }

        public void AddCodesSB(string op, Addr32 dest)
        {
            AddCodesSBA(op, dest, null);
        }

        public void AddCodesSBA(string op, Addr32 dest, Addr32 ad)
        {
            switch (op)
            {
                case "push":
                    if (ad != null)
                        Add(I386.MovsxBA(Reg32.EAX, ad));
                    else
                        Add(I386.MovsxB(Reg32.EAX, Reg8.AL));
                    Add(I386.Push(Reg32.EAX));
                    break;
                default:
                    if (ad != null) Add(I386.MovBRA(Reg8.AL, ad));
                    if (dest != null)
                        Add(I386.FromName2BAR(op, dest, Reg8.AL));
                    else
                        Add(I386.MovsxB(Reg32.EAX, Reg8.AL));
                    break;
            }
        }

        public void AddCodesUB(string op, Addr32 dest)
        {
            AddCodesUBA(op, dest, null);
        }

        public void AddCodesUBA(string op, Addr32 dest, Addr32 ad)
        {
            switch (op)
            {
                case "push":
                    if (ad != null)
                        Add(I386.MovzxBA(Reg32.EAX, ad));
                    else
                        Add(I386.MovzxB(Reg32.EAX, Reg8.AL));
                    Add(I386.Push(Reg32.EAX));
                    break;
                default:
                    if (ad != null) Add(I386.MovBRA(Reg8.AL, ad));
                    if (dest != null)
                        Add(I386.FromName2BAR(op, dest, Reg8.AL));
                    else
                        Add(I386.MovzxB(Reg32.EAX, Reg8.AL));
                    break;
            }
        }

        public OpCode[] ToArray()
        {
            var ret = new OpCode[list.Count];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = (OpCode)list[i];
            return ret;
        }
    }
}
