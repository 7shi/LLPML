using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Var : NodeBase, IIntValue
    {
        public const int DefaultSize = 4;
        public const Reg32 DestRegister = Reg32.ECX;

        public virtual TypeBase Type { get { return Reference.Type; } }
        public Var.Declare Reference { get; protected set; }

        public Var() { }
        public Var(BlockBase parent) : base(parent) { }

        public Var(BlockBase parent, Declare var)
            : base(parent, var.Name)
        {
            Reference = var;
        }

        public Var(BlockBase parent, string name)
            : base(parent, name)
        {
            Reference = parent.GetVar(name);
            if (Reference == null)
                throw Abort("undefined pointer: " + name);
        }

        public Var(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequiresName(xr);

            Reference = Parent.GetVar(name);
            if (Reference == null)
                throw Abort(xr, "undefined variable: " + name);
        }

        public virtual Struct.Define GetStruct()
        {
            return Types.GetStruct(Type);
        }

        public virtual Addr32 GetAddress(OpModule codes)
        {
            return Reference.GetAddress(codes, Parent);
        }

        public virtual void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            Type.AddGetCodes(codes, op, dest, GetAddress(codes));
        }
    }
}
