using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.LLPML.Struct;
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

        public static Var New(BlockBase parent, VarDeclare var)
        {
            var ret = new Var();
            ret.Parent = parent;
            ret.name = var.Name;
            ret.Reference = var;
            return ret;
        }

        public static Var NewName(BlockBase parent, string name)
        {
            var ret = new Var();
            ret.Parent = parent;
            ret.name = name;
            ret.Reference = parent.GetVar(name);
            if (ret.Reference == null)
                throw ret.Abort("undefined pointer: " + name);
            return ret;
        }

        public virtual Define GetStruct()
        {
            return Types.GetStruct(Type);
        }

        public virtual Addr32 GetAddress(OpModule codes)
        {
            return Reference.GetAddress(codes, Parent);
        }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
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
