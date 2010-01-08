using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class IntValue : NodeBase
    {
        private object src;
        public object Source { get { return src; } }
        public bool IsVarInt { get { return src is VarInt; } }
        public bool HasValue { get { return src != null; } }

        public IntValue() { }
        public IntValue(int value) { src = value; }
        public IntValue(Block parent) : base(parent) { }
        public IntValue(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            if (xr.NodeType == XmlNodeType.Element)
            {
                src = null;
                string n = xr["name"];
                switch (xr.Name)
                {
                    case "int":
                        src = parent.ReadInt(xr);
                        break;
                    case "string":
                        src = parent.ReadString(xr);
                        break;
                    case "string-length":
                        src = parent.ReadStringLength(xr);
                        break;
                    case "var-int":
                        src = new VarInt(parent, xr);
                        break;
                    case "var-int-ptr":
                        src = new VarInt.Ptr(parent, xr);
                        break;
                    case "ptr":
                        src = new Pointer(parent, xr);
                        break;
                    case "call":
                        src = new Call(parent, xr);
                        break;
                    case "function-ptr":
                        src = new Function.Ptr(parent, xr);
                        break;
                    case "struct-member":
                        src = new Struct.Member(parent, xr);
                        break;
                    default:
                        throw Abort(xr);
                }
                if (src == null)
                {
                    string msg = "invalid argument";
                    if (n != null) msg += ": " + n;
                    throw Abort(xr, msg);
                }
            }
            else
            {
                throw Abort(xr, "value required");
            }
        }

        public void ReadValue(XmlTextReader xr, bool text)
        {
            switch (xr.NodeType)
            {
                case XmlNodeType.Text:
                    if (src != null) throw Abort(xr, "multiple value");
                    if (text) src = xr.Value; else src = int.Parse(xr.Value);
                    break;

                case XmlNodeType.Element:
                    if (src != null) throw Abort(xr, "multiple value");
                    Read(xr);
                    break;

                case XmlNodeType.Whitespace:
                case XmlNodeType.Comment:
                    break;

                default:
                    throw Abort(xr, "value required");
            }
        }

        public void AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            Val32 v = null;
            Addr32 ad = null;
            if (src is int)
            {
                v = (uint)(int)src;
            }
            else if (src is string)
            {
                v = m.GetString(src as string);
            }
            else if (src is VarInt)
            {
                ad = (src as VarInt).GetAddress(codes, m);
            }
            else if (src is VarInt.Ptr)
            {
                codes.Add(I386.Lea(Reg32.EAX, (src as VarInt.Ptr).GetAddress(codes, m)));
            }
            else if (src is Pointer)
            {
                (src as Pointer).GetValue(codes, m);
            }
            else if (src is Call)
            {
                (src as Call).AddCodes(codes, m);
            }
            else if (src is Function.Ptr)
            {
                v = (src as Function.Ptr).GetAddress(m);
            }
            else
            {
                throw new Exception("unknown value");
            }

            if (v != null)
            {
                switch (op)
                {
                    case "push":
                        codes.Add(I386.Push(v));
                        break;
                    case "mov":
                        codes.Add(I386.Mov(dest, v));
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
            else if (ad != null)
            {
                switch (op)
                {
                    case "push":
                        codes.Add(I386.Push(ad));
                        break;
                    case "mov":
                        codes.Add(I386.Mov(Reg32.EAX, ad));
                        codes.Add(I386.Mov(dest, Reg32.EAX));
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
            else
            {
                switch (op)
                {
                    case "push":
                        codes.Add(I386.Push(Reg32.EAX));
                        break;
                    case "mov":
                        codes.Add(I386.Mov(dest, Reg32.EAX));
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
}
