﻿using System;
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

        private Ptr<uint> hintAddress = 0;

        private Ptr<uint> importRef = new Ptr<uint>(0, true);
        public Ptr<uint> ImportRef { get { return importRef; } }

        public Symbol(ushort hint, string name)
        {
            Hint = hint;
            Name = name;
        }

        public int NameSize
        {
            get
            {
                return HeaderBase.GetPaddedSize(4, Name);
            }
        }

        public void Write(Block block, bool lookup)
        {
            if (!lookup) importRef.Value = block.Current;
            block.Add(hintAddress);
        }

        public void Write(Block block)
        {
            hintAddress.Value = block.Current;
            block.Add(Hint);
            block.Add(HeaderBase.Pad(NameSize, Name));
        }
    }
}
