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
        public const int DefaultSize = 4;
        public virtual int Size { get { return reference.Length; } }

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

        public override Struct.Define GetStruct()
        {
            return Reference.GetStruct();
        }

        public override bool IsArray { get { return Reference.IsArray; } }
        public override TypeBase Type { get { return Reference.Type; } }
        public override string TypeName { get { return Reference.TypeName; } }

        public override int TypeSize
        {
            get
            {
                int vsz = SizeOf.GetValueSize(TypeName);
                if (vsz > 0) return vsz;

                var st = GetStruct();
                if (st != null) return st.GetSize();

                return 0;
            }
        }

        public override Addr32 GetAddress(OpCodes codes)
        {
            return reference.GetAddress(codes, parent);
        }

        public virtual void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            var ad = GetAddress(codes);
            var t = Type;
            if (t != null)
                t.AddGetCodes(codes, op, dest, ad);
            else
                codes.AddCodes(op, dest, ad);
        }
    }
}
