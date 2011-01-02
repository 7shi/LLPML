using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class VarAdd : VarOperator
    {
        public override string Tag { get { return "add"; } }

        public VarAdd(BlockBase parent, NodeBase dest, params NodeBase[] values) : base(parent, dest, values) { }
        public VarAdd(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

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

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            var ad = Calculate(codes);
            if (this.dest.Type.Size < Var.DefaultSize)
                codes.AddCodes(op, dest);
            else
                codes.AddCodesA(op, dest, ad);
        }
    }

    public class VarSub : VarAdd
    {
        public override string Tag { get { return "sub"; } }
        public VarSub(BlockBase parent, NodeBase dest, params NodeBase[] values) : base(parent, dest, values) { }
        public VarSub(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class VarAnd : VarAdd
    {
        public override string Tag { get { return "and"; } }
        public VarAnd(BlockBase parent, NodeBase dest, params NodeBase[] values) : base(parent, dest, values) { }
        public VarAnd(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class VarOr : VarAdd
    {
        public override string Tag { get { return "or"; } }
        public VarOr(BlockBase parent, NodeBase dest, params NodeBase[] values) : base(parent, dest, values) { }
        public VarOr(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class VarXor : VarAdd
    {
        public override string Tag { get { return "xor"; } }
        public VarXor(BlockBase parent, NodeBase dest, params NodeBase[] values) : base(parent, dest, values) { }
        public VarXor(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class VarShiftLeft : VarAdd
    {
        public override string Tag { get { return "shift-left"; } }
        public VarShiftLeft(BlockBase parent, NodeBase dest, params NodeBase[] values) : base(parent, dest, values) { }
        public VarShiftLeft(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class VarShiftRight : VarShiftLeft
    {
        public override string Tag { get { return "shift-right"; } }
        public VarShiftRight(BlockBase parent, NodeBase dest, params NodeBase[] values) : base(parent, dest, values) { }
        public VarShiftRight(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class VarMul : VarAdd
    {
        public override string Tag { get { return "mul"; } }
        public VarMul(BlockBase parent, NodeBase dest, params NodeBase[] values) : base(parent, dest, values) { }
        public VarMul(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class VarDiv : VarAdd
    {
        public override string Tag { get { return "div"; } }
        public VarDiv(BlockBase parent, NodeBase dest, params NodeBase[] values) : base(parent, dest, values) { }
        public VarDiv(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }

    public class VarMod : VarAdd
    {
        public override string Tag { get { return "mod"; } }
        public VarMod(BlockBase parent, NodeBase dest, params NodeBase[] values) : base(parent, dest, values) { }
        public VarMod(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
