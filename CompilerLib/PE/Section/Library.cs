using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Girl.Binary;

namespace Girl.PE
{
    public class Library
    {
        private string name;
        public string Name { get { return name; } }

        private ImportTable table = new ImportTable();
        private Hashtable symbols = new Hashtable();

        public static Library New(string name)
        {
            var ret = new Library();
            ret.name = name;
            return ret;
        }

        public Symbol Add(string name)
        {
            Symbol sym;
            if (symbols.ContainsKey(name))
            {
                sym = symbols[name] as Symbol;
            }
            else
            {
                sym = Symbol.New(0, name);
                symbols.Add(name, sym);
            }
            return sym;
        }

        public int NameSize
        {
            get
            {
                return HeaderBase.GetPaddedSize(4, name);
            }
        }

        public void WriteImportTable(Block32 block)
        {
            table.WriteBlock(block);
        }

        public void WriteImportLookupTable(Block32 block)
        {
            table.ImportLookupTable = block.Current;
            foreach (var sym in symbols.Values)
                (sym as Symbol).WriteLookup(block, true);
            block.AddInt(0);
        }

        public void WriteImportAddressTable(Block32 block)
        {
            table.ImportAddressTable = block.Current;
            foreach (var sym in symbols.Values)
                (sym as Symbol).WriteLookup(block, false);
            block.AddInt(0);
        }

        public void WriteSymbols(Block32 block)
        {
            foreach (var sym in symbols.Values)
                (sym as Symbol).Write(block);
        }

        public void WriteName(Block32 block)
        {
            table.Name = block.Current;
            block.AddString(HeaderBase.Pad(NameSize, name));
        }
    }
}
