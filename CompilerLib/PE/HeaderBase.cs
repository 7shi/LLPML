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

        public void WriteBlock(Block32 block)
        {
            block.AddUInt(Address);
            block.AddUInt(Size);
        }
    }

    public abstract class HeaderBase : WriterBase
    {
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
