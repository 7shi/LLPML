using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.X86;

namespace Girl.LLPML
{
    public class VarInt : NodeBase
    {
        string name;
        public string Name { get { return name; } }

        Call call;
        int? v = null;
        Addr32 address = null;

        public VarInt() { }
        public VarInt(Root root, XmlTextReader xr) { Read(root, xr); }

        public override void Read(Root root, XmlTextReader xr)
        {
            name = xr["name"];
            v = null;
            call = null;
            address = root.GetVarInt(name);
            Parse(xr, delegate
            {
                if (xr.NodeType == XmlNodeType.Text)
                {
                    v = int.Parse(xr.Value);
                }
                else if (xr.NodeType == XmlNodeType.Element)
                {
                    switch (xr.Name)
                    {
                        case "call":
                            call = new Call(root, xr);
                            break;
                        default:
                            throw Abort(xr);
                    }
                }
            });
        }

        public override void AddCodes(List<OpCode> codes, Girl.PE.Module m)
        {
            if (!address.IsInitialized) address.Set(new Addr32(m.GetInt32(name)));
            if (v != null)
            {
                codes.Add(I386.Mov(Reg32.EAX, (uint)v));
                codes.Add(I386.Mov(address, Reg32.EAX));
            }
            else if (call != null)
            {
                call.AddCodes(codes, m);
                codes.Add(I386.Mov(address, Reg32.EAX));
            }
        }
    }
}
