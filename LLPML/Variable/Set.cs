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

        public Set(BlockBase parent, Var dest) : base(parent, dest) { }

        public Set(BlockBase parent, Var dest, IIntValue value)
            : base(parent, dest)
        {
            this.values.Add(value);
        }

        public Set(BlockBase parent, Var dest, int value)
            : this(parent, dest, new IntValue(value))
        {
        }

        public Set(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpCodes codes)
        {
            values[0].AddCodes(codes, "push", null);
            var ad = dest.GetAddress(codes);
            codes.Add(I386.Pop(Reg32.EAX));
            AddCodes(dest.Size, codes, ad);
        }

        public override void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            AddCodes(codes);
            codes.AddCodes(op, dest);
        }

        public static void AddCodes(int size, OpCodes codes, Addr32 ad)
        {
            switch (size)
            {
                case 2:
                    codes.Add(I386.MovW(ad, Reg16.AX));
                    break;
                case 1:
                    codes.Add(I386.MovB(ad, Reg8.AL));
                    break;
                default:
                    codes.Add(I386.Mov(ad, Reg32.EAX));
                    break;
            }
        }
    }
}
