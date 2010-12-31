using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Girl.Binary
{
    public abstract class WriterBase
    {
        public abstract void WriteBlock(Block block);

        public Block ToBlock()
        {
            var ret = new Block();
            WriteBlock(ret);
            return ret;
        }

        public void Write(BinaryWriter bw)
        {
            ToBlock().Write(bw);
        }
    }
}
