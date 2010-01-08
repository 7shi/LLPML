using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class OpModule : OpCodes
    {
        public Module Module { get; private set; }
        public Root Root { get; private set; }

        public OpModule(Module m, Root r)
        {
            Module = m;
            Root = r;
        }

        public Val32 GetData(string category, string name, byte[] data)
        {
            return Module.RData.Add(category, name, data).Address;
        }

        private Dictionary<string, Val32> strings = new Dictionary<string, Val32>();

        public Val32 GetString(string s)
        {
            if (strings.ContainsKey(s)) return strings[s];

            var db = new Girl.Binary.Block();
            db.Add(Module.DefaultEncoding.GetBytes(s + "\0"));
            return strings[s] = AddData("string_constant", s, 0u, 2u, (uint)s.Length, db);
        }

        public Val32 AddData(string category, string name, Val32 dtor, Val32 size, Val32 len, Girl.Binary.Block data)
        {
            var db = new DataBlock();
            db.Block.Add(dtor);
            db.Block.Add(0);
            db.Block.Add(size);
            db.Block.Add(len);
            var offset = db.Block.Length;
            db.Block.Add(data);
            Module.RData.Add(category, name, db);
            return new Val32(db.Address, offset);
        }
    }
}
