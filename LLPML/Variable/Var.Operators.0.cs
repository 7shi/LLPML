using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Inc : Var.Operator
    {
        public override string Tag { get { return "inc"; } }

        public override int Min { get { return 0; } }
        public override int Max { get { return 0; } }

        public Inc(BlockBase parent, Var dest) : base(parent, dest) { }
        public Inc(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        private Addr32 Calculate(OpCodes codes)
        {
            var ad1 = dest.GetAddress(codes);
            var ad2 = ad1;
            if (dest.Size < Var.DefaultSize)
            {
                ad2 = new Addr32(Reg32.ESP);
                dest.Type.AddGetCodes(codes, "push", null, ad1);
            }
            GetFunc()(codes, ad2, null);
            if (dest.Size < Var.DefaultSize)
            {
                codes.Add(I386.Pop(Reg32.EAX));
                dest.Type.AddSetCodes(codes, ad1);
            }
            return ad1;
        }

        public override void AddCodes(OpCodes codes)
        {
            Calculate(codes);
        }

        public override void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            var ad = Calculate(codes);
            if (this.dest.Size < Var.DefaultSize)
                codes.AddCodes(op, dest);
            else
                codes.AddCodes(op, dest, ad);
        }
    }

    public class Dec : Inc
    {
        public override string Tag { get { return "dec"; } }
        public Dec(BlockBase parent, Var dest) : base(parent, dest) { }
        public Dec(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class PostInc : Inc
    {
        public override string Tag { get { return "post-inc"; } }

        public PostInc(BlockBase parent, Var dest) : base(parent, dest) { }
        public PostInc(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            var ad1 = this.dest.GetAddress(codes);
            var ad2 = ad1;
            this.dest.Type.AddGetCodes(codes, "push", null, ad1);
            if (this.dest.Size < Var.DefaultSize)
            {
                ad2 = new Addr32(Reg32.ESP);
                codes.Add(I386.Push(ad2));
            }
            GetFunc()(codes, ad2, null);
            if (this.dest.Size < Var.DefaultSize)
            {
                codes.Add(I386.Pop(Reg32.EAX));
                this.dest.Type.AddSetCodes(codes, ad1);
            }
            codes.Add(I386.Pop(Reg32.EAX));
            codes.AddCodes(op, dest);
        }
    }

    public class PostDec : PostInc
    {
        public override string Tag { get { return "post-dec"; } }
        public PostDec(BlockBase parent, Var dest) : base(parent, dest) { }
        public PostDec(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
