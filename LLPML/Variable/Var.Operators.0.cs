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

        public Inc(BlockBase parent, NodeBase dest) : base(parent, dest) { }
        public Inc(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        private Addr32 Calculate(OpModule codes)
        {
            var dest = Var.Get(this.dest);
            if (dest == null)
                throw Abort("{0}: destination is not variable", Tag);
            var ad1 = dest.GetAddress(codes);
            var ad2 = ad1;
            if (dest.Type.Size < Var.DefaultSize)
            {
                ad2 = Addr32.New(Reg32.ESP);
                dest.Type.AddGetCodes(codes, "push", null, ad1);
            }
            GetFunc()(codes, ad2);
            if (dest.Type.Size < Var.DefaultSize)
            {
                codes.Add(I386.Pop(Reg32.EAX));
                dest.Type.AddSetCodes(codes, ad1);
            }
            return ad1;
        }

        public override void AddCodes(OpModule codes)
        {
            Calculate(codes);
        }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            var ad = Calculate(codes);
            if (this.dest.Type.Size < Var.DefaultSize)
                codes.AddCodes(op, dest);
            else
                codes.AddCodesA(op, dest, ad);
        }
    }

    public class Dec : Inc
    {
        public override string Tag { get { return "dec"; } }
        public Dec(BlockBase parent, NodeBase dest) : base(parent, dest) { }
        public Dec(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class PostInc : Inc
    {
        public override string Tag { get { return "post-inc"; } }

        public PostInc(BlockBase parent, NodeBase dest) : base(parent, dest) { }
        public PostInc(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            var thisdest = Var.Get(this.dest);
            if (thisdest == null)
                throw Abort("{0}: destination is not variable", Tag);
            var ad1 = thisdest.GetAddress(codes);
            var ad2 = ad1;
            thisdest.Type.AddGetCodes(codes, "push", null, ad1);
            if (thisdest.Type.Size < Var.DefaultSize)
            {
                ad2 = Addr32.New(Reg32.ESP);
                codes.Add(I386.PushA(ad2));
            }
            GetFunc()(codes, ad2);
            if (thisdest.Type.Size < Var.DefaultSize)
            {
                codes.Add(I386.Pop(Reg32.EAX));
                thisdest.Type.AddSetCodes(codes, ad1);
            }
            codes.Add(I386.Pop(Reg32.EAX));
            codes.AddCodes(op, dest);
        }
    }

    public class PostDec : PostInc
    {
        public override string Tag { get { return "post-dec"; } }
        public PostDec(BlockBase parent, NodeBase dest) : base(parent, dest) { }
        public PostDec(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
