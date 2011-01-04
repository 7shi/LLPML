using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Girl.Binary
{
    public abstract class WriterBase
    {
        public abstract void WriteBlock(Block32 block);

        public Block32 ToBlock()
        {
            var ret = new Block32();
            WriteBlock(ret);
            return ret;
        }

        public void Write(BinaryWriter bw)
        {
            ToBlock().Write(bw);
        }
    }
}
