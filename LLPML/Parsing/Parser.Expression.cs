using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.LLPML.Parsing
{
    public partial class Parser
    {
        // Expression ::= Factor (operator Factor)*
        private IIntValue Expression()
        {
            return Expression(0);
        }

        private IIntValue Expression(int order)
        {
            if (!CanRead) throw Abort("式がありません。");
            if (order >= operators.Length) return Factor();

            var ret = Expression(order + 1);
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
        private IIntValue Factor()
        {
            if (Read() == "(")
            {
                var cast = Cast();
                if (cast != null) return cast;

                var g = Group();
                if (g != null) return g;
            }
            Rewind();
            return Unary();
        }

        // Group ::= "(" Expression ")"
        private IIntValue Group()
        {
            if (!CanRead) return null;

            var ret = Expression();
            if (ret == null || !CanRead) return null;
            if (Read() != ")")
            {
                Rewind();
                return null;
            }
            return ret;
        }

        private IIntValue Unary()
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
                            if (t == "-") return new IntValue(-v.Value) { SrcInfo = si };
                            return v;
                        }
                        var n = Expression(order);
                        if (t == "-") return new Neg(parent, n) { SrcInfo = si };
                        return n;
                    }
                case "!":
                    return new Not(parent, Expression(order)) { SrcInfo = si };
                case "~":
                    return new Rev(parent, Expression(order)) { SrcInfo = si };
                case "++":
                    return new Inc(parent, Expression(order)) { SrcInfo = si };
                case "--":
                    return new Dec(parent, Expression(order)) { SrcInfo = si };
            }
            Rewind();
            return Value();
        }

        private IIntValue Value()
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
                var ret = Expression();
                if (ret is NodeBase)
                    (ret as NodeBase).Parent = parent.Root;
                return ret;
            }

            var v = parent.GetVar(t);
            if (v != null && !(v.Parent is Struct.Define))
                return new Var(parent, v) { SrcInfo = si };

            var i = parent.GetInt(t);
            if (i != null) return i;

            var s = parent.GetString(t);
            if (s != null) return s;

            // 未定義語を関数ポインタとして解釈
            if (Tokenizer.IsWord(t))
                return new Variant(parent, t) { SrcInfo = si };

            Rewind();
            throw Abort("評価できません: {0}", t);
        }

        private IntValue Integer()
        {
            var si = SrcInfo;
            var t = Read();
            if (t == null) return null;
            if (t.StartsWith("0x")) return new IntValue(t) { SrcInfo = si };

            foreach (var ch in t)
            {
                if (!char.IsDigit(ch))
                {
                    Rewind();
                    return null;
                }
            }
            return new IntValue(t) { SrcInfo = si };
        }

        private StringValue String()
        {
            var si = SrcInfo;
            var t = Read();
            if (t == null) return null;
            if (t.Length < 2 || !t.StartsWith("\"") || !t.EndsWith("\""))
            {
                Rewind();
                return null;
            }
            return new StringValue(GetString(t.Substring(1, t.Length - 2))) { SrcInfo = si };
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
            return new CharValue(s[0]) { SrcInfo = si };
        }

        private Cast Cast()
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
                    return new Cast(parent, type, Expression()) { SrcInfo = si };
                Rewind();
            }
            Rewind();
            return null;
        }
    }
}
