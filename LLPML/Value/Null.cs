using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Null : NodeBase
    {
        public Null(BlockBase parent)
        {
            Parent = parent;
            name = "null";
        }

        public override TypeBase Type { get { return null; } }

        public override void AddCodesValue(OpModule codes, string op, Addr32 dest)
        {
            codes.AddCodesV(op, dest, Val32.New(0));
        }
    }
}
