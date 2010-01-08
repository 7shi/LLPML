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
        public const int Size = 4;

        private Declare reference;
        protected Declare Reference
        {
            get { return reference; }
            set
            {
                reference = value;
                if (reference == null)
                    throw Abort("undefined variable: " + value.Name);
            }
        }

        public Var() { }
        public Var(BlockBase parent) : base(parent) { }
        public Var(BlockBase parent, Declare var) : base(parent, var.Name) { Reference = var; }
        public Var(BlockBase parent, string name) : base(parent, name) { Reference = parent.GetVar(name); }
        public Var(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequiresName(xr);

            Reference = parent.GetVar(name);
        }

        public virtual Struct.Define GetStruct()
        {
            return Reference.GetStruct();
        }

        public virtual Struct2.Define GetStruct2()
        {
            return Reference.GetStruct2();
        }

        public virtual string Type
        {
            get
            {
                return Reference.Type;
            }
        }

        public virtual Addr32 GetAddress(List<OpCode> codes, Module m)
        {
            if (reference.HasThis)
            {
                return reference.GetAddress(codes, m);
            }
            else
            {
                Addr32 ad = Reference.Address;
                if (parent.Level == Reference.Parent.Level || ad.IsAddress)
                {
                    return ad;
                }
                int lv = Reference.Parent.Level;
                if (lv <= 0 || lv >= parent.Level)
                {
                    throw Abort("Invalid variable scope: " + name);
                }
                codes.Add(I386.Mov(Reg32.EDX, new Addr32(Reg32.EBP, -lv * 4)));
                return new Addr32(Reg32.EDX, ad.Disp);
            }
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            IntValue.AddCodes(codes, op, dest, GetAddress(codes, m));
        }
    }
}
