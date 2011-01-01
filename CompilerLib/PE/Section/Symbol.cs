using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Girl.Binary;

namespace Girl.PE
{
    public class Symbol
    {
        public ushort Hint;
        public string Name;

        private Val32 hintAddress = Val32.New(0);

        private Val32 importRef = Val32.NewB(0, true);
        public Val32 ImportRef { get { return importRef; } }

        public static Symbol New(ushort hint, string name)
        {
            var ret = new Symbol();
            ret.Hint = hint;
            ret.Name = name;
            return ret;
        }

        public int NameSize
        {
            get
            {
                return HeaderBase.GetPaddedSize(4, Name);
            }
        }

        public void WriteLookup(Block block, bool lookup)
        {
            if (!lookup) importRef.Value = block.Current;
            block.AddVal32(hintAddress);
        }

        public void Write(Block block)
        {
            hintAddress.Value = block.Current;
            block.AddUShort(Hint);
            block.AddString(HeaderBase.Pad(NameSize, Name));
        }
    }
}
