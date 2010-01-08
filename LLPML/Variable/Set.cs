using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Set : Var.Operator
    {
        public override string Tag { get { return "set"; } }

        public override int Min { get { return 1; } }
        public override int Max { get { return 1; } }

        public Set(BlockBase parent, IIntValue dest) : base(parent, dest) { }

        public Set(BlockBase parent, IIntValue dest, IIntValue value)
            : base(parent, dest)
        {
            this.values.Add(value);
        }

        public Set(BlockBase parent, IIntValue dest, int value)
            : this(parent, dest, new IntValue(value))
        {
        }

        public Set(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpModule codes)
        {
            var dest = Var.Get(this.dest);
            if (dest == null)
            {
                if (this.dest is Variant)
                {
                    var setter = (this.dest as Variant).GetSetter();
                    if (setter != null)
                    {
                        new Call(Parent, setter, new Struct.This(Parent), values[0])
                            .AddCodes(codes);
                        return;
                    }
                }
                throw Abort("set: destination is not variable");
            }
            var dt = dest.Type;
            if (dt is TypeConstChar)
                throw Abort("set: can not change constants");
            if (dest is Struct.Member)
            {
                var mem = dest as Struct.Member;
                if (mem.IsSetter)
                {
                    mem.AddSetterCodes(codes, values[0]);
                    return;
                }
            }

            var v = values[0];
            v.AddCodes(codes, "push", null);
            var ad = dest.GetAddress(codes);
            if (!OpModule.NeedsDtor(v))
            {
                codes.Add(I386.Pop(Reg32.EAX));
                dt.AddSetCodes(codes, ad);
            }
            else
            {
                codes.Add(I386.Mov(Reg32.EAX, new Addr32(Reg32.ESP)));
                dt.AddSetCodes(codes, ad);
                codes.AddDtorCodes(v.Type);
            }
        }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            AddCodes(codes);
            codes.AddCodes(op, dest);
        }
    }
}
