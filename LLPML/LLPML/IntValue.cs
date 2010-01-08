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
                        case "add":
                            return new Add(parent, xr);
                        case "sub":
                            return new Sub(parent, xr);
                        default:
                            throw NodeBase.Abort(xr);
                    }

                case XmlNodeType.Text:
                    if (isInt)
                        return new IntValue(int.Parse(xr.Value));
                    else
                        return new StringValue(xr.Value);

                case XmlNodeType.Comment:
                case XmlNodeType.Whitespace:
                    return null;
            }
            throw NodeBase.Abort(xr, "value required");
        }

        private int value;

        public IntValue(int value) { this.value = value; }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            AddCodes(codes, op, dest, (uint)value);
        }

        public static void AddCodes(List<OpCode> codes, string op, Addr32 dest, Val32 v)
        {
            switch (op)
            {
                case "push":
                    codes.Add(I386.Push(v));
                    break;
                case "mov":
                    if (dest != null)
                        codes.Add(I386.Mov(dest, v));
                    else
                        codes.Add(I386.Mov(Reg32.EAX, v));
                    break;
                case "add":
                    codes.Add(I386.Add(dest, v));
                    break;
                case "sub":
                    codes.Add(I386.Sub(dest, v));
                    break;
                default:
                    throw new Exception("unknown operation: " + op);
            }
        }

        public static void AddCodes(List<OpCode> codes, string op, Addr32 dest, Addr32 ad)
        {
            switch (op)
            {
                case "push":
                    codes.Add(I386.Push(ad));
                    break;
                case "mov":
                    codes.Add(I386.Mov(Reg32.EAX, ad));
                    if (dest != null) codes.Add(I386.Mov(dest, Reg32.EAX));
                    break;
                case "add":
                    codes.Add(I386.Mov(Reg32.EAX, ad));
                    codes.Add(I386.Add(dest, Reg32.EAX));
                    break;
                case "sub":
                    codes.Add(I386.Mov(Reg32.EAX, ad));
                    codes.Add(I386.Sub(dest, Reg32.EAX));
                    break;
                default:
                    throw new Exception("unknown operation: " + op);
            }
        }

        public static void AddCodes(List<OpCode> codes, string op, Addr32 dest)
        {
            switch (op)
            {
                case "push":
                    codes.Add(I386.Push(Reg32.EAX));
                    break;
                case "mov":
                    if (dest != null) codes.Add(I386.Mov(dest, Reg32.EAX));
                    break;
                case "add":
                    codes.Add(I386.Add(dest, Reg32.EAX));
                    break;
                case "sub":
                    codes.Add(I386.Sub(dest, Reg32.EAX));
                    break;
                default:
                    throw new Exception("unknown operation: " + op);
            }
        }
    }
}
