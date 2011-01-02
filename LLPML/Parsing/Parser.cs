using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.LLPML.Parsing
{
    public partial class Parser
    {
        private Tokenizer tokenizer;
        private BlockBase parent;
        private bool CanRead { get { return tokenizer.CanRead; } }
        private string Read() { return tokenizer.Read(); }
        private string Peek() { return tokenizer.Peek(); }
        private void Rewind() { tokenizer.Rewind(); }

        public Parser(Tokenizer tokenizer, BlockBase parent)
        {
            this.tokenizer = tokenizer;
            this.parent = parent;
            Init();
        }

        public NodeBase[] ParseExpressions()
        {
            return Arguments(",", null, false);
        }

        public NodeBase[] Parse()
        {
            var ret = new List<NodeBase>();
            while (CanRead)
            {
#if DEBUG
                var s = Sentence();
#else
                NodeBase[] s = null;
                try
                {
                    s = Sentence();
                }
                catch (Exception ex)
                {
                    parent.Root.OnError(ex);
                }
#endif
                if (s != null) ret.AddRange(s);
            }
            return ret.ToArray();
        }

        private NodeBase[] Arguments(string sep, string end, bool mustSep)
        {
            var br = Read();
            if (br == null) return null;

            var ret = new List<NodeBase>();
            if (br == end) return ret.ToArray();
            Rewind();

            while (CanRead)
            {
                var arg = Expression();
                if (arg == null) return null;
                ret.Add(arg);
                var t = Read();
                if (mustSep)
                {
                    if (t != sep)
                    {
                        Rewind();
                        return null;
                    }
                }
                else if (t == end)
                    return ret.ToArray();
                else if (t == null)
                    return null;
                else if (sep == null)
                    Rewind();
                else if (t != sep)
                {
                    Rewind();
                    return null;
                }
            }
            return ret.ToArray();
        }

        public static string GetString(string s)
        {
            var sb = new StringBuilder();
            var esc = false;
            foreach (var ch in s)
            {
                if (esc)
                {
                    switch (ch)
                    {
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case '0':
                            sb.Append('\0');
                            break;
                        case 'a':
                            sb.Append('\a');
                            break;
                        case 'b':
                            sb.Append('\b');
                            break;
                        case 'f':
                            sb.Append('\f');
                            break;
                        default:
                            sb.Append(ch);
                            break;
                    }
                    esc = false;
                }
                else if (ch == '\\')
                    esc = true;
                else
                    sb.Append(ch);
            }
            return sb.ToString();
        }

        private Exception Abort(string format, params object[] args)
        {
            return parent.AbortInfo(SrcInfo, format, args);
        }

        private SrcInfo SrcInfo { get { return tokenizer.SrcInfo; } }
    }
}
