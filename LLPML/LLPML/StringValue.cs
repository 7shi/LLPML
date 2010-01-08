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
        private string value;
        public string Value{get{return value;}}

        public StringValue(string value){ this.value = value; }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            IntValue.AddCodes(codes, op, dest, m.GetString(value));
        }
    }
}
