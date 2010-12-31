﻿using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public class OpCode
    {
        public Val32 Address = Val32.New(0);
        private byte[] data;
        private object op1;
        private object op2;
        private bool relative;
        public bool ByteRelative { get; set; }

        public static OpCode NewBytes(byte[] d)
        {
            var ret = new OpCode();
            ret.data = d;
            return ret;
        }

        public static OpCode NewString(string text)
        {
            var ret = new OpCode();
            ret.data = Encoding.ASCII.GetBytes(text);
            return ret;
        }

        public static OpCode New(byte[] d, object op)
        {
            var ret = new OpCode();
            ret.data = d;
            ret.op1 = op;
            return ret;
        }

        public static OpCode NewA(byte[] d, object op, Addr32 mem)
        {
            var ret = new OpCode();
            ret.data = d;
            ret.op1 = op;
            ret.op2 = mem;
            return ret;
        }

        public static OpCode NewB(byte[] d, object op1, byte op2)
        {
            var ret = new OpCode();
            ret.data = d;
            ret.op1 = op1;
            ret.op2 = op2;
            return ret;
        }

        public static OpCode NewV(byte[] d, Val32 op, bool rel)
        {
            var ret = new OpCode();
            ret.data = d;
            ret.op1 = op;
            ret.relative = rel;
            return ret;
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

            if (op1 is byte)
                data = Util.AddByteToBytes(data, (byte)op1);
            else if (op1 is ushort)
                data = Util.AddUShortToBytes(data, (ushort)op1);
            else if (op1 is Val32)
            {
                uint val = ((Val32)op1).Value;
                if (ByteRelative)
                {
                    val -= Address.Value + (uint)data.Length + 1;
                    data = Util.AddByteToBytes(data, (byte)val);
                }
                else
                {
                    if (relative) val -= Address.Value + (uint)data.Length + 4;
                    data = Util.AddUIntToBytes(data, val);
                }
            }
            else
            {
                throw new Exception("The method or operation is not implemented.");
            }
            if (op2 is byte) data = Util.Concat(data, Util.GetBytes1((byte)op2));
            return data;
        }

        public void Write(Block block)
        {
            if (op1 is Val32 && relative)
            {
                block.AddBytes(GetCodes());
            }
            else if (data != null)
            {
                block.AddBytes(data);
                if (op2 is Addr32) (op2 as Addr32).Write(block);
                if (op1 != null)
                {
                    if (op1 is byte) block.AddByte((byte)op1);
                    else if (op1 is ushort) block.AddUShort((ushort)op1);
                    else if (op1 is Val32) block.AddVal32((Val32)op1);
                    else if (op1 is Val32) block.AddVal32((Val32)op1);
                    else throw new Exception("The method or operation is not implemented.");
                }
                if (op2 is byte) block.AddByte((byte)op2);
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
