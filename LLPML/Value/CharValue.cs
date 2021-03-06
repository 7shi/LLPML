using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;
using Girl.LLPML.Parsing;

namespace Girl.LLPML
{
    public class CharValue : NodeBase
    {
        private char value;
        public char Value { get { return value; } }

        public static CharValue New(char value, SrcInfo si)
        {
            var ret = new CharValue();
            ret.value = value;
            ret.SrcInfo = si;
            return ret;
        }

        public override TypeBase Type { get { return TypeChar.Instance; } }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            codes.AddCodesV(op, dest, Val32.New(Value));
        }
    }
}
