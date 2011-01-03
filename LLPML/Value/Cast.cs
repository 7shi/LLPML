using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.LLPML.Parsing;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Cast : Var
    {
        public NodeBase Source { get; private set; }

        private string type;
        public override TypeBase Type
        {
            get
            {
                var t = Types.GetType(Parent, type);
                if (!(Source is Call
                    || (Source.Type != null && Source.Type.IsValue)))
                    return t;
                return Types.ToVarType(t);
            }
        }

        public static Cast New(BlockBase parent, string type, NodeBase source)
        {
            var ret = new Cast();
            ret.Parent = parent;
            ret.name = "__cast";
            ret.type = type;
            ret.Source = source;
            return ret;
        }

        public override Addr32 GetAddress(OpModule codes)
        {
            if (Source is Var)
                return (Source as Var).GetAddress(codes);
            else if (Source is IntValue)
                codes.Add(I386.MovR(Var.DestRegister,
                    Val32.NewI((Source as IntValue).Value)));
            else if (Source is StringValue)
                codes.Add(I386.MovR(Var.DestRegister,
                    codes.GetString((Source as StringValue).Value)));
            else
            {
                Source.AddCodesV(codes, "mov", null);
                return null;
            }
            return Addr32.New(Var.DestRegister);
        }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            var t = Type;
            var st = Source.Type;
            if (st is TypeIntBase && t.Size < st.Size)
            {
                Source.AddCodesV(codes, "mov", null);
                t.AddGetCodes(codes, op, dest, null);
            }
            else
                Source.AddCodesV(codes, op, dest);
        }

        public NodeBase GetSource()
        {
            var ret = Source;
            if (ret == null || !(ret is Cast)) return ret;
            return (ret as Cast).GetSource();
        }
    }
}
