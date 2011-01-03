using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Break : BreakBase
    {
        public static Break New(BlockBase parent)
        {
            var ret = new Break();
            ret.init(parent);
            return ret;
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
            var b = Parent;
            var ptrs = UsingPointers;
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
