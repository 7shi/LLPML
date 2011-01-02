using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class VarOperator1 : VarOperator
    {
        private Addr32 Calculate(OpModule codes)
        {
            var dest = Var.Get(this.dest);
            if (dest == null)
                throw Abort("{0}: destination is not variable", Tag);
            var ad = dest.GetAddress(codes);
            var ad2 = ad;
            var f = GetFunc();
            TypeBase.Func schar = null, sint = null;
            if (dest.Type is TypeString)
            {
                schar = dest.Type.GetFunc(Tag + "-char");
                sint = dest.Type.GetFunc(Tag + "-int");
            }
            var size = dest.Type.Size;
            var cleanup = OpModule.NeedsDtor(dest);
            var indirect = (dest.Reference != null && dest.Reference.Parent != Parent)
                || size < Var.DefaultSize || cleanup;
            if (indirect)
            {
                if (!ad.IsAddress && ad.Register != Reg32.EBP)
                    codes.Add(I386.Push(ad.Register));
                dest.Type.AddGetCodes(codes, "push", null, ad);
                ad2 = Addr32.New(Reg32.ESP);
            }
            foreach (NodeBase v in values)
            {
                var vv = v;
                var ff = f;
                if (schar != null && vv.Type is TypeChar)
                    ff = schar;
                else if (sint != null && vv.Type is TypeIntBase)
                    ff = sint;
                codes.AddOperatorCodes(ff, ad2, vv, false);
            }
            if (indirect)
            {
                codes.Add(I386.Pop(Reg32.EAX));
                if (!ad.IsAddress && ad.Register != Reg32.EBP)
                    codes.Add(I386.Pop(ad.Register));
                dest.Type.AddSetCodes(codes, ad);
            }
            return ad;
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

    public class VarAdd : VarOperator1
    {
        public override string Tag { get { return "add"; } }

        public static VarAdd New(BlockBase parent, NodeBase dest, NodeBase arg)
        {
            return Init1(new VarAdd(), parent, dest, arg) as VarAdd;
        }
    }

    public class VarSub : VarOperator1
    {
        public override string Tag { get { return "sub"; } }

        public static VarSub New(BlockBase parent, NodeBase dest, NodeBase arg)
        {
            return Init1(new VarSub(), parent, dest, arg) as VarSub;
        }
    }

    public class VarAnd : VarOperator1
    {
        public override string Tag { get { return "and"; } }

        public static VarAnd New(BlockBase parent, NodeBase dest, NodeBase arg)
        {
            return Init1(new VarAnd(), parent, dest, arg) as VarAnd;
        }
    }

    public class VarOr : VarOperator1
    {
        public override string Tag { get { return "or"; } }

        public static VarOr New(BlockBase parent, NodeBase dest, NodeBase arg)
        {
            return Init1(new VarOr(), parent, dest, arg) as VarOr;
        }
    }

    public class VarXor : VarOperator1
    {
        public override string Tag { get { return "xor"; } }

        public static VarXor New(BlockBase parent, NodeBase dest, NodeBase arg)
        {
            return Init1(new VarXor(), parent, dest, arg) as VarXor;
        }
    }

    public class VarShiftLeft : VarOperator1
    {
        public override string Tag { get { return "shift-left"; } }

        public static VarShiftLeft New(BlockBase parent, NodeBase dest, NodeBase arg)
        {
            return Init1(new VarShiftLeft(), parent, dest, arg) as VarShiftLeft;
        }
    }

    public class VarShiftRight : VarOperator1
    {
        public override string Tag { get { return "shift-right"; } }

        public static VarShiftRight New(BlockBase parent, NodeBase dest, NodeBase arg)
        {
            return Init1(new VarShiftRight(), parent, dest, arg) as VarShiftRight;
        }
    }

    public class VarMul : VarOperator1
    {
        public override string Tag { get { return "mul"; } }

        public static VarMul New(BlockBase parent, NodeBase dest, NodeBase arg)
        {
            return Init1(new VarMul(), parent, dest, arg) as VarMul;
        }
    }

    public class VarDiv : VarOperator1
    {
        public override string Tag { get { return "div"; } }

        public static VarDiv New(BlockBase parent, NodeBase dest, NodeBase arg)
        {
            return Init1(new VarDiv(), parent, dest, arg) as VarDiv;
        }
    }

    public class VarMod : VarOperator1
    {
        public override string Tag { get { return "mod"; } }

        public static VarMod New(BlockBase parent, NodeBase dest, NodeBase arg)
        {
            return Init1(new VarMod(), parent, dest, arg) as VarMod;
        }
    }
}
