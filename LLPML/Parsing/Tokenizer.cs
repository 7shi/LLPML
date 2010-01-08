using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.LLPML.Parsing
{
    public class Tokenizer
    {
        private string source;
        public string Source { get { return source; } }

        private int pos = 0;
        private Stack<string> tokens = new Stack<string>();
        private Stack<string> results = new Stack<string>();

        private string[] reserved;
        public string[] Reserved { set { reserved = value; } }

        public Tokenizer(string src)
        {
            source = src.Trim();
        }

        public bool CanRead
        {
            get
            {
                return CanReadChar || results.Count > 0;
            }
        }

        public string Read()
        {
            string ret = ReadInternal();
            if (ret != null) tokens.Push(ret);
            return ret;
        }

        public void Rewind()
        {
            if (tokens.Count > 0) results.Push(tokens.Pop());
        }

        public string Latest
        {
            get
            {
                if (results.Count > 0) return results.Peek();
                if (tokens.Count > 0) return tokens.Peek();
                return null;
            }
        }

        private bool CanReadChar
        {
            get
            {
                return source != null && pos < source.Length;
            }
        }

        private char? ReadChar()
        {
            if (!CanReadChar) return null;
            return source[pos++];
        }

        private char? PeekChar()
        {
            if (!CanReadChar) return null;
            return source[pos];
        }

        private void RewindChar()
        {
            if (pos > 0) pos--;
        }

        private string ReadInternal()
        {
            if (results.Count > 0) return results.Pop();

            StringBuilder sb = new StringBuilder();
            char? ch, pre = null, str = null;
            bool isWord = true;
            while ((ch = ReadChar()) != null)
            {
                if (ch == '\'' || ch == '"')
                {
                    if (sb.Length == 0)
                    {
                        str = ch;
                    }
                    else if (str == null)
                    {
                        RewindChar();
                        break;
                    }
                    else if (pre != '\\' && str == ch)
                    {
                        sb.Append(ch);
                        break;
                    }
                    sb.Append(ch);
                }
                else if (str != null)
                {
                    sb.Append(ch);
                }
                else if (ch <= ' ')
                {
                    if (sb.Length > 0) break;
                }
                else if (ch == '_' || char.IsLetterOrDigit((char)ch))
                {
                    if (!isWord)
                    {
                        RewindChar();
                        break;
                    }
                    sb.Append(ch);
                }
                else
                {
                    isWord = false;
                    if (sb.Length > 0)
                    {
                        bool? b = IsReserved(sb.ToString() + ch);
                        if (b == false)
                            RewindChar();
                        else
                            sb.Append(ch);
                        if (b != null) break;
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                }
                pre = ch;
            }
            if (sb.Length == 0) return null;
            return sb.ToString();
        }

        private bool? IsReserved(string s)
        {
            foreach (string r in reserved)
            {
                if (r == s) return true;
                if (r.StartsWith(s)) return null;
            }
            return false;
        }
    }
}
