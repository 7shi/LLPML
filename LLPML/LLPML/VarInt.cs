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
        protected VarInt.Define reference;

        public VarInt() { }

        public VarInt(Block parent, string name)
            : base(parent, name)
        {
            reference = parent.GetVarInt(name);
            if (reference == null)
                throw new Exception("undefined variable: " + name);
        }

        public VarInt(Block parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            if (!xr.IsEmptyElement)
                throw Abort(xr, "<" + xr.Name + "> can not have any children");

            name = xr["name"];
            if (name == null) throw Abort(xr, "name required");

            reference = parent.GetVarInt(name);
            if (reference == null)
                throw Abort(xr, "undefined variable: " + name);
        }

        public Addr32 GetAddress(List<OpCode> codes, Module m)
        {
            AddCodes(codes, m);
            Addr32 ad = reference.Address;
            if (parent == reference.Parent || ad.IsAddress)
            {
                return ad;
            }
            int lv = reference.Parent.Level;
            if (lv <= 0 || lv >= parent.Level)
            {
                throw new Exception("Invalid variable scope: " + name);
            }
            codes.Add(I386.Mov(Reg32.EAX, new Addr32(Reg32.EBP, -lv * 4)));
            return new Addr32(Reg32.EAX, ad.Disp);
        }
    }
}
