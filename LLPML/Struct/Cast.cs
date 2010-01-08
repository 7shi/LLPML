using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Cast : Var
    {
        public IIntValue Source { get; private set; }

        private string type;
        public override TypeBase Type
        {
            get
            {
                var t = Types.GetType(parent, type);
                if (!Source.Type.IsValue) return t;
                return Types.ConvertVarType(t);
            }
        }

        public Cast(BlockBase parent, string type, IIntValue source)
        {
            this.parent = parent;
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
                IIntValue[] v = IntValue.Read(parent, xr);
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

        public override Addr32 GetAddress(OpCodes codes)
        {
            if (Source is Var)
                return (Source as Var).GetAddress(codes);
            else if (Source is IntValue)
                codes.Add(I386.Mov(Var.DestRegister,
                    (uint)(Source as IntValue).Value));
            else if (Source is StringValue)
                codes.Add(I386.Mov(Var.DestRegister,
                    codes.Module.GetString((Source as StringValue).Value)));
            else
            {
                Source.AddCodes(codes, "mov", null);
                codes.Add(I386.Mov(Var.DestRegister, Reg32.EAX));
            }
            return new Addr32(Var.DestRegister);
        }

        public override void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            Source.AddCodes(codes, op, dest);
        }
    }
}
