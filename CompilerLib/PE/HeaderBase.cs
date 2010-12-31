using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Girl.Binary;

namespace Girl.PE
{
    public struct Table
    {
        public uint Address, Size;
    }

    public abstract class HeaderBase : WriterBase
    {
        public FieldInfo[] GetFields()
        {
            return this.GetType().GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        public override void WriteBlock(Block block)
        {
            foreach (FieldInfo fi in GetFields())
            {
                Object obj = fi.GetValue(this);
                if (obj is Table)
                {
                    block.AddUInt(((Table)obj).Address);
                    block.AddUInt(((Table)obj).Size);
                }
                else if (obj is byte) block.AddByte((byte)obj);
                else if (obj is ushort) block.AddUShort((ushort)obj);
                else if (obj is uint) block.AddUInt((uint)obj);
                //else if (obj is long) block.Add((long)obj);
                else if (obj is char[]) block.AddChars((char[])obj);
                else if (obj is string) block.AddString((string)obj);
                else if (obj is Val32) block.AddVal32((Val32)obj);
                else throw new Exception("The method or operation is not implemented.");
            }
        }

        public static string Trim(string s)
        {
            int p = s.IndexOf('\0');
            if (p < 0) return s;
            return s.Substring(0, p);
        }

        public static string Pad(int len, string s)
        {
            if (s.Length > len)
            {
                return s.Substring(0, len);
            }
            else if (s.Length < len)
            {
                return s + new string('\0', len - s.Length);
            }
            return s;
        }

        public static int GetPaddedSize(int pad, string s)
        {
            return (s.Length / pad + 1) * pad;
        }
    }
}
