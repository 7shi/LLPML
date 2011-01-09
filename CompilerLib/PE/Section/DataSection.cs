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

        private Block32 block = new Block32();
        public Block32 Block { get { return block; } }

        public static DataBlock New(byte[] d)
        {
            var ret = new DataBlock();
            ret.block.AddBytes(d);
            return ret;
        }

        public void Write(Block32 block)
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

        private ListDictionary data = new ListDictionary();

        public bool IsEmtpy { get { return data.Count == 0; } }

        private ListDictionary GetCategory(string name)
        {
            if (data.ContainsKey(name)) return data.Get(name) as ListDictionary;

            var ret = new ListDictionary();
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
            if (ctg.ContainsKey(name)) return ctg.Get(name) as DataBlock;

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

        public override void Write(Block32 block)
        {
            foreach (ListDictionary ctg in data.Values)
                foreach (var db in ctg.Values)
                    (db as DataBlock).Write(block);
            if (IsEmtpy) block.AddBytes(new byte[16]);
        }
    }

    public class ListDictionary
    {
        private Hashtable dict = new Hashtable();
        private ArrayList list = new ArrayList();

        public void Add(string key, object value)
        {
            dict.Add(key, value);
            list.Add(value);
        }

        public int Count { get { return list.Count; } }

        public Object[] Values
        {
            get { return list.ToArray(); }
        }

        public object Get(string name)
        {
            if (dict.ContainsKey(name))
                return dict[name];
            else
                return null;
        }

        public bool ContainsKey(string name)
        {
            return dict.ContainsKey(name);
        }
    }
}
