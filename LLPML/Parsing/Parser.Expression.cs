using System;
using System.Collections.Generic;
using System.Text;
using Girl.LLPML.Struct;

namespace Girl.LLPML.Parsing
{
    public partial class Parser
    {
        // Expression ::= Factor (operator Factor)*
        private NodeBase ReadExpression()
        {
            return ReadExpressionOrder(0);
        }

        private NodeBase ReadExpressionOrder(int order)
        {
            if (!CanRead) throw Abort("式がありません。");
            if (order >= operators.Length) return Factor();

            var ret = ReadExpressionOrder(order + 1);
            var si = SrcInfo;

            while (CanRead)
            {
                var t = Read();
                OperatorBase op;
                if (!orders[order].TryGetValue(t, out op))
                {
                    Rewind();
                    break;
                }
                ret = op.Read(ret, order);
                if (ret is NodeBase)
                    (ret as NodeBase).SrcInfo = si;
                si = SrcInfo;
            }
            return ret;
        }

        // Factor ::= Cast | Group | Unary
        private NodeBase Factor()
        {
            if (Read() == "(")
            {
                var cast = ReadCast();
                if (cast != null) return cast;

                var g = Group();
                if (g != null) return g;
            }
            Rewind();
            return Unary();
        }

        // Group ::= "(" Expression ")"
        private NodeBase Group()
        {
            if (!CanRead) return null;

            var ret = ReadExpression();
            if (ret == null || !CanRead) return null;
            if (Read() != ")")
            {
                Rewind();
                return null;
            }
            return ret;
        }

        private NodeBase Unary()
        {
            if (!CanRead) throw Abort("式がありません。");

            // 前置演算子
            var si = SrcInfo;
            var t = Read();
            int order = operators.Length - 1;
            switch (t)
            {
                case "+":
                case "-":
                    {
                        var v = Integer();
                        if (v != null)
                        {
                            if (t == "-")
                            {
                                var ret = IntValue.New(-v.Value);
                                ret.SrcInfo = si;
                                return ret;
                            }
                            else
                                return v;
                        }
                        var n = ReadExpressionOrder(order);
                        if (t == "-") return Neg.New(parent, n, si);
                        return n;
                    }
                case "!":
                    return Not.New(parent, ReadExpressionOrder(order), si);
                case "~":
                    return Rev.New(parent, ReadExpressionOrder(order), si);
                case "++":
                    return Inc.New(parent, ReadExpressionOrder(order), si);
                case "--":
                    return Dec.New(parent, ReadExpressionOrder(order), si);
            }
            Rewind();
            return Value();
        }

        private NodeBase Value()
        {
            if (!CanRead) throw Abort("式がありません。");

            var iv = Integer();
            if (iv != null) return iv;

            var sv = String();
            if (sv != null) return sv;

            var cv = Char();
            if (cv != null) return cv;

            var r = Reserved();
            if (r != null) return r;

            var si = SrcInfo;
            var t = Read();

            if (t == "::")
            {
                var ret = ReadExpression();
                if (ret is NodeBase)
                    (ret as NodeBase).Parent = parent.Root;
                return ret;
            }

            var v = parent.GetVar(t);
            if (v != null && !(v.Parent is Define))
            {
                var ret = Var.New(parent, v);
                ret.SrcInfo = si;
                return ret;
            }

            var i = parent.GetInt(t);
            if (i != null) return i;

            var s = parent.GetString(t);
            if (s != null) return s;

            // 未定義語を関数ポインタとして解釈
            if (Tokenizer.IsWord(t))
                return new Variant(parent, t) { SrcInfo = si };

            //Rewind();
            throw Abort("評価できません: {0}", t);
        }

        private IntValue Integer()
        {
            var si = SrcInfo;
            var t = Read();
            if (t == null) return null;
            IntValue ret;
            if (!t.StartsWith("0x"))
            {
                foreach (var ch in t)
                {
                    if (!char.IsDigit(ch))
                    {
                        Rewind();
                        return null;
                    }
                }
            }
            ret = IntValue.NewString(t);
            ret.SrcInfo = si;
            return ret;
        }

        private StringValue String()
        {
            var si = SrcInfo;
            var t = Read();
            if (t == null) return null;
            if (t.EndsWith("\""))
            {
                StringValue sv = null;
                if (t.Length >= 2 && t.StartsWith("\""))
                    sv = StringValue.New(GetString(t.Substring(1, t.Length - 2)));
                else if (t.Length >= 3 && t.StartsWith("@\""))
                    sv = StringValue.New(t.Substring(2, t.Length - 3));
                if (sv != null)
                {
                    sv.SrcInfo = si;
                    return sv;
                }
            }
            Rewind();
            return null;
        }

        private CharValue Char()
        {
            var si = SrcInfo;
            var t = Read();
            if (t == null) return null;
            if (t.Length < 2 || !t.StartsWith("'") || !t.EndsWith("'"))
            {
                Rewind();
                return null;
            }
            var s = GetString(t.Substring(1, t.Length - 2));
            if (s.Length != 1)
                throw Abort("1文字ではありません: {0}", t);
            return CharValue.New(s[0], si);
        }

        private Cast ReadCast()
        {
            var si = SrcInfo;
            var type = Read();
            if (Tokenizer.IsWord(type) && parent.GetVar(type) == null)
            {
                var br2 = Read();
                if (br2 == "*")
                {
                    type += br2;
                    br2 = Read();
                }
                else if (br2 == "[")
                {
                    var br3 = Read();
                    if (br3 == "]")
                    {
                        type += "[]";
                        br2 = Read();
                    }
                    else if (br3 != null)
                        Rewind();
                }
                if (br2 == ")")
                    return Cast.New(parent, type, ReadExpression(), si);
                Rewind();
            }
            Rewind();
            return null;
        }
    }
}
