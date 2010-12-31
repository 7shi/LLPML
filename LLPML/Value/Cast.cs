using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Cast : Var
    {
        public IIntValue Source { get; private set; }

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

        public Cast(BlockBase parent, string type, IIntValue source)
        {
            this.Parent = parent;
            name = "__cast";
            this.type = type;
            Source = source;
        }

        public Cast(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            name = "__cast";
            this.type = xr["type"];

            Parse(xr, delegate
            {
                IIntValue[] v = IntValue.Read(Parent, xr);
                if (v != null)
                {
                    if (v.Length > 1 || Source != null)
                        throw Abort(xr, "too many sources");
                    Source = v[0];
                }
            });

            if (Source == null)
                throw Abort(xr, "requires a source");
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
                Source.AddCodes(codes, "mov", null);
                return null;
            }
            return Addr32.New(Var.DestRegister);
        }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            var t = Type;
            var st = Source.Type;
            if (st is TypeIntBase && t.Size < st.Size)
            {
                Source.AddCodes(codes, "mov", null);
                t.AddGetCodes(codes, op, dest, null);
            }
            else
                Source.AddCodes(codes, op, dest);
        }

        public IIntValue GetSource()
        {
            var ret = Source;
            if (ret == null || !(ret is Cast)) return ret;
            return (ret as Cast).GetSource();
        }
    }
}
