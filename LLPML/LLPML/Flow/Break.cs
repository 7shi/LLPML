using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Break : NodeBase
    {
        public Break(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            if (!CanBreak()) throw Abort(xr, "can not break");
            NoChild(xr);
        }

        public bool CanBreak()
        {
            for (Block p = parent; p != null; p = p.Parent)
            {
                if (p is Function) return false;
                if (p.AcceptsBreak) return true;
            }
            return false;
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            Block b = parent;
            for (; ; b = b.Parent)
            {
                if (b == null || b is Function)
                    throw new Exception("invalid break");
                if (b.AcceptsBreak) break;
                b.AddExitCodes(codes, m);
            }
            codes.Add(I386.Jmp(b.Destruct));
        }
    }
}