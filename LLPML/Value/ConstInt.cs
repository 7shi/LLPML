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
    public class ConstInt : NodeBase, IIntValue
    {
        public IIntValue Value { get; private set; }

        public ConstInt(BlockBase parent, IIntValue value)
            : base(parent)
        {
            Value = value;
        }

        public TypeBase Type { get { return TypeInt.Instance; } }

        public void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            var v = IntValue.GetValue(Value);
            if (v != null)
                v.AddCodes(codes, op, dest);
            else
                Value.AddCodes(codes, op, dest);
        }
    }
}
