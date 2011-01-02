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

        public For(BlockBase parent) : base(parent) { }

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
                Cond = new Cond(this, new IntValue(1));
            Cond.First = Block.First;
            AddSentence(Cond);
            base.AddCodes(codes);
        }
    }
}
