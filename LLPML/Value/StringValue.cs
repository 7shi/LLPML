using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class StringValue : IIntValue
    {
        public string Value { get; private set; }

        public StringValue(string value)
        {
            Value = value;
        }

        public TypeBase Type { get { return TypeConstString.Instance; } }

        public void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            codes.AddCodes(op, dest, codes.GetString(Value));
        }
    }
}
