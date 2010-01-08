using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class BlockName : IIntValue
    {
        private BlockBase parent;

        public BlockName(BlockBase parent)
        {
            this.parent = parent;
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            IntValue.AddCodes(codes, op, dest, m.GetString(parent.Name));
        }
    }
}
