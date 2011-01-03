using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class StringValue : NodeBase
    {
        public string Value { get; protected set; }

        public static StringValue New(string value)
        {
            var ret = new StringValue();
            ret.Value = value;
            return ret;
        }

        public override TypeBase Type { get { return TypeConstString.Instance; } }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            codes.AddCodesV(op, dest, codes.GetString(Value));
        }
    }
}
