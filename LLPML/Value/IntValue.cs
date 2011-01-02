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

        protected IntValue(BlockBase parent) : base(parent) { }
        protected IntValue(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        public IntValue(int value) { this.value = value; }
        public IntValue(string value) : this(Parse(value)) { }

        public override TypeBase Type { get { return TypeInt.Instance; } }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            codes.AddCodesV(op, dest, Val32.NewI(Value));
        }

        private static NodeBase ReadElement(BlockBase parent, XmlTextReader xr)
        {
            switch (xr.Name)
            {
                case "int":
                    return parent.ReadInt(xr);
                case "string":
                    return new StringValue(parent.ReadString(xr));
                case "string-length":
                    return new IntValue(parent.ReadStringLength(xr));
                case "null":
                    return new Null(parent, xr);
                case "var":
                    return new Var(parent, xr);
                case "ptr":
                    return new Var(parent, xr);
                case "this":
                    return new Struct.This(parent, xr);
                case "base":
                    return new Struct.Base(parent, xr);
                case "cast":
                    return new Cast(parent, xr);
                case "call":
                    return new Call(parent, xr);
                case "delegate":
                    return new Delegate(parent, xr);
                case "function":
                    return new Function(parent, xr);
                case "function-ptr":
                    return new Variant(parent, xr);
                case "struct-member":
                    return new Struct.Member(parent, xr);
                case "size-of":
                    return new SizeOf(parent, xr);
                case "addr-of":
                    return new AddrOf(parent, xr);
                case "type-of":
                    return new TypeOf(parent, xr);
                case "index":
                    return new Index(parent, xr);
                case "get-function":
                    return new StringValue(parent.FullName);
                case "get-file":
                    return new StringValue(parent.Root.Source);
                case "get-line":
                    return new IntValue(xr.LineNumber);
                case "set":
                    return new Set(parent, xr);
                case "inc":
                    return new Inc(parent, xr);
                case "dec":
                    return new Dec(parent, xr);
                case "post-inc":
                    return new PostInc(parent, xr);
                case "post-dec":
                    return new PostDec(parent, xr);
                case "var-add":
                    return new VarAdd(parent, xr);
                case "var-sub":
                    return new VarSub(parent, xr);
                case "var-mul":
                    return new VarMul(parent, xr);
                case "var-div":
                    return new VarDiv(parent, xr);
                case "var-and":
                    return new VarAnd(parent, xr);
                case "var-or":
                    return new VarOr(parent, xr);
                case "var-shift-left":
                    return new VarShiftLeft(parent, xr);
                case "var-shift-right":
                    return new VarShiftRight(parent, xr);
                case "add":
                    return new Add(parent, xr);
                case "sub":
                    return new Sub(parent, xr);
                case "and":
                    return new And(parent, xr);
                case "or":
                    return new Or(parent, xr);
                case "xor":
                    return new Xor(parent, xr);
                case "mul":
                    return new Mul(parent, xr);
                case "div":
                    return new Div(parent, xr);
                case "mod":
                    return new Mod(parent, xr);
                case "shift-left":
                    return new ShiftLeft(parent, xr);
                case "shift-right":
                    return new ShiftRight(parent, xr);
                case "and-also":
                    return new AndAlso(parent, xr);
                case "or-else":
                    return new OrElse(parent, xr);
                case "neg":
                    return new Neg(parent, xr);
                case "rev":
                    return new Rev(parent, xr);
                case "not":
                    return new Not(parent, xr);
                case "equal":
                    return new Equal(parent, xr);
                case "not-equal":
                    return new NotEqual(parent, xr);
                case "greater":
                    return new Greater(parent, xr);
                case "greater-equal":
                    return new GreaterEqual(parent, xr);
                case "less":
                    return new Less(parent, xr);
                case "less-equal":
                    return new LessEqual(parent, xr);
                case "new":
                    return new Struct.New(parent, xr);
                case "is":
                    return new Struct.Is(parent, xr);
                case "as":
                    return new Struct.As(parent, xr);
                default:
                    throw parent.Abort(xr);
            }
        }

        public static NodeBase[] Read(BlockBase parent, XmlTextReader xr)
        {
            switch (xr.NodeType)
            {
                case XmlNodeType.Element:
                    return new NodeBase[] { ReadElement(parent, xr) };

                case XmlNodeType.Text:
                    {
                        NodeBase[] ret = ReadText(parent,
                            new Tokenizer(parent.Root.Source, xr));
                        if (ret == null)
                            throw parent.Abort(xr, "invalid expression");
                        return ret;
                    }

                case XmlNodeType.Comment:
                case XmlNodeType.Whitespace:
                    return null;
            }
            throw parent.Abort(xr, "value required");
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
