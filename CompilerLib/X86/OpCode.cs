using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class OpCode
    {
        public Ref<uint> Address = 0;

        private byte[] data;
        private object operand;
        private Addr32 memory;
        private bool relative = false;

        public OpCode() { }
        public OpCode(byte[] d) { data = d; }
        public OpCode(string text) { data = Encoding.ASCII.GetBytes(text); }

        public OpCode(byte[] d, object op)
        {
            data = d;
            operand = op;
        }

        public OpCode(byte[] d, object op, Addr32 mem)
        {
            data = d;
            operand = op;
            memory = mem;
        }

        public OpCode(byte[] d, Ref<uint> op, bool rel)
        {
            data = d;
            operand = op;
            relative = rel;
        }

        public void Set(OpCode src)
        {
            data = src.data;
            operand = src.operand;
            memory = src.memory;
            relative = src.relative;
        }

        public byte[] GetCodes()
        {
            byte[] data = this.data;
            if (memory != null) data = Util.Concat(data, memory.GetCodes());
            if (operand == null) return data;

            int len = data.Length;
            if (operand is byte)
                return Util.GetBytes(data, (byte)operand);
            else if (operand is ushort)
                return Util.GetBytes(data, (ushort)operand);
            else if (operand is Ref<uint>)
            {
                uint val = ((Ref<uint>)operand).Value;
                if (relative) val -= Address.Value + (uint)data.Length + 4;
                return Util.GetBytes(data, val);
            }
            throw new Exception("The method or operation is not implemented.");
        }

        public void Write(Block block)
        {
            if (operand is Ref<uint> && relative)
            {
                block.Add(GetCodes());
            }
            else if (data != null)
            {
                block.Add(data);
                if (memory != null) memory.Write(block);
                if (operand != null)
                {
                    if (operand is byte) block.Add((byte)operand);
                    else if (operand is ushort) block.Add((ushort)operand);
                    else if (operand is Ref<uint>) block.Add((Ref<uint>)operand);
                    else throw new Exception("The method or operation is not implemented.");
                }
            }
        }
    }
}
