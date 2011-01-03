using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;
using Girl.LLPML.Parsing;

namespace Girl.LLPML
{
    public abstract class VarOperatorPre : VarOperator
    {
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
            CheckFunc().AddOpCodes(Tag, codes, ad2);
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

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            var ad = Calculate(codes);
            if (this.dest.Type.Size < Var.DefaultSize)
                codes.AddCodes(op, dest);
            else
                codes.AddCodesA(op, dest, ad);
        }
    }

    public class Inc : VarOperatorPre
    {
        public override string Tag { get { return "inc"; } }

        public static Inc New(BlockBase parent, NodeBase dest, SrcInfo si)
        {
            return Init0(new Inc(), parent, dest, si) as Inc;
        }
    }

    public class Dec : VarOperatorPre
    {
        public override string Tag { get { return "dec"; } }

        public static Dec New(BlockBase parent, NodeBase dest, SrcInfo si)
        {
            return Init0(new Dec(), parent, dest, si) as Dec;
        }
    }

    public abstract class VarOperatorPost : VarOperatorPre
    {
        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
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
            CheckFunc().AddOpCodes(Tag, codes, ad2);
            if (thisdest.Type.Size < Var.DefaultSize)
            {
                codes.Add(I386.Pop(Reg32.EAX));
                thisdest.Type.AddSetCodes(codes, ad1);
            }
            codes.Add(I386.Pop(Reg32.EAX));
            codes.AddCodes(op, dest);
        }
    }

    public class PostInc : VarOperatorPost
    {
        public override string Tag { get { return "post-inc"; } }

        public static PostInc New(BlockBase parent, NodeBase dest)
        {
            return Init0(new PostInc(), parent, dest, null) as PostInc;
        }
    }

    public class PostDec : VarOperatorPost
    {
        public override string Tag { get { return "post-dec"; } }

        public static PostDec New(BlockBase parent, NodeBase dest)
        {
            return Init0(new PostDec(), parent, dest, null) as PostDec;
        }
    }
}
