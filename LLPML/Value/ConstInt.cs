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
    public class ConstInt : NodeBase
    {
        public NodeBase Value { get; private set; }

        public ConstInt(BlockBase parent, NodeBase value)
        {
            Parent = parent;
            Value = value;
        }

        public override TypeBase Type { get { return TypeInt.Instance; } }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            var v = IntValue.GetValue(Value);
            if (v != null)
                v.AddCodesV(codes, op, dest);
            else
                Value.AddCodesV(codes, op, dest);
        }
    }
}
