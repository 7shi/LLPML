using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Do : BlockBase
    {
        public Cond Cond { get; set; }
        public Block Block { get; set; }

        public override bool AcceptsBreak { get { return true; } }
        public override bool AcceptsContinue { get { return true; } }
        public override Val32 Continue { get { return Block.Last; } }

        public static Do New(BlockBase parent)
        {
            var ret = new Do();
            ret.init(parent);
            return ret;
        }

        public override void AddCodes(OpModule codes)
        {
            sentences.Clear();
            Cond.First = Block.First;
            AddSentence(Block);
            AddSentence(Cond);
            base.AddCodes(codes);
        }
    }
}
