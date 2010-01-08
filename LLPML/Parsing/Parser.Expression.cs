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
                        var n = Expression();
                        if (t == "-") return new Neg(parent, n) { SrcInfo = si };
                        return n;
                    }
                case "!":
                    return new Not(parent, Expression()) { SrcInfo = si };
                case "~":
                    return new Rev(parent, Expression()) { SrcInfo = si };
                case "++":
                    {
                        var target = Expression() as Var;
                        if (target == null)
                            throw parent.Abort(si, "++: 対象が変数ではありません。");
                        return new Inc(parent, target) { SrcInfo = si };
                    }
                case "--":
                    {
                        var target = Expression() as Var;
                        if (target == null)
                            throw parent.Abort(si, "--: 対象が変数ではありません。");
                        return new Dec(parent, target) { SrcInfo = si };
                    }
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

            var r = Reserved();
            if (r != null) return r;

            var si = SrcInfo;
            var t = Read();

            if (t == "::")
            {
                var p = parent;
                parent = p.Root;
                var ret = Expression();
                parent = p;
                return ret;
            }

            var v = parent.GetVar(t);
            if (v != null) return new Var(parent, v) { SrcInfo = si };

            var i = parent.GetInt(t);
            if (i != null) return new IntValue((int)i) { SrcInfo = si };

            var s = parent.GetString(t);
            if (s != null) return new StringValue(s);

            // 未定義語を関数ポインタとして解釈
            if (Tokenizer.IsWord(t))
                return new Function.Ptr(parent, t) { SrcInfo = si };

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
            return new StringValue(GetString(t.Substring(1, t.Length - 2)));
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
