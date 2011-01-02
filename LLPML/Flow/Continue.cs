using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Continue : BreakBase
    {
        public Continue(BlockBase parent) : base(parent) { }

        public bool CanContinue()
        {
            for (BlockBase p = Parent; p != null; p = p.Parent)
            {
                if (p is Function) return false;
                if (p.AcceptsContinue) return true;
            }
            return false;
        }

        public override void AddCodes(OpModule codes)
        {
            BlockBase b = Parent;
            VarDeclare[] ptrs = UsingPointers;
            for (; ; ptrs = b.UsingPointers, b = b.Parent)
            {
                if (b == null || b is Function)
                    throw Abort("invalid continue");
                b.AddDestructors(codes, ptrs);
                if (b.AcceptsContinue) break;
                b.AddExitCodes(codes);
            }
            codes.Add(I386.JmpD(b.Continue));
        }
    }
}
