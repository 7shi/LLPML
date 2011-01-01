using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.PE
{
    public class TextSection : SectionBase
    {
        public override string Name { get { return ".text"; } }

        public OpCode[] OpCodes;

        public override void Write(Block block)
        {
            for (int i = 0; i < OpCodes.Length; i++)
            {
                var op = OpCodes[i];
                op.Address.Value = block.Current;
                op.Write(block);
            }
        }
    }
}
