using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Girl.LLPML.Parsing
{
    public class Tokenizer
    {
        private class Data
        {
            public int Pos;
            public string String, Comment;
            public SrcInfo SrcInfo;

            public Data(int pos, SrcInfo si)
            {
                Pos = pos;
                SrcInfo = si;
            }
        }

        private string file;
        public string Source { get; private set; }

        private int pos = 0, lineNumber = 1, linePosition = 1;
        private Stack<int> linePositions = new Stack<int>();
        private Stack<Data> tokens = new Stack<Data>();
        private Stack<Data> results = new Stack<Data>();

        private string[] reserved;
        public string[] Reserved { set { reserved = value; } }

        public Tokenizer(string file, string src)
        {
            this.file = file;
            Source = src;
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
            Data ret = ReadInternal();
            if (ret == null) return null;

            if (ret.String == "//")
            {
                var sb = new StringBuilder(ret.String);
                for (; ; )
                {
                    var ch = ReadChar();
                    if (ch == null || ch == '\r' || ch == '\n') break;
                    sb.Append(ch);
                }
                ret.Comment = sb.ToString();
                return Read();
            }
            else if (ret.String == "/*")
            {
                var sb = new StringBuilder(ret.String);
                bool aster = false;
                for (; ; )
                {
                    var ch = ReadChar();
                    if (ch == null) break;

                    sb.Append(ch);
                    if (aster && ch == '/') break;
                    aster = ch == '*';
                }
                ret.Comment = sb.ToString();
                return Read();
            }

            tokens.Push(ret);
            return ret.String;
        }

        private void SkipSpaces()
        {
            for (; ; )
            {
                var ch = ReadChar();
                if (ch == null) break;
                if (ch > ' ')
                {
                    RewindChar();
                    break;
                }
            }
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

        public SrcInfo SrcInfo
        {
            get
            {
                if (results.Count > 0)
                {
                    var d = results.Peek();
                    return d.SrcInfo;
                }
                SkipSpaces();
                return new SrcInfo(file, lineNumber, linePosition);
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
            char ret = Source[pos++];
            if (ret == '\n')
            {
                linePositions.Push(linePosition);
                linePosition = 1;
                lineNumber++;
            }
            else
                linePosition++;
            return ret;
        }

        public char? PeekChar()
        {
            if (!CanReadChar) return null;
            return Source[pos];
        }

        private void RewindChar()
        {
            if (pos <= 0) return;

            pos--;
            linePosition--;
            if (linePosition < 1)
            {
                linePosition = linePositions.Pop();
                lineNumber--;
            }
        }

        private Data ReadInternal()
        {
            if (results.Count > 0) return results.Pop();

            SkipSpaces();
            var ret = new Data(pos, SrcInfo);
            StringBuilder sb = new StringBuilder();
            char? ch, str = null;
            bool isWord = true, useEscape = true;
            while ((ch = ReadChar()) != null)
            {
                if (str != null && ch == '\\' && useEscape)
                {
                    sb.Append(ch);
                    ch = ReadChar();
                    if (ch == null) break;
                    sb.Append(ch);
                }
                else if (ch == '@' && sb.Length == 0)
                {
                    sb.Append(ch);
                    ch = ReadChar();
                    if (ch == null)
                        break;
                    else if (ch == '"')
                    {
                        str = ch;
                        useEscape = false;
                        sb.Append(ch);
                    }
                    else
                        RewindChar();
                }
                else if (ch == '\'' || ch == '"')
                {
                    if (sb.Length == 0)
                        str = ch;
                    else if (str == null)
                    {
                        RewindChar();
                        break;
                    }
                    else if (str == ch)
                    {
                        sb.Append(ch);
                        break;
                    }
                    sb.Append(ch);
                }
                else if (str != null)
                    sb.Append(ch);
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
                        sb.Append(ch);
                }
            }
            if (sb.Length == 0) return null;
            ret.String = sb.ToString();
            return ret;
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
                "{0}: [{1}:{2}] {3}", file, lineNumber, linePosition, msg));
        }
    }
}
