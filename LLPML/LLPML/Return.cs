using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Return : NodeBase
    {
        public bool IsLast = false;
        private object retval;
        private VarInt.Define __retval;

        public Return() { }
        public Return(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            retval = null;
            __retval = null;
            Parse(xr, delegate
            {
                if (xr.NodeType == XmlNodeType.Element)
                {
                    string name = xr["name"];
                    switch (xr.Name)
                    {
                        case "int":
                            retval = parent.ReadInt(xr);
                            break;
                        case "string":
                            retval = parent.ReadString(xr);
                            break;
                        case "var-int":
                            retval = new VarInt(parent, xr);
                            break;
                        case "ptr":
                            retval = new Pointer(parent, xr);
                            break;
                        default:
                            throw Abort(xr);
                    }
                    if (retval == null)
                    {
                        string msg = "invalid return value";
                        if (name != null) msg += ": " + name;
                        throw Abort(xr, msg);
                    }
                    __retval = new VarInt.Define(parent, "__retval", 0);
                }
            });
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            if (retval != null)
            {
                Addr32 ret = __retval.Address;
                if (retval is int)
                {
                    codes.Add(I386.Mov(ret, (uint)(int)retval));
                }
                else if (retval is string)
                {
                    codes.Add(I386.Mov(ret, m.GetString(retval as string)));
                }
                else if (retval is VarInt)
                {
                    codes.Add(I386.Mov(Reg32.EAX, (retval as VarInt).GetAddress(codes, m)));
                    codes.Add(I386.Mov(ret, Reg32.EAX));
                }
                else if (retval is Pointer)
                {
                    (retval as Pointer).GetValue(codes, m);
                    codes.Add(I386.Mov(ret, Reg32.EAX));
                }
                else
                {
                    throw new Exception("Unknown argument.");
                }
            }
            if (!IsLast) codes.Add(I386.Jmp(parent.Last));
        }
    }
}
