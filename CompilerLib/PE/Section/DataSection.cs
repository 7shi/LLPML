using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.PE
{
    public class ByteData
    {
        private ValueWrap address = new ValueWrap(0, true);
        public ValueWrap Address { get { return address; } }

        private byte[] data;
        public byte[] Data { get { return data; } }

        public ByteData(byte[] d) { data = d; }

        public void Write(Block block)
        {
            address.Value = block.Current;
            block.Add(data);
            uint padlen = Module.Align((uint)data.Length, 4) - (uint)data.Length;
            if (padlen > 0) block.Add(new byte[padlen]);
        }
    }

    public class DataSection : SectionBase
    {
        private string name;
        public override string Name { get { return name; } }

        public DataSection(string name)
        {
            this.name = name;
        }

        private Dictionary<string, Dictionary<string, ByteData>> data =
            new Dictionary<string, Dictionary<string, ByteData>>();

        public bool IsEmtpy { get { return data.Count == 0; } }

        private Dictionary<string, ByteData> GetCategory(string name)
        {
            Dictionary<string, ByteData> ret;
            if (data.ContainsKey(name))
            {
                ret = data[name];
            }
            else
            {
                ret = new Dictionary<string, ByteData>();
                data.Add(name, ret);
            }
            return ret;
        }

        public ByteData Add(string category, string name, byte[] data)
        {
            Dictionary<string, ByteData> ctg = GetCategory(category);
            ByteData ret;
            if (ctg.ContainsKey(name))
            {
                ret = ctg[name];
            }
            else
            {
                ret = new ByteData(data);
                ctg.Add(name, ret);
            }
            return ret;
        }

        public ByteData AddString(string s)
        {
            return Add("string", s, Module.DefaultEncoding.GetBytes(s + "\0"));
        }

        public ByteData AddBuffer(string name, int size)
        {
            return Add("buffer", name, new byte[size]);
        }

        public override void Write(Block block)
        {
            foreach (Dictionary<string, ByteData> ctg in data.Values)
            {
                foreach (ByteData bd in ctg.Values)
                {
                    bd.Write(block);
                }
            }
            if (IsEmtpy) block.Add(new byte[16]);
        }
    }
}
