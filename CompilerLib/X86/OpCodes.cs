using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;

namespace Girl.X86
{
    public class OpCodes : List<OpCode>
    {
        public Module Module { get; private set; }
        public OpCodes(Module m) { this.Module = m; }

        public void AddCodes(string op, Addr32 dest)
        {
            switch (op)
            {
                case "push":
                    Add(I386.Push(Reg32.EAX));
                    break;
                default:
                    if (dest != null) Add(I386.FromName(op, dest, Reg32.EAX));
                    break;
            }
        }

        public void AddCodes(string op, Addr32 dest, Val32 v)
        {
            switch (op)
            {
                case "push":
                    Add(I386.Push(v));
                    break;
                default:
                    if (dest != null)
                        Add(I386.FromName(op, dest, v));
                    else
                        Add(I386.FromName(op, Reg32.EAX, v));
                    break;
            }
        }

        public void AddCodes(string op, Addr32 dest, Addr32 ad)
        {
            switch (op)
            {
                case "push":
                    Add(I386.Push(ad));
                    break;
                default:
                    Add(I386.Mov(Reg32.EAX, ad));
                    if (dest != null) Add(I386.FromName(op, dest, Reg32.EAX));
                    break;
            }
        }

        public void AddCodesSW(string op, Addr32 dest, Addr32 ad)
        {
            switch (op)
            {
                case "push":
                    AddRange(new OpCode[] {
                        I386.MovsxW(Reg32.EAX, ad),
                        I386.Push(Reg32.EAX)
                    });
                    break;
                default:
                    Add(I386.MovW(Reg16.AX, ad));
                    if (dest != null)
                        Add(I386.FromNameW(op, dest, Reg16.AX));
                    else
                        Add(I386.MovsxW(Reg32.EAX, Reg16.AX));
                    break;
            }
        }

        public void AddCodesUW(string op, Addr32 dest, Addr32 ad)
        {
            switch (op)
            {
                case "push":
                    AddRange(new OpCode[] {
                        I386.MovzxW(Reg32.EAX, ad),
                        I386.Push(Reg32.EAX)
                    });
                    break;
                default:
                    Add(I386.MovW(Reg16.AX, ad));
                    if (dest != null)
                        Add(I386.FromNameW(op, dest, Reg16.AX));
                    else
                        Add(I386.MovzxW(Reg32.EAX, Reg16.AX));
                    break;
            }
        }

        public void AddCodesSB(string op, Addr32 dest, Addr32 ad)
        {
            switch (op)
            {
                case "push":
                    AddRange(new OpCode[] {
                        I386.MovsxB(Reg32.EAX, ad),
                        I386.Push(Reg32.EAX)
                    });
                    break;
                default:
                    Add(I386.MovB(Reg8.AL, ad));
                    if (dest != null)
                        Add(I386.FromNameB(op, dest, Reg8.AL));
                    else
                        Add(I386.MovsxB(Reg32.EAX, Reg8.AL));
                    break;
            }
        }

        public void AddCodesUB(string op, Addr32 dest, Addr32 ad)
        {
            switch (op)
            {
                case "push":
                    AddRange(new OpCode[] {
                        I386.MovzxB(Reg32.EAX, ad),
                        I386.Push(Reg32.EAX)
                    });
                    break;
                default:
                    Add(I386.MovB(Reg8.AL, ad));
                    if (dest != null)
                        Add(I386.FromNameB(op, dest, Reg8.AL));
                    else
                        Add(I386.MovzxB(Reg32.EAX, Reg8.AL));
                    break;
            }
        }
    }
}
