﻿using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public class OpCode
    {
        public Ptr<uint> Address = 0;

        private byte[] data;
        private object op1, op2;
        private bool relative = false;

        public OpCode() { }
        public OpCode(byte[] d) { data = d; }
        public OpCode(string text) { data = Encoding.ASCII.GetBytes(text); }

        public OpCode(byte[] d, object op)
        {
            data = d;
            op1 = op;
        }

        public OpCode(byte[] d, object op, Addr32 mem)
        {
            data = d;
            op1 = op;
            op2 = mem;
        }

        public OpCode(byte[] d, object op1, byte op2)
        {
            data = d;
            this.op1 = op1;
            this.op2 = op2;
        }

        public OpCode(byte[] d, Ptr<uint> op, bool rel)
        {
            data = d;
            op1 = op;
            relative = rel;
        }

        public void Set(OpCode src)
        {
            data = src.data;
            op1 = src.op1;
            op2 = src.op2;
            relative = src.relative;
        }

        public byte[] GetCodes()
        {
            byte[] data = this.data;
            if (op2 is Addr32) data = Util.Concat(data, (op2 as Addr32).GetCodes());
            if (op1 == null) return data;

            int len = data.Length;
            if (op1 is byte)
                data = Util.GetBytes(data, (byte)op1);
            else if (op1 is ushort)
                data = Util.GetBytes(data, (ushort)op1);
            else if (op1 is Ptr<uint>)
            {
                uint val = ((Ptr<uint>)op1).Value;
                if (relative) val -= Address.Value + (uint)data.Length + 4;
                data = Util.GetBytes(data, val);
            }
            else
            {
                throw new Exception("The method or operation is not implemented.");
            }
            if (op2 is byte) data = Util.Concat(data, new byte[] { (byte)op2 });
            return data;
        }

        public void Write(Block block)
        {
            if (op1 is Ptr<uint> && relative)
            {
                block.Add(GetCodes());
            }
            else if (data != null)
            {
                block.Add(data);
                if (op2 is Addr32) (op2 as Addr32).Write(block);
                if (op1 != null)
                {
                    if (op1 is byte) block.Add((byte)op1);
                    else if (op1 is ushort) block.Add((ushort)op1);
                    else if (op1 is Ptr<uint>) block.Add((Ptr<uint>)op1);
                    else throw new Exception("The method or operation is not implemented.");
                }
                if (op2 is byte) block.Add((byte)op2);
            }
        }

        public void Test(string mnemonic, string data)
        {
            string datastr = BitConverter.ToString(GetCodes());
            if (data != datastr)
            {
                throw new Exception(string.Format(
                    "[Test 1 failed] {0}\r\n\tOK: {1}\r\n\tNG: {2}",
                    mnemonic, data, datastr));
            }
            Block block = new Block();
            Write(block);
            string datastr2 = BitConverter.ToString(block.ToByteArray());
            if (data != datastr2)
            {
                throw new Exception(string.Format(
                    "[Test 2 failed] {0}\r\n\tOK: {1}\r\n\tNG: {2}",
                    mnemonic, data, datastr2));
            }
            //Console.WriteLine("OK: {0}: {1}", datastr, mnemonic);
        }
    }
}
