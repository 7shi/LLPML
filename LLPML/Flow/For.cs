using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class For : BlockBase
    {
        public Block Init;
        public Cond Cond;
        public Block Loop, Block;

        public override bool AcceptsBreak { get { return true; } }
        public override bool AcceptsContinue { get { return true; } }
        public override Val32 Continue { get { return Block.Last; } }

        public static For New(BlockBase parent)
        {
            var ret = new For();
            ret.init(parent);
            return ret;
        }

        protected override void BeforeAddCodes(OpModule codes)
        {
            base.BeforeAddCodes(codes);
            if (Init != null) Init.AddCodes(codes);
            if (Loop != null)
                codes.Add(I386.JmpD(Loop.Last));
            else
                codes.Add(I386.JmpD(Block.Last));
        }

        public override void AddCodes(OpModule codes)
        {
            sentences.Clear();
            AddSentence(Block);
            if (Loop != null) AddSentence(Loop);
            if (Cond == null)
                Cond = Cond.New(this, IntValue.New(1));
            Cond.First = Block.First;
            AddSentence(Cond);
            base.AddCodes(codes);
        }
    }
}
