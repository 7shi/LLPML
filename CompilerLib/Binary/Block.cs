using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Girl.Binary
{
    public class Byte
    {
        public byte Value { get; private set; }
        public static Byte New(byte v) { var ret = new Byte(); ret.Value = v; return ret; }
    }

    public class UShort
    {
        public ushort Value { get; private set; }
        public static UShort New(ushort v) { var ret = new UShort(); ret.Value = v; return ret; }
    }

    public class UInt
    {
        public uint Value { get; private set; }
        public static UInt New(uint v) { var ret = new UInt(); ret.Value = v; return ret; }
    }

    public class Int
    {
        public int Value { get; private set; }
        public static Int New(int v) { var ret = new Int(); ret.Value = v; return ret; }
    }

    public class Block
    {
        private ArrayList data;
        private uint length;
        public uint Address;
        private List<uint> relocs;

        public uint Length { get { return length; } }
        public uint Current { get { return Address + length; } }
        public uint[] Relocations { get { return relocs.ToArray(); } }

        public Block()
        {
            data = new ArrayList();
            relocs = new List<uint>();
        }

        public static Block New(uint addr) { var ret = new Block(); ret.Address = addr; return ret; }

        public void AddByte(byte v) { data.Add(Byte.New(v)); length += sizeof(byte); }
        public void AddByte2(Byte v) { data.Add(v); length += sizeof(byte); }
        public void AddUShort(ushort v) { data.Add(UShort.New(v)); length += sizeof(ushort); }
        public void AddUShort2(UShort v) { data.Add(v); length += sizeof(ushort); }
        public void AddUInt(uint v) { data.Add(UInt.New(v)); length += sizeof(uint); }
        public void AddUInt2(UInt v) { data.Add(v); length += sizeof(uint); }
        public void AddInt(int v) { data.Add(Int.New(v)); length += sizeof(int); }
        public void AddInt2(Int v) { data.Add(v); length += sizeof(int); }
        public void AddBytes(byte[] v) { data.Add(v); length += (uint)v.Length; }
        public void AddChars(char[] v) { data.Add(v); length += (uint)v.Length; }
        public void AddString(string v) { data.Add(v); length += (uint)v.Length; }
        public void AddVal32(Val32 v)
        {
            data.Add(v);
            if (v.IsNeedForRelocation) relocs.Add(length);
            length += sizeof(uint);
        }

        public void AddBlock(Block block)
        {
            for (int i = 0; i < block.data.Count; i++)
            {
                var obj = block.data[i];
                if (obj is Byte) AddByte2((Byte)obj);
                else if (obj is UShort) AddUShort2((UShort)obj);
                else if (obj is UInt) AddUInt2((UInt)obj);
                else if (obj is Int) AddInt2((Int)obj);
                else if (obj is byte[]) AddBytes((byte[])obj);
                else if (obj is char[]) AddChars((char[])obj);
                else if (obj is string) AddString((string)obj);
                else if (obj is Block) AddBlock((Block)obj);
                else if (obj is Val32) AddVal32((Val32)obj);
                else throw new Exception("The method or operation is not implemented.");
            }
        }

        public void Write(BinaryWriter bw)
        {
            for (int i = 0; i < data.Count; i++)
            {
                var obj = data[i];
                if (obj is Byte) bw.Write(((Byte)obj).Value);
                else if (obj is UShort) bw.Write(((UShort)obj).Value);
                else if (obj is UInt) bw.Write(((UInt)obj).Value);
                else if (obj is Int) bw.Write(((Int)obj).Value);
                //else if (obj is long) bw.Write((long)obj);
                else if (obj is byte[]) bw.Write((byte[])obj);
                else if (obj is char[]) bw.Write((char[])obj);
                else if (obj is string) bw.Write(((string)obj).ToCharArray());
                else if (obj is Block) (obj as Block).Write(bw);
                else if (obj is Val32) bw.Write(((Val32)obj).Value);
                else throw new Exception("The method or operation is not implemented.");
            }
        }

        public byte[] ToByteArray()
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms, Encoding.ASCII);
            Write(bw);
            bw.Close();
            ms.Close();
            return ms.ToArray();
        }
    }
}
