using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Pointer : VarBase
    {
        private Pointer.Define reference;

        private int length = 0;
        public int Length { get { return length; } }

        public Pointer() { }

        public Pointer(Block parent, string name)
            : base(parent, name)
        {
            reference = parent.GetPointer(name);
            if (reference == null)
                throw new Exception("undefined pointer: " + name);
        }

        public Pointer(Block parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            if (!xr.IsEmptyElement)
                throw Abort(xr, "<" + xr.Name + "> can not have any children");

            name = xr["name"];
            if (name == null) throw Abort(xr, "name required");

            reference = parent.GetPointer(name);
            if (reference == null)
                throw Abort(xr, "undefined pointer: " + name);
        }

        public void GetValue(List<OpCode> codes, Module m)
        {
            AddCodes(codes, m);
            Addr32 ad = reference.Address;
            if (parent == reference.Parent || ad.IsAddress)
            {
                codes.Add(I386.Lea(Reg32.EAX, ad));
                return;
            }
            int lv = reference.Parent.Level;
            if (lv <= 0 || lv >= parent.Level)
            {
                throw new Exception("Invalid variable scope: " + name);
            }
            codes.Add(I386.Mov(Reg32.EAX, new Addr32(Reg32.EBP, -lv * 4)));
            codes.Add(I386.Sub(Reg32.EAX, (uint)-ad.Disp));
        }
    }
}
