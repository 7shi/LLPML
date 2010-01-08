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
        public Continue(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            if (!CanContinue()) throw Abort(xr, "can not break");
            NoChild(xr);

            base.Read(xr);
        }

        public bool CanContinue()
        {
            for (BlockBase p = parent; p != null; p = p.Parent)
            {
                if (p is Function) return false;
                if (p.AcceptsContinue) return true;
            }
            return false;
        }

        public override void AddCodes(OpCodes codes)
        {
            BlockBase b = parent;
            Pointer.Declare[] ptrs = UsingPointers;
            for (; ; ptrs = b.UsingPointers, b = b.Parent)
            {
                if (b == null || b is Function)
                    throw Abort("invalid continue");
                b.AddDestructors(codes, ptrs);
                if (b.AcceptsContinue) break;
                b.AddExitCodes(codes);
            }
            codes.Add(I386.Jmp(b.Continue));
        }
    }
}
