using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.Binary
{
    public class Util
    {
        public static byte[] AddByteToBytes(byte[] b, byte v)
        {
            int len = b.Length;
            byte[] ret = new byte[len + 1];
            Array.Copy(b, ret, len);
            ret[len] = v;
            return ret;
        }

        public static byte[] AddUShortToBytes(byte[] b, ushort v)
        {
            int len = b.Length;
            byte[] ret = new byte[len + sizeof(ushort)];
            Array.Copy(b, ret, len);
            SetUShort(ret, len, v);
            return ret;
        }

        public static byte[] AddUIntToBytes(byte[] b, uint v)
        {
            int len = b.Length;
            byte[] ret = new byte[len + sizeof(uint)];
            Array.Copy(b, ret, len);
            SetUInt(ret, len, v);
            return ret;
        }

        public static byte[] AddBytesToByte(byte b1, byte[] b2)
        {
            byte[] ret = new byte[1 + b2.Length];
            ret[0] = b1;
            Array.Copy(b2, 0, ret, 1, b2.Length);
            return ret;
        }

        public static byte[] Concat(byte[] b1, byte[] b2)
        {
            byte[] ret = new byte[b1.Length + b2.Length];
            Array.Copy(b1, ret, b1.Length);
            Array.Copy(b2, 0, ret, b1.Length, b2.Length);
            return ret;
        }

        public static void SetUShort(byte[] b, int pos, ushort v)
        {
            b[pos] = (byte)v;
            b[pos + 1] = (byte)(v >> 8);
        }

        public static void SetUInt(byte[] b, int pos, uint v)
        {
            b[pos] = (byte)v;
            b[pos + 1] = (byte)(v >> 8);
            b[pos + 2] = (byte)(v >> 16);
            b[pos + 3] = (byte)(v >> 24);
        }

        public static byte[] GetBytes1(byte b1)
        {
            return new[] { b1 };
        }

        public static byte[] GetBytes2(byte b1, byte b2)
        {
            return new[] { b1, b2 };
        }

        public static byte[] GetBytes3(byte b1, byte b2, byte b3)
        {
            return new[] { b1, b2, b3 };
        }

        public static byte[] GetBytes4(byte b1, byte b2, byte b3, byte b4)
        {
            return new[] { b1, b2, b3, b4 };
        }
    }
}
