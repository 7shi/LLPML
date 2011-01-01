using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.PE
{
    public class AnySection : SectionBase
    {
        private string name;
        public override string Name { get { return name; } }

        public byte[] data;

        public static AnySection New(string name, byte[] data)
        {
            var ret = new AnySection();
            ret.name = name;
            ret.data = data;
            return ret;
        }

        public override void Write(Block block)
        {
            block.AddBytes(data);
        }
    }
}
