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
    public class IntValue : NodeBase
    {
        private int value;
        public virtual int Value { get { return value; } }

        protected IntValue(BlockBase parent) { Parent = parent; }
        public IntValue(int value) { this.value = value; }
        public IntValue(string value) : this(Parse(value)) { }

        public override TypeBase Type { get { return TypeInt.Instance; } }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            codes.AddCodesV(op, dest, Val32.NewI(Value));
        }

        public static NodeBase[] ReadText(BlockBase parent, Tokenizer token)
        {
            Parser parser = new Parser(token, parent);
            NodeBase[] ret = parser.ParseExpressions();
            if (token.CanRead) ret = null;
            return ret;
        }

        public static int Parse(string value)
        {
            if (value.StartsWith("0x"))
                return Convert.ToInt32(value.Substring(2), 16);
            if (value.Length > 1 && value.StartsWith("0"))
                return Convert.ToInt32(value.Substring(1), 8);
            return int.Parse(value);
        }

        public static IntValue GetValue(NodeBase v)
        {
            if (v is ConstInt)
                return GetValue((v as ConstInt).Value);
            else if (v is IntValue)
                return v as IntValue;
            else if (v is Operator)
                return (v as Operator).GetConst();
            else if (v is Variant)
                return GetValue((v as Variant).GetConst());
            else
                return null;
        }
    }
}
