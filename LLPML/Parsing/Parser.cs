﻿using System;
using System.Collections;
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

        public static Parser Create(Tokenizer tokenizer, BlockBase parent)
        {
            var ret = new Parser();
            ret.tokenizer = tokenizer;
            ret.parent = parent;
            ret.InitOperator();
            return ret;
        }

        public NodeBase[] ParseExpressions()
        {
            return Arguments(",", null, false);
        }

        public NodeBase[] Parse()
        {
            var list = new ArrayList();
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
                if (s != null)
                {
                    for (int i = 0; i < s.Length; i++)
                        list.Add(s[i]);
                }
            }
            var ret = new NodeBase[list.Count];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = list[i] as NodeBase;
            return ret;
        }

        private NodeBase[] Arguments(string sep, string end, bool mustSep)
        {
            var br = Read();
            if (br == null) return null;
            if (br == end) return new NodeBase[0];

            var list = new ArrayList();
            Rewind();

            while (CanRead)
            {
                var arg = ReadExpression();
                if (arg == null) return null;
                list.Add(arg);
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
                    break;
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
            var ret = new NodeBase[list.Count];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = list[i] as NodeBase;
            return ret;
        }

        public static string GetString(string s)
        {
            var sb = new StringBuilder();
            var esc = false;
            for (int i = 0; i < s.Length; i++)
            {
                var ch = s[i];
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
