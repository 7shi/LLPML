using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Expression : Operator
    {
        public override string Tag { get { return "expression"; } }

        public override int Min { get { return 1; } }
        public override int Max { get { return 1; } }

        public Expression(BlockBase parent, IIntValue value) : base(parent, value) { }
        public Expression(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpModule codes)
        {
            if (AddConstCodes(codes, "mov", null)) return;

            var v = values[0];
            var nb = v as NodeBase;
            if (nb != null && !OpModule.NeedsDtor(v))
                nb.AddCodes(codes);
            else
                AddCodes(codes, "mov", null);
        }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            if (AddConstCodes(codes, op, dest)) return;

            var v = values[0];
            if (!OpModule.NeedsDtor(v))
                v.AddCodes(codes, op, dest);
            else
            {
                v.AddCodes(codes, "mov", null);
                codes.Add(I386.Push(Reg32.EAX));
                codes.AddCodes(op, dest);
                codes.AddDtorCodes(v.Type);
            }
        }

        public override IntValue GetConst()
        {
            return IntValue.GetValue(values[0]);
        }
    }
}
