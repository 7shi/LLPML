using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class VarInt : VarBase
    {
        public override Addr32 Address
        {
            get { return parent.GetVarInt(name).address; }
            set { parent.GetVarInt(name).address = value; }
        }

        private Call call;
        private int? value;
        private VarInt src;

        public VarInt() { }

        public VarInt(Block parent, string name)
            : base(parent, name)
        {
            parent.AddVarInt(this);
        }

        public VarInt(Block parent, string name, int value)
            : this(parent, name)
        {
            this.value = value;
        }

        public VarInt(Block parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            name = xr["name"];
            value = null;
            call = null;
            src = null;
            Parse(xr, delegate
            {
                if (xr.NodeType == XmlNodeType.Text)
                {
                    value = int.Parse(xr.Value);
                }
                else if (xr.NodeType == XmlNodeType.Element)
                {
                    switch (xr.Name)
                    {
                        case "call":
                            call = new Call(parent, xr);
                            break;
                        case "var-int":
                            src = parent.ReadVarInt(xr);
                            break;
                        default:
                            throw Abort(xr);
                    }
                }
            });
            if (value != null || call != null || src != null) parent.AddVarInt(this);
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            if (value != null)
            {
                codes.Add(I386.Mov(Address, (uint)value));
            }
            else if (call != null)
            {
                call.AddCodes(codes, m);
                codes.Add(I386.Mov(Address, Reg32.EAX));
            }
            else if (src != null)
            {
                codes.Add(I386.Mov(Reg32.EAX, src.Address));
                codes.Add(I386.Mov(Address, Reg32.EAX));
            }
        }

        public void WriteArg(List<OpCode> codes, int blv)
        {
            int lv = parent.Level;
            if (lv == blv || address.IsAddress)
            {
                codes.Add(I386.Push(address));
            }
            else if (0 < lv && lv < blv)
            {
                codes.Add(I386.Mov(Reg32.EAX, new Addr32(Reg32.EBP, -lv * 4)));
                codes.Add(I386.Push(new Addr32(Reg32.EAX, address.Disp)));
            }
            else
            {
                throw new Exception("Invalid variable scope.");
            }
        }
    }
}
