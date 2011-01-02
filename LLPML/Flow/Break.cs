using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Break : BreakBase
    {
        public Break(BlockBase parent) : base(parent) { }
        public Break(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            if (!CanBreak()) throw Abort(xr, "can not break");
            NoChild(xr);

            base.Read(xr);
        }

        public bool CanBreak()
        {
            for (var p = Parent; p != null; p = p.Parent)
            {
                if (p is Function) return false;
                if (p.AcceptsBreak) return true;
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
                    throw Abort("invalid break");
                b.AddDestructors(codes, ptrs);
                if (b.AcceptsBreak) break;
                b.AddExitCodes(codes);
            }
            codes.Add(I386.JmpD(b.Destruct));
        }
    }
}
