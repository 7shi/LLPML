using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;
using Girl.LLPML.Parsing;

namespace Girl.LLPML
{
    public class CharValue : NodeBase, IIntValue
    {
        private char value;
        public char Value { get { return value; } }

        protected CharValue(BlockBase parent, string name) : base(parent, name) { }
        protected CharValue(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        public CharValue(char value) { this.value = value; }

        public TypeBase Type { get { return TypeChar.Instance; } }

        public void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            codes.AddCodesV(op, dest, Val32.New(Value));
        }
    }
}
