using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class IntValue : IIntValue
    {
        private int value;
        public int Value { get { return value; } }

        public IntValue(int value) { this.value = value; }
        public IntValue(string value) : this(Parse(value)) { }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            AddCodes(codes, op, dest, (uint)value);
        }

        public static IIntValue Read(Block parent, XmlTextReader xr, bool isInt)
        {
            switch (xr.NodeType)
            {
                case XmlNodeType.Element:
                    switch (xr.Name)
                    {
                        case "int":
                            return new IntValue(parent.ReadInt(xr));
                        case "string":
                            return new StringValue(parent.ReadString(xr));
                        case "string-length":
                            return new IntValue(parent.ReadStringLength(xr));
                        case "var-int":
                            return new VarInt(parent, xr);
                        case "var-int-ptr":
                            return new VarInt.Ptr(parent, xr);
                        case "ptr":
                            return new Pointer(parent, xr);
                        case "call":
                            return new Call(parent, xr);
                        case "function-ptr":
                            return new Function.Ptr(parent, xr);
                        case "struct-member":
                            return new Struct.Member(parent, xr);
                        case "struct-size":
                            return new Struct.Size(parent, xr);
                        case "inc":
                            return new Inc(parent, xr);
                        case "dec":
                            return new Dec(parent, xr);
                        case "post-inc":
                            return new PostInc(parent, xr);
                        case "post-dec":
                            return new PostDec(parent, xr);
                        case "var-int-add":
                            return new VarInt.Add(parent, xr);
                        case "var-int-sub":
                            return new VarInt.Sub(parent, xr);
                        case "var-int-mul":
                            return new VarInt.Mul(parent, xr);
                        case "var-int-unsigned-mul":
                            return new VarInt.UnsignedMul(parent, xr);
                        case "var-int-div":
                            return new VarInt.Div(parent, xr);
                        case "var-int-unsigned-div":
                            return new VarInt.UnsignedDiv(parent, xr);
                        case "var-int-and":
                            return new VarInt.And(parent, xr);
                        case "var-int-or":
                            return new VarInt.Or(parent, xr);
                        case "var-int-shift-left":
                            return new VarInt.ShiftLeft(parent, xr);
                        case "var-int-shift-right":
                            return new VarInt.ShiftRight(parent, xr);
                        case "var-int-unsigned-shift-left":
                            return new VarInt.UnsignedShiftLeft(parent, xr);
                        case "var-int-unsigned-shift-right":
                            return new VarInt.UnsignedShiftRight(parent, xr);
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
                        case "unsigned-mul":
                            return new UnsignedMul(parent, xr);
                        case "div":
                            return new Div(parent, xr);
                        case "unsigned-div":
                            return new UnsignedDiv(parent, xr);
                        case "mod":
                            return new Mod(parent, xr);
                        case "unsigned-mod":
                            return new UnsignedMod(parent, xr);
                        case "shift-left":
                            return new ShiftLeft(parent, xr);
                        case "shift-right":
                            return new ShiftRight(parent, xr);
                        case "unsigned-shift-left":
                            return new UnsignedShiftLeft(parent, xr);
                        case "unsigned-shift-right":
                            return new UnsignedShiftRight(parent, xr);
                        case "and-also":
                            return new AndAlso(parent, xr);
                        case "or-else":
                            return new OrElse(parent, xr);
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
                        case "unsigned-greater":
                            return new UnsignedGreater(parent, xr);
                        case "unsigned-greater-equal":
                            return new UnsignedGreaterEqual(parent, xr);
                        case "unsigned-less":
                            return new UnsignedLess(parent, xr);
                        case "unsigned-less-equal":
                            return new UnsignedLessEqual(parent, xr);
                        default:
                            throw NodeBase.Abort(xr);
                    }

                case XmlNodeType.Text:
                    if (isInt)
                        return new IntValue(xr.Value);
                    else
                        return new StringValue(xr.Value);

                case XmlNodeType.Comment:
                case XmlNodeType.Whitespace:
                    return null;
            }
            throw NodeBase.Abort(xr, "value required");
        }

        public static int Parse(string value)
        {
            if (value.StartsWith("0x"))
                return Convert.ToInt32(value.Substring(2), 16);
            if (value.Length > 1 && value.StartsWith("0"))
                return Convert.ToInt32(value.Substring(1), 8);
            return int.Parse(value);
        }

        public static void AddCodes(List<OpCode> codes, string op, Addr32 dest, Val32 v)
        {
            switch (op)
            {
                case "push":
                    codes.Add(I386.Push(v));
                    break;
                default:
                    if (dest != null)
                        codes.Add(I386.FromName(op, dest, v));
                    else
                        codes.Add(I386.FromName(op, Reg32.EAX, v));
                    break;
            }
        }

        public static void AddCodes(List<OpCode> codes, string op, Addr32 dest, Addr32 ad)
        {
            switch (op)
            {
                case "push":
                    codes.Add(I386.Push(ad));
                    break;
                default:
                    codes.Add(I386.Mov(Reg32.EAX, ad));
                    if (dest != null) codes.Add(I386.FromName(op, dest, Reg32.EAX));
                    break;
            }
        }

        public static void AddCodes(List<OpCode> codes, string op, Addr32 dest)
        {
            switch (op)
            {
                case "push":
                    codes.Add(I386.Push(Reg32.EAX));
                    break;
                default:
                    if (dest != null) codes.Add(I386.FromName(op, dest, Reg32.EAX));
                    break;
            }
        }
    }
}
