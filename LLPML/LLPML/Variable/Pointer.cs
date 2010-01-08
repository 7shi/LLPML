using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Pointer : VarBase, IIntValue
    {
        private Declare reference;

        private int length = 0;
        public int Length { get { return length; } }

        public Pointer() { }

        public Pointer(BlockBase parent, string name)
            : base(parent, name)
        {
            reference = parent.GetPointer(name);
            if (reference == null)
                throw Abort("undefined pointer: " + name);
        }

        public Pointer(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequiresName(xr);

            reference = parent.GetPointer(name);
            if (reference == null)
                throw Abort(xr, "undefined pointer: " + name);
        }

        public virtual string Type
        {
            get
            {
                Struct.Declare st = reference as Struct.Declare;
                return st == null ? null : st.Type;
            }
        }

        public virtual void GetValue(List<OpCode> codes, Module m)
        {
            Addr32 ad = reference.Address;
            if (parent.Level == reference.Parent.Level || ad.IsAddress)
            {
                codes.Add(I386.Lea(Reg32.EAX, ad));
                return;
            }
            int lv = reference.Parent.Level;
            if (lv <= 0 || lv >= parent.Level)
            {
                throw Abort("Invalid variable scope: " + name);
            }
            codes.Add(I386.Mov(Reg32.EAX, new Addr32(Reg32.EBP, -lv * 4)));
            codes.Add(I386.Sub(Reg32.EAX, (uint)-ad.Disp));
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            GetValue(codes, m);
            IntValue.AddCodes(codes, op, dest);
        }
    }
}
