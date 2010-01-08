using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Var : VarBase, IIntValue
    {
        protected Declare reference;
        public Declare Reference { get { return reference; } }

        public Var() { }

        public Var(Block parent, string name)
            : base(parent, name)
        {
            reference = parent.GetVar(name);
            if (reference == null)
                throw new Exception("undefined variable: " + name);
        }

        public Var(Block parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequiresName(xr);

            reference = parent.GetVar(name);
            if (reference == null)
                throw Abort(xr, "undefined variable: " + name);
        }

        public virtual Addr32 GetAddress(List<OpCode> codes, Module m)
        {
            Addr32 ad = reference.Address;
            if (parent.Level == reference.Parent.Level || ad.IsAddress)
            {
                return ad;
            }
            int lv = reference.Parent.Level;
            if (lv <= 0 || lv >= parent.Level)
            {
                throw new Exception("Invalid variable scope: " + name);
            }
            codes.Add(I386.Mov(Reg32.EDX, new Addr32(Reg32.EBP, -lv * 4)));
            return new Addr32(Reg32.EDX, ad.Disp);
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            IntValue.AddCodes(codes, op, dest, GetAddress(codes, m));
        }
    }
}