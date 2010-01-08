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
                if (value == null)
                    throw Abort("undefined variable: " + name);
                reference = value;
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

        public virtual string Type
        {
            get
            {
                return Reference.Type;
            }
        }

        public virtual Addr32 GetAddress(List<OpCode> codes, Module m)
        {
            return reference.GetAddress(codes, m, parent);
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            IntValue.AddCodes(codes, op, dest, GetAddress(codes, m));
        }
    }
}
