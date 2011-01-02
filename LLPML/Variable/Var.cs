using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Var : NodeBase
    {
        public const int DefaultSize = 4;
        public const Reg32 DestRegister = Reg32.ECX;

        public override TypeBase Type { get { return Reference.Type; } }
        public VarDeclare Reference { get; protected set; }

        public Var() { }
        public Var(BlockBase parent) : base(parent) { }

        public Var(BlockBase parent, VarDeclare var)
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

        public virtual Struct.Define GetStruct()
        {
            return Types.GetStruct(Type);
        }

        public virtual Addr32 GetAddress(OpModule codes)
        {
            return Reference.GetAddress(codes, Parent);
        }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            Type.AddGetCodes(codes, op, dest, GetAddress(codes));
        }

        public static Var Get(NodeBase v)
        {
            if (v is Variant)
                return (v as Variant).GetVar();
            else
                return v as Var;
        }
    }
}
