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
        public abstract void Write(Block block);

        private Block block;
        public Block Block { get { return block; } }

        public Block ToBlock(uint vaddr)
        {
            Block ret = new Block(vaddr);
            Write(ret);
            block = ret;
            return ret;
        }
    }
}
