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
            if (!CanRead) throw Abort("��������܂���B");
            if (order >= operators.Length) return Index();

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
                ret = op.Handler(ret, Expression(order + op.Associativity));
            }
            return ret;
        }

        private IIntValue Index()
        {
            IIntValue ret = null;
            for (; ; )
            {
                var si = SrcInfo;
                if (ret == null)
                    ret = Member();
                else
                {
                    var m = Member(ret);
                    if (m == null) return ret;
                    ret = m;
                }
                var br = Read();
                if (br != "[")
                {
                    if (br != null) Rewind();
                    break;
                }
                var ar = ret as VarBase;
                if (ar == null)
                    throw parent.Abort(si, "�z�񂪕K�v�ł��B");
                ret = new Index(parent, ar, Expression()) { SrcInfo = si };
                Check("�z��", "]");
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
            var si = SrcInfo;
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
                    throw Abort("���O���s�K�؂ł�: {0}", t2);
                if (Read() == "(")
                {
                    var args = Arguments(",", ")", false);
                    if (args == null)
                        throw Abort("�������s���S�ł��B");
                    if (mem != null) return new Call(parent, t2, mem, args) { SrcInfo = si };
                    return new Call(parent, t2, f, args) { SrcInfo = si };
                }
                Rewind();
                var t2m = new Struct.Member(parent, t2);
                t2m.SrcInfo = SrcInfo;
                if (mem == null)
                {
                    if (f is VarBase)
                        t2m.Target = f as VarBase;
                    else
                        throw Abort("�\���̂ł͂���܂���B");
                    mem = t2m;
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
            if (!CanRead) throw Abort("��������܂���B");

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
                        var target = Member() as Var;
                        if (target == null)
                            throw parent.Abort(si, "++: �Ώۂ��ϐ��ł͂���܂���B");
                        return new Inc(parent, target) { SrcInfo = si };
                    }
                case "--":
                    {
                        var target = Member() as Var;
                        if (target == null)
                            throw parent.Abort(si, "--: �Ώۂ��ϐ��ł͂���܂���B");
                        return new Dec(parent, target) { SrcInfo = si };
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

            var si = SrcInfo;
            switch (Read())
            {
                case "++":
                    {
                        var target = v as Var;
                        if (target == null)
                            throw Abort("++: �Ώۂ��ϐ��ł͂���܂���B");
                        return new PostInc(parent, target) { SrcInfo = si };
                    }
                case "--":
                    {
                        var target = v as Var;
                        if (target == null)
                            throw Abort("--: �Ώۂ��ϐ��ł͂���܂���B");
                        return new PostDec(parent, target) { SrcInfo = si };
                    }
            }
            Rewind();
            return v;
        }

        private IIntValue Value()
        {
            if (!CanRead) throw Abort("��������܂���B");

            var iv = Integer();
            if (iv != null) return iv;

            var sv = String();
            if (sv != null) return sv;

            var si = SrcInfo;
            var t = Read();
            switch (t)
            {
                case "null":
                    return new Null(parent) { SrcInfo = si };
                case "true":
                    return new IntValue(1) { SrcInfo = si };
                case "false":
                    return new IntValue(0) { SrcInfo = si };
                case "__FUNCTION__":
                    return new StringValue(parent.GetName());
                case "__FILE__":
                    return new StringValue(parent.Root.Source);
                case "sizeof":
                    {
                        var br = Read();
                        if (br != "(")
                            if (br != null) Rewind();
                        var arg = Read();
                        if (arg == null)
                            throw Abort("sizeof: �������K�v�ł��B");
                        if (br == "(") Check("sizeof", ")");
                        return new SizeOf(parent, arg) { SrcInfo = si };
                    }
                case "addrof":
                    {
                        var ex = Expression() as Var;
                        if (ex == null)
                            throw parent.Abort(si, "addrof: �������s�K�؂ł��B");
                        return new AddrOf(parent, ex) { SrcInfo = si };
                    }
                case "typeof":
                    {
                        var ex = Expression() as VarBase;
                        if (ex == null)
                            throw parent.Abort(si, "typeof: �������s�K�؂ł��B");
                        return new TypeOf(parent, ex) { SrcInfo = si };
                    }
                case "__LLPML__":
                    return new StringValue("LLPML ver." + Root.LLPMLVersion);
            }

            var vd = parent.GetVar(t);
            var v = vd != null ? new Var(parent, vd) { SrcInfo = si } : null;

            var pd = parent.GetPointer(t);
            var p = pd != null ? new Pointer(parent, pd) { SrcInfo = si } : null;

            switch (Read())
            {
                case null:
                    break;
                case "(":
                    {
                        var args = Arguments(",", ")", false);
                        if (args == null)
                            throw Abort("{0}: �������s���S�ł��B", t);
                        if (v != null)
                            return new Call(parent, v, null, args) { SrcInfo = si };
                        return new Call(parent, t, null, args) { SrcInfo = si };
                    }
                default:
                    Rewind();
                    break;
            }

            if (v != null) return v;
            if (p != null) return p;

            var i = parent.GetInt(t);
            if (i != null) return new IntValue((int)i) { SrcInfo = si };

            var s = parent.GetString(t);
            if (s != null) return new StringValue(s);

            // ����`����֐��|�C���^�Ƃ��ĉ���
            if (Tokenizer.IsWord(t))
                return new Function.Ptr(parent, t) { SrcInfo = si };

            Rewind();
            throw Abort("�]���ł��܂���: {0}", t);
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

        private Struct.Cast Cast()
        {
            var si = SrcInfo;
            var br1 = Read();
            if (br1 == "(")
            {
                var type = Read();
                if (Tokenizer.IsWord(type) && parent.GetPointer(type) == null)
                {
                    var br2 = Read();
                    if (br2 == "[")
                    {
                        br2 = Read();
                        if (br2 == "]")
                        {
                            br2 = Read();
                            type += "[]";
                        }
                        else
                        {
                            Rewind();
                            br2 = "[";
                        }
                    }
                    if (br2 == ")")
                        return new Struct.Cast(parent, type, Expression()) { SrcInfo = si };
                    Rewind();
                }
                Rewind();
            }
            Rewind();
            return null;
        }
    }
}
