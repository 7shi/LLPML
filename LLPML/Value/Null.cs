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
    public class Null : NodeBase
    {
        public static Null New(BlockBase parent)
        {
            var ret = new Null();
            ret.Parent = parent;
            ret.name = "null";
            return ret;
        }

        public override TypeBase Type { get { return null; } }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            codes.AddCodesV(op, dest, Val32.New(0));
        }
    }
}
