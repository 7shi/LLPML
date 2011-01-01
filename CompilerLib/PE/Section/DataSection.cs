using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.PE
{
    public class DataBlock
    {
        private Val32 address = Val32.NewB(0, true);
        public Val32 Address { get { return address; } }

        private Block block = new Block();
        public Block Block { get { return block; } }

        public static DataBlock New(byte[] d)
        {
            var ret = new DataBlock();
            ret.block.AddBytes(d);
            return ret;
        }

        public void Write(Block block)
        {
            address.Value = block.Current;
            block.AddBlock(Block);
            uint padlen = Module.Align((uint)Block.Length, 4) - (uint)Block.Length;
            if (padlen > 0) block.AddBytes(new byte[padlen]);
        }
    }

    public class DataSection : SectionBase
    {
        private string name;
        public override string Name { get { return name; } }

        public static DataSection New(string name)
        {
            var ret = new DataSection();
            ret.name = name;
            return ret;
        }

        private Hashtable data = new Hashtable();

        public bool IsEmtpy { get { return data.Count == 0; } }

        private Hashtable GetCategory(string name)
        {
            if (data.ContainsKey(name)) return data[name] as Hashtable;

            var ret = new Hashtable();
            data.Add(name, ret);
            return ret;
        }

        public void AddDataBlock(string category, string name, DataBlock data)
        {
            var ctg = GetCategory(category);
            if (!ctg.ContainsKey(name)) ctg.Add(name, data);
        }

        public DataBlock Add(string category, string name, byte[] data)
        {
            var ctg = GetCategory(category);
            if (ctg.ContainsKey(name)) return ctg[name] as DataBlock;

            var ret = DataBlock.New(data);
            ctg.Add(name, ret);
            return ret;
        }

        public DataBlock AddString(string s)
        {
            return Add("string", s, Module.EncodeString(s));
        }

        public DataBlock AddBuffer(string name, int size)
        {
            return Add("buffer", name, new byte[size]);
        }

        public override void Write(Block block)
        {
            foreach (Hashtable ctg in data.Values)
                foreach (var db in ctg.Values)
                    (db as DataBlock).Write(block);
            if (IsEmtpy) block.AddBytes(new byte[16]);
        }
    }
}
