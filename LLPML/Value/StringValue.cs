using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class StringValue : NodeBase
    {
        public string Value { get; private set; }

        protected StringValue(BlockBase parent, string value)
        {
            Parent = parent;
            Value = value;
        }

        public StringValue(string value)
        {
            Value = value;
        }

        public override TypeBase Type { get { return TypeConstString.Instance; } }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            codes.AddCodesV(op, dest, codes.GetString(Value));
        }
    }
}
