using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.LLPML.Parsing;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class AddrOf : NodeBase
    {
        public NodeBase Target { get; private set; }

        public static AddrOf New(BlockBase parent, NodeBase target)
        {
            var ret = new AddrOf();
            ret.Parent = parent;
            ret.Target = target;
            return ret;
        }

        public override TypeBase Type { get { return TypePointer.New(Target.Type); } }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            var t = Var.Get(Target);
            if (t == null)
                throw Abort("addrof: variable required");
            var ad = t.GetAddress(codes);
            codes.Add(I386.Lea(Reg32.EAX, ad));
            codes.AddCodes(op, dest);
        }
    }
}
