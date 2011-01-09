using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Girl.Binary;

namespace Girl.PE
{
    public class ImportSection : SectionBase
    {
        public override string Name { get { return ".idata"; } }

        private ListDictionary libraries = new ListDictionary();

        public Symbol Add(string libname, string sym)
        {
            Library lib;
            if (libraries.ContainsKey(libname))
            {
                lib = libraries.Get(libname) as Library;
            }
            else
            {
                lib = Library.New(libname);
                libraries.Add(libname, lib);
            }
            return lib.Add(sym);
        }

        public override void Write(Block32 block)
        {
            foreach (Library lib in libraries.Values) lib.WriteImportTable(block);
            new ImportTable().WriteBlock(block);

            foreach (Library lib in libraries.Values) lib.WriteImportLookupTable(block);
            foreach (Library lib in libraries.Values) lib.WriteImportAddressTable(block);
            foreach (Library lib in libraries.Values) lib.WriteSymbols(block);
            foreach (Library lib in libraries.Values) lib.WriteName(block);
        }
    }
}
