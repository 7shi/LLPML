using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Girl.LLPML.Parsing
{
    public class Tokenizer
    {
        private class IntString
        {
            public int Int;
            public string String;

            public IntString(int i, string s)
            {
                Int = i;
                String = s;
            }
        }

        private string file;
        public string Source { get; private set; }

        private int pos = 0, lineNumber = 0, linePosition = 0;
        private Stack<IntString> tokens = new Stack<IntString>();
        private Stack<IntString> results = new Stack<IntString>();

        private string[] reserved;
        public string[] Reserved { set { reserved = value; } }

        public Tokenizer(string file, string src)
        {
            this.file = file;
            Source = src.TrimEnd();
        }

        public Tokenizer(string file, string src, int lineNumber, int linePosition)
            : this(file, src)
        {
            this.lineNumber = lineNumber;
            this.linePosition = linePosition;
        }

        public Tokenizer(string file, XmlTextReader xr)
            : this(file, xr.Value, xr.LineNumber, xr.LinePosition)
        {
        }

        public bool CanRead { get { return Peek() != null; } }

        public string Read()
        {
            IntString ret = ReadInternal();
            if (ret == null) return null;

            if (ret.String == "//")
            {
                //var sb = new StringBuilder(ret.String);
                for (; ; )
                {
                    var ch = ReadChar();
                    if (ch == null || ch == '\r' || ch == '\n') break;
                    //sb.Append(ch);
                }
                //ret.String = sb.ToString();
                return Read();
            }
            else if (ret.String == "/*")
            {
                //var sb = new StringBuilder(ret.String);
                bool aster = false;
                for (; ; )
                {
                    var ch = ReadChar();
                    if (ch == null) break;

                    //sb.Append(ch);
                    if (aster && ch == '/') break;
                    aster = ch == '*';
                }
                //ret.String = sb.ToString();
                return Read();
            }

            tokens.Push(ret);
            return ret.String;
        }

        public void Rewind()
        {
            if (tokens.Count > 0) results.Push(tokens.Pop());
        }

        public string Peek()
        {
            var ret = Read();
            if (ret != null) Rewind();
            return ret;
        }

        public string Latest
        {
            get
            {
                if (results.Count > 0) return results.Peek().String;
                if (tokens.Count > 0) return tokens.Peek().String;
                return null;
            }
        }

        public int Position
        {
            get
            {
                int ret = pos;
                if (results.Count > 0) ret = results.Peek().Int;
                for (; ret < Source.Length; ret++)
                {
                    if (Source[ret] > ' ') break;
                }
                return ret;
            }
        }

        public int LineNumber
        {
            get
            {
                int ret = lineNumber;
                foreach (char ch in Source.Substring(0, Position))
                {
                    if (ch == '\n') ret++;
                }
                return ret;
            }
        }

        public int LinePosition
        {
            get
            {
                var pos = Position;
                var p = Source.LastIndexOf('\n', Math.Min(pos, Source.Length - 1));
                if (p < 0) return linePosition + pos;
                return pos - p;
            }
        }

        public static bool IsWordChar(char ch)
        {
            return ch > 128 || ch == '_' || char.IsLetterOrDigit(ch);
        }

        public static bool IsWord(string s)
        {
            return CheckString(s, char.IsDigit, IsWordChar);
        }

        public static bool IsDigit(string s)
        {
            return CheckString(s, null, char.IsDigit);
        }

        public static bool CheckString(string s, Func<char, bool> func1, Func<char, bool> func2)
        {
            if (string.IsNullOrEmpty(s)) return false;
            bool first = true;
            foreach (var ch in s)
            {
                if (first)
                {
                    if (func1 != null && func1(ch)) return false;
                    first = false;
                }
                if (!func2(ch)) return false;
            }
            return true;
        }

        private bool CanReadChar
        {
            get
            {
                return Source != null && pos < Source.Length;
            }
        }

        private char? ReadChar()
        {
            if (!CanReadChar) return null;
            return Source[pos++];
        }

        private char? PeekChar()
        {
            if (!CanReadChar) return null;
            return Source[pos];
        }

        private void RewindChar()
        {
            if (pos > 0) pos--;
        }

        private IntString ReadInternal()
        {
            if (results.Count > 0) return results.Pop();

            int pos = this.pos;
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
                else if (IsWordChar((char)ch))
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
            return new IntString(pos, sb.ToString());
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

        public Exception Abort(string msg)
        {
            return new Exception(string.Format(
                "{0}: [{1}:{2}] {3}", file, LineNumber, LinePosition, msg));
        }
    }
}
