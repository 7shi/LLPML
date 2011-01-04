using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.PE
{
    public abstract class SectionBase
    {
        public SectionHeader Header;

        public abstract string Name { get; }
        public abstract void Write(Block32 block);

        private Block32 block;
        public Block32 Block { get { return block; } }

        public Block32 ToBlock(uint vaddr)
        {
            Block32 ret = Block32.New(vaddr);
            Write(ret);
            block = ret;
            return ret;
        }
    }
}
