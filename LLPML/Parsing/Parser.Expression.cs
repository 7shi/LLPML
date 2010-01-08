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
            if (order >= operators.Length) return Member();

            var ret = Expression(order + 1);

            while (CanRead)
            {
                var t = Read();
                Operator op;
                if (!orders[order].TryGetValue(t, out op))
                {
                    Rewind();
                    break;
                }
                ret = op.Handler(ret, Expression(order + 1));
            }
            return ret;
        }

        private IIntValue Member()
        {
            var f = Factor();
            var mem = Member(f);
            if (mem != null) return mem;
            return f;
        }

        private IIntValue Member(IIntValue f)
        {
            Struct.Member mem = f as Struct.Member;
            while (CanRead)
            {
                var t1 = Read();
                if (t1 != ".")
                {
                    Rewind();
                    break;
                }
                var t2 = Read();
                if (!Tokenizer.IsWord(t2))
                    throw Abort("名前が不適切です: {0}", t2);
                if (Read() == "(")
                {
                    var args = Arguments(",", ")", false);
                    if (args == null)
                        throw Abort("引数が不完全です。");
                    var inv = new Struct.Invoke(parent, t2, args);
                    if (mem != null) return new Struct.Invoke(inv, mem);
                    return new Struct.Invoke(inv, f);
                }
                Rewind();
                var t2m = new Struct.Member(parent, t2);
                if (mem == null)
                {
                    if (f is Var)
                        t2m.Var = f as Var;
                    else if (f is Pointer && (f as Pointer).Reference is Struct.Declare)
                        t2m.Ptr = (f as Pointer).Reference as Struct.Declare;
                    else
                        throw Abort("構造体ではありません。");
                    mem = t2m;
                    mem.SetLine(tokenizer.LineNumber, tokenizer.LinePosition);
                }
                else
                    mem.Append(t2m);
            }
            if (mem != null) return Primary(mem);
            return null;
        }

        // Factor ::= Cast | Group | Unary
        private IIntValue Factor()
        {
            if (Peek() == "(")
            {
                var cast = Cast();
                if (cast != null) return cast;
                var g = Group();
                if (g != null) return g;
            }
            return Unary();
        }

        // Group ::= "(" Expression ")"
        private IIntValue Group()
        {
            if (!CanRead) return null;

            if (Read() != "(")
            {
                Rewind();
                return null;
            }
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

            var t = Read();
            switch (t)
            {
                case "+":
                case "-":
                    {
                        var v = Integer();
                        if (v != null)
                        {
                            if (t == "-") return new IntValue(-v.Value);
                            return v;
                        }
                        var n = Expression();
                        if (t == "-") return new Neg(parent, n);
                        return n;
                    }
                case "!":
                    return new Not(parent, Expression());
                case "~":
                    return new Rev(parent, Expression());
                case "++":
                    {
                        var target = Primary() as Var;
                        if (target == null)
                            throw Abort("++: 対象が変数ではありません。");
                        return new Inc(parent, target);
                    }
                case "--":
                    {
                        var target = Primary() as Var;
                        if (target == null)
                            throw Abort("--: 対象が変数ではありません。");
                        return new Dec(parent, target);
                    }
            }
            Rewind();
            return Primary();
        }

        private IIntValue Primary()
        {
            return Primary(Value());
        }

        private IIntValue Primary(IIntValue v)
        {
            if (!CanRead) return v;

            switch (Read())
            {
                case "++":
                    {
                        var target = v as Var;
                        if (target == null)
                            throw Abort("++: 対象が変数ではありません。");
                        return new PostInc(parent, target);
                    }
                case "--":
                    {
                        var target = v as Var;
                        if (target == null)
                            throw Abort("--: 対象が変数ではありません。");
                        return new PostDec(parent, target);
                    }
            }
            Rewind();
            return v;
        }

        private IIntValue Value()
        {
            if (!CanRead) throw Abort("式がありません。");

            var iv = Integer();
            if (iv != null) return iv;

            var sv = String();
            if (sv != null) return sv;

            var t = Read();
            switch (t)
            {
                case "null":
                    return new Null(parent);
                case "__FUNCTION__":
                    return new StringValue(parent.GetName());
                case "__FILE__":
                    return new StringValue(parent.Root.Source);
                case "sizeof":
                    {
                        Check("sizeof", "(");
                        var arg = Read();
                        if (arg == null)
                            throw Abort("sizeof: 引数が必要です。");
                        Check("sizeof", ")");
                        return new Struct.Size(parent, arg);
                    }
                case "addrof":
                    return AddrOf();
            }

            var vd = parent.GetVar(t);
            var v = vd != null ? new Var(parent, vd) : null;

            var pd = parent.GetPointer(t);
            var p = pd != null ? new Pointer(parent, pd) : null;

            switch (Read())
            {
                case null:
                    break;
                case "(":
                    {
                        var args = Arguments(",", ")", false);
                        if (args == null)
                            throw Abort("{0}: 引数が不完全です。", t);
                        if (v != null)
                            return new Call(parent, v, args);
                        return new Call(parent, t, args);
                    }
                default:
                    Rewind();
                    break;
            }

            if (v != null) return v;
            if (p != null) return p;

            var i = parent.GetInt(t);
            if (i != null) return new IntValue((int)i);

            var s = parent.GetString(t);
            if (s != null) return new StringValue(s);

            Rewind();
            throw Abort("評価できません: {0}", t);
        }

        private IntValue Integer()
        {
            var t = Read();
            if (t == null) return null;
            if (t.StartsWith("0x")) return new IntValue(t);

            foreach (var ch in t)
            {
                if (!char.IsDigit(ch))
                {
                    Rewind();
                    return null;
                }
            }
            return new IntValue(t);
        }

        private StringValue String()
        {
            var t = Read();
            if (t == null) return null;
            if (t.Length < 2 || !t.StartsWith("\"") || !t.EndsWith("\""))
            {
                Rewind();
                return null;
            }
            return new StringValue(GetString(t.Substring(1, t.Length - 2)));
        }

        private IIntValue AddrOf()
        {
            Check("addrof", "(");

            IIntValue ret = null;
            var ln = tokenizer.LineNumber;
            var lp = tokenizer.LinePosition;
            var t = Read();
            if (!Tokenizer.IsWord(t))
            {
                if (t != null) Rewind();
                throw Abort("addrof: 引数が不適切です: {0}", t);
            }
            var p = parent.GetPointer(t);
            if (p != null)
            {
                ret = new Pointer(parent, p);
                if (Peek() == ".")
                {
                    ret = Member(ret) as Struct.Member;
                    if (ret == null)
                        throw parent.Abort(ln, lp, "addrof: 引数が不適切です。");
                }
            }
            else
                ret = new Function.Ptr(parent, t);

            Check("addrof", ")");
            return ret;
        }

        private Struct.Cast Cast()
        {
            var br1 = Read();
            if (br1 == "(")
            {
                var type = Read();
                if (Tokenizer.IsWord(type) && parent.GetPointer(type) == null)
                {
                    var br2 = Read();
                    if (br2 == ")")
                        return new Struct.Cast(parent, type, Expression());
                    Rewind();
                }
                Rewind();
            }
            Rewind();
            return null;
        }
    }
}
