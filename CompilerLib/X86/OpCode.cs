using System;
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

        public static OpCode NewB(byte[] d, byte op)
        {
            var ret = new OpCode();
            ret.data = d;
            ret.op1 = Girl.Binary.Byte.New(op);
            return ret;
        }

        public static OpCode NewW(byte[] d, ushort op)
        {
            var ret = new OpCode();
            ret.data = d;
            ret.op1 = UShort.New(op);
            return ret;
        }

        public static OpCode NewD(byte[] d, Val32 op)
        {
            var ret = new OpCode();
            ret.data = d;
            ret.op1 = op;
            return ret;
        }

        public static OpCode NewA(byte[] d, Addr32 mem)
        {
            var ret = new OpCode();
            ret.data = d;
            ret.op2 = mem;
            return ret;
        }

        public static OpCode NewBA(byte[] d, byte op, Addr32 mem)
        {
            var ret = new OpCode();
            ret.data = d;
            ret.op1 = Girl.Binary.Byte.New(op);
            ret.op2 = mem;
            return ret;
        }

        public static OpCode NewWA(byte[] d, ushort op, Addr32 mem)
        {
            var ret = new OpCode();
            ret.data = d;
            ret.op1 = UShort.New(op);
            ret.op2 = mem;
            return ret;
        }

        public static OpCode NewDA(byte[] d, Val32 op, Addr32 mem)
        {
            var ret = new OpCode();
            ret.data = d;
            ret.op1 = op;
            ret.op2 = mem;
            return ret;
        }

        public static OpCode NewWB(byte[] d, ushort op1, byte op2)
        {
            var ret = new OpCode();
            ret.data = d;
            ret.op1 = UShort.New(op1);
            ret.op2 = Girl.Binary.Byte.New(op2);
            return ret;
        }

        public static OpCode NewDRel(byte[] d, Val32 op, bool rel)
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
            if (this.op2 is Addr32) data = Util.Concat(data, (op2 as Addr32).GetCodes());
            if (op1 == null) return data;

            if (this.op1 is Girl.Binary.Byte)
                data = Util.AddByteToBytes(data, (op1 as Girl.Binary.Byte).Value);
            else if (this.op1 is UShort)
                data = Util.AddUShortToBytes(data, (op1 as UShort).Value);
            else if (this.op1 is Val32)
            {
                uint val = (op1 as Val32).Value;
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
            if (this.op2 is Girl.Binary.Byte) data = Util.Concat(data, Util.GetBytes1((op2 as Girl.Binary.Byte).Value));
            return data;
        }

        public void Write(Block32 block)
        {
            if (this.op1 is Val32 && relative)
            {
                block.AddBytes(GetCodes());
            }
            else if (data != null)
            {
                block.AddBytes(data);
                if (this.op2 is Addr32) (op2 as Addr32).Write(block);
                if (op1 != null)
                {
                    if (this.op1 is Girl.Binary.Byte) block.AddByte2(op1 as Girl.Binary.Byte);
                    else if (this.op1 is UShort) block.AddUShort2(op1 as UShort);
                    else if (this.op1 is Val32) block.AddVal32(op1 as Val32);
                    else throw new Exception("The method or operation is not implemented.");
                }
                if (this.op2 is Girl.Binary.Byte) block.AddByte2(op2 as Girl.Binary.Byte);
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
            Block32 block = new Block32();
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
