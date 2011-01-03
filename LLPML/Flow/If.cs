using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class If : BlockBase
    {
        public class CondBlock : NodeBase
        {
            public Cond Cond { get; set; }
            public Block Block { get; set; }
            public CondBlock Next { get; set; }

            private OpCode first = new OpCode();
            public Val32 First { get { return first.Address; } }

            public CondBlock(If parent) { Parent = parent; }

            public override void AddCodes(OpModule codes)
            {
                var next = new OpCode();
                codes.Add(first);
                if (Cond != null)
                {
                    Cond.Next = next.Address;
                    Cond.AddCodes(codes);
                }
                Block.AddCodes(codes);
                if (Next != null)
                    codes.Add(I386.JmpD(Parent.Destruct));
                codes.Add(next);
            }
        }

        private ArrayList blocks = new ArrayList();
        public ArrayList Blocks { get { return blocks; } }

        public static If New(BlockBase parent)
        {
            var ret = new If();
            ret.init(parent);
            return ret;
        }

        public override void AddCodes(OpModule codes)
        {
            sentences.Clear();
            int len = blocks.Count;
            for (int i = 0; i < len; i++)
            {
                var cb = blocks[i] as CondBlock;
                if (i < len - 1)
                    cb.Next = blocks[i + 1] as CondBlock;
                else
                    cb.Next = null;
                AddSentence(cb);
            }
            base.AddCodes(codes);
        }
    }
}
