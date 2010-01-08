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
        public object Src;

        public IntValue() { }
        public IntValue(int value) { Src = value; }
        public IntValue(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            if (xr.NodeType == XmlNodeType.Element)
            {
                Src = null;
                string n = xr["name"];
                switch (xr.Name)
                {
                    case "int":
                        Src = parent.ReadInt(xr);
                        break;
                    case "string":
                        Src = parent.ReadString(xr);
                        break;
                    case "var-int":
                        Src = new VarInt(parent, xr);
                        break;
                    case "ptr":
                        Src = new Pointer(parent, xr);
                        break;
                    case "call":
                        Src = new Call(parent, xr);
                        break;
                    case "function-ptr":
                        Src = new Function.Ptr(parent, xr);
                        break;
                    default:
                        throw Abort(xr);
                }
                if (Src == null)
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

        public void GetValue(List<OpCode> codes, Module m, Addr32 ad)
        {
            if (Src is int)
            {
                if (ad == null)
                    codes.Add(I386.Push((uint)(int)Src));
                else
                    codes.Add(I386.Mov(ad, (uint)(int)Src));
            }
            else if (Src is string)
            {
                if (ad == null)
                    codes.Add(I386.Push(m.GetString(Src as string)));
                else
                    codes.Add(I386.Mov(ad, m.GetString(Src as string)));
            }
            else if (Src is VarInt)
            {
                if (ad == null)
                    codes.Add(I386.Push((Src as VarInt).GetAddress(codes, m)));
                else
                {
                    codes.Add(I386.Mov(Reg32.EAX, (Src as VarInt).GetAddress(codes, m)));
                    codes.Add(I386.Mov(ad, Reg32.EAX));
                }
            }
            else if (Src is Pointer)
            {
                (Src as Pointer).GetValue(codes, m);
                if (ad == null)
                    codes.Add(I386.Push(Reg32.EAX));
                else
                    codes.Add(I386.Mov(ad, Reg32.EAX));
            }
            else if (Src is Call)
            {
                (Src as Call).AddCodes(codes, m);
                if (ad == null)
                    codes.Add(I386.Push(Reg32.EAX));
                else
                    codes.Add(I386.Mov(ad, Reg32.EAX));
            }
            else if (Src is Function.Ptr)
            {
                if (ad == null)
                    codes.Add(I386.Push((Src as Function.Ptr).GetAddress(m)));
                else
                    codes.Add(I386.Mov(ad, (Src as Function.Ptr).GetAddress(m)));
            }
            else
            {
                throw new Exception("Unknown value.");
            }
        }
    }
}
