using System;
using System.Collections;
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
        private List<int> linePositions = new List<int>();
        private ArrayList tokens = new ArrayList();
        private ArrayList results = new ArrayList();

        private string[] reserved;
        public string[] Reserved { set { reserved = value; } }

        public static Tokenizer New(string file, string src)
        {
            var ret = new Tokenizer();
            ret.file = file;
            ret.Source = src;
            return ret;
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
                    if (ch == -1 || ch == '\r' || ch == '\n') break;
                    sb.Append((char)ch);
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
                    if (ch == -1) break;

                    sb.Append((char)ch);
                    if (aster && ch == '/') break;
                    aster = ch == '*';
                }
                ret.Comment = sb.ToString();
                return Read();
            }

            tokens.Add(ret);
            return ret.String;
        }

        private void SkipSpaces()
        {
            for (; ; )
            {
                var ch = ReadChar();
                if (ch == -1) break;
                if (ch > ' ')
                {
                    RewindChar();
                    break;
                }
            }
        }

        public void Rewind()
        {
            if (tokens.Count > 0)
            {
                int last = tokens.Count - 1;
                results.Add(tokens[last]);
                tokens.RemoveAt(last);
            }
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
                if (results.Count > 0)
                    return (results[results.Count - 1] as Data).String;
                if (tokens.Count > 0)
                    return (tokens[tokens.Count - 1] as Data).String;
                return null;
            }
        }

        public SrcInfo SrcInfo
        {
            get
            {
                if (results.Count > 0)
                {
                    var d = results[results.Count - 1] as Data;
                    return d.SrcInfo;
                }
                SkipSpaces();
                return SrcInfo.New(file, lineNumber, linePosition);
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
            for (int i = 0; i < s.Length; i++)
            {
                var ch = s[i];
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

        private int ReadChar()
        {
            if (!CanReadChar) return -1;
            char ret = Source[pos++];
            if (ret == '\n')
            {
                linePositions.Add(linePosition);
                linePosition = 1;
                lineNumber++;
            }
            else
                linePosition++;
            return ret;
        }

        public int PeekChar()
        {
            if (!CanReadChar) return -1;
            return Source[pos];
        }

        private void RewindChar()
        {
            if (pos <= 0) return;

            pos--;
            linePosition--;
            if (linePosition < 1)
            {
                var last = linePositions.Count - 1;
                linePosition = linePositions[last];
                linePositions.RemoveAt(last);
                lineNumber--;
            }
        }

        private Data ReadInternal()
        {
            if (results.Count > 0)
            {
                int last = results.Count - 1;
                var result = results[last] as Data;
                results.RemoveAt(last);
                return result;
            }

            SkipSpaces();
            var ret = new Data(pos, SrcInfo);
            StringBuilder sb = new StringBuilder();
            int chi, str = 0;
            bool isWord = true, useEscape = true;
            while ((chi = ReadChar()) != -1)
            {
                var ch = (char)chi;
                if (str != 0 && ch == '\\' && useEscape)
                {
                    sb.Append(ch);
                    chi = ReadChar();
                    if (chi == -1) break;
                    sb.Append((char)chi);
                }
                else if (ch == '@' && sb.Length == 0)
                {
                    sb.Append(ch);
                    chi = ReadChar();
                    if (chi == -1)
                        break;
                    else if (chi == '"')
                    {
                        str = chi;
                        useEscape = false;
                        sb.Append((char)chi);
                    }
                    else
                        RewindChar();
                }
                else if (ch == '\'' || ch == '"')
                {
                    if (sb.Length == 0)
                        str = ch;
                    else if (str == 0)
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
                else if (str != 0)
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
                        var b = IsReserved(sb.ToString() + ch);
                        if (b == 0)
                            RewindChar();
                        else
                            sb.Append(ch);
                        if (b != 2) break;
                    }
                    else
                        sb.Append(ch);
                }
            }
            if (sb.Length == 0) return null;
            ret.String = sb.ToString();
            return ret;
        }

        private int IsReserved(string s)
        {
            for (int i = 0; i < reserved.Length; i++)
            {
                var r = reserved[i];
                if (r == s) return 1;
                if (r.StartsWith(s)) return 2;
            }
            return 0;
        }

        public Exception Abort(string msg)
        {
            return new Exception(string.Format(
                "{0}: [{1}:{2}] {3}", file, lineNumber, linePosition, msg));
        }
    }
}
