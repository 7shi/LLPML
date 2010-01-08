using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.LLPML.Parsing
{
    public class Parser
    {
        private Tokenizer tokenizer;
        private BlockBase parent;
        private bool CanRead { get { return tokenizer.CanRead; } }
        private string Read() { return tokenizer.Read(); }
        private void Rewind() { tokenizer.Rewind(); }

        private Operator[][] operators;
        private string[] reserved;
        private Dictionary<string, Operator>[] orders;

        public Parser(Tokenizer tokenizer, BlockBase parent)
        {
            this.tokenizer = tokenizer;
            this.parent = parent;
            operators = new Operator[][]
            {
                new Operator[]
                {
                    new Operator("=", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        Var dest = arg1 as Var;
                        return dest == null ? null : new Let(parent, dest, arg2);
                    }),
                    new Operator("+=", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        Var dest = arg1 as Var;
                        return dest == null ? null : new Var.Add(parent, dest, arg2);
                    }),
                    new Operator("-=", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        Var dest = arg1 as Var;
                        return dest == null ? null : new Var.Sub(parent, dest, arg2);
                    }),
                    new Operator("*=", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        Var dest = arg1 as Var;
                        return dest == null ? null : new Var.Mul(parent, dest, arg2);
                    }),
                    new Operator("/=", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        Var dest = arg1 as Var;
                        return dest == null ? null : new Var.Div(parent, dest, arg2);
                    }),
                    new Operator("%=", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        Var dest = arg1 as Var;
                        return dest == null ? null : new Var.Mod(parent, dest, arg2);
                    }),
                    new Operator("&=", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        Var dest = arg1 as Var;
                        return dest == null ? null : new Var.And(parent, dest, arg2);
                    }),
                    new Operator("|=", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        Var dest = arg1 as Var;
                        return dest == null ? null : new Var.Or(parent, dest, arg2);
                    }),
                    new Operator("^=", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        Var dest = arg1 as Var;
                        return dest == null ? null : new Var.Xor(parent, dest, arg2);
                    }),
                    new Operator("<<=", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        Var dest = arg1 as Var;
                        return dest == null ? null : new Var.ShiftLeft(parent, dest, arg2);
                    }),
                    new Operator(">>=", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        Var dest = arg1 as Var;
                        return dest == null ? null : new Var.ShiftRight(parent, dest, arg2);
                    }),
                },
                new Operator[]
                {
                    new Operator("&&", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new AndAlso(parent, arg1, arg2);
                    }),
                    new Operator("||", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new OrElse(parent, arg1, arg2);
                    }),
                    new Operator("andalso", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new AndAlso(parent, arg1, arg2);
                    }),
                    new Operator("orelse", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new OrElse(parent, arg1, arg2);
                    }),
                },
                new Operator[]
                {
                    new Operator("&", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new And(parent, arg1, arg2);
                    }),
                    new Operator("|", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new Or(parent, arg1, arg2);
                    }),
                    new Operator("^", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new Xor(parent, arg1, arg2);
                    }),
                    new Operator("and", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new And(parent, arg1, arg2);
                    }),
                    new Operator("or", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new Or(parent, arg1, arg2);
                    }),
                    new Operator("xor", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new Xor(parent, arg1, arg2);
                    }),
                },
                new Operator[]
                {
                    new Operator("==", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new Equal(parent, arg1, arg2);
                    }),
                    new Operator("!=", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new NotEqual(parent, arg1, arg2);
                    }),
                },
                new Operator[]
                {
                    new Operator("<", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new Less(parent, arg1, arg2);
                    }),
                    new Operator(">", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new Greater(parent, arg1, arg2);
                    }),
                    new Operator("<=", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new LessEqual(parent, arg1, arg2);
                    }),
                    new Operator(">=", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new GreaterEqual(parent, arg1, arg2);
                    }),
                    new Operator("lt", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new Less(parent, arg1, arg2);
                    }),
                    new Operator("gt", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new Greater(parent, arg1, arg2);
                    }),
                    new Operator("le", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new LessEqual(parent, arg1, arg2);
                    }),
                    new Operator("ge", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new GreaterEqual(parent, arg1, arg2);
                    }),
                },
                new Operator[]
                {
                    new Operator("<<", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new ShiftLeft(parent, arg1, arg2);
                    }),
                    new Operator(">>", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new ShiftRight(parent, arg1, arg2);
                    }),
                    new Operator("shl", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new ShiftLeft(parent, arg1, arg2);
                    }),
                    new Operator("shr", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new ShiftRight(parent, arg1, arg2);
                    }),
                },
                new Operator[]
                {
                    new Operator("+", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new Add(parent, arg1, arg2);
                    }),
                    new Operator("-", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new Sub(parent, arg1, arg2);
                    }),
                },
                new Operator[]
                {
                    new Operator("*", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new Mul(parent, arg1, arg2);
                    }),
                    new Operator("/", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new Div(parent, arg1, arg2);
                    }),
                    new Operator("%", delegate(IIntValue arg1, IIntValue arg2)
                    {
                        return new Mod(parent, arg1, arg2);
                    }),
                },
            };
            reserved = new string[] { "++", "--" };
            Init();
        }

        public IIntValue[] ParseExpressions()
        {
            return Arguments(",", null, false);
        }

        public NodeBase[] Parse()
        {
            IIntValue[] exprs = Arguments(";", null, true);
            if (exprs == null) return null;
            NodeBase[] ret = new NodeBase[exprs.Length];
            for (int i = 0; i < exprs.Length; i++)
            {
                if (exprs[i] is NodeBase)
                    ret[i] = exprs[i] as NodeBase;
                else
                    return null;
            }
            return ret;
        }

        public static string GetString(string s)
        {
            StringBuilder sb = new StringBuilder();
            bool esc = false;
            foreach (char ch in s)
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

        #region Operator

        private void Init()
        {
            orders = new Dictionary<string, Operator>[operators.Length];
            List<string> reserved = new List<string>();
            for (int i = 0; i < operators.Length; i++)
            {
                orders[i] = new Dictionary<string, Operator>();
                foreach (Operator op in operators[i])
                {
                    orders[i].Add(op.Name, op);
                    if (op.Name.Length > 1) reserved.Add(op.Name);
                }
            }
            reserved.AddRange(this.reserved);
            tokenizer.Reserved = reserved.ToArray();
        }

        private delegate IIntValue NodeDelegate(IIntValue arg1, IIntValue arg2);

        private class Operator
        {
            public string Name;
            public NodeDelegate Handler;

            public Operator(string name, NodeDelegate handler)
            {
                Name = name;
                Handler = handler;
            }
        }

        #endregion

        // Expression ::= Factor (operator Factor)*
        private IIntValue Expression(int order)
        {
            if (!CanRead) return null;
            if (order >= operators.Length) return Factor();

            IIntValue ret = Expression(order + 1);
            if (ret == null) return null;

            while (CanRead)
            {
                string t = Read();
                Operator op;
                if (orders[order].TryGetValue(t, out op))
                {
                    ret = op.Handler(ret, Expression(order + 1));
                    if (ret == null) return null;
                }
                else
                {
                    Rewind();
                    return ret;
                }
            }
            return ret;
        }

        // Factor ::= Group | Unary
        private IIntValue Factor()
        {
            IIntValue ret = Group();
            if (ret == null) ret = Unary();
            return ret;
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
            IIntValue ret = Expression(0);
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
            if (!CanRead) return null;

            IIntValue ret;
            string t = Read();
            switch (t)
            {
                case "+":
                case "-":
                    {
                        IntValue v = Integer();
                        if (v != null)
                        {
                            if (t == "-")
                                ret = new IntValue(-v.Value);
                            else
                                ret = v;
                        }
                        else
                        {
                            IIntValue n = Expression(0);
                            if (n == null) return null;
                            if (t == "-")
                                ret = new Neg(parent, n);
                            else
                                ret = n;
                        }
                        break;
                    }
                case "!":
                    {
                        IIntValue n = Expression(0);
                        if (n == null) return null;
                        ret = new Not(parent, n);
                        break;
                    }
                case "~":
                    {
                        IIntValue n = Expression(0);
                        if (n == null) return null;
                        ret = new Rev(parent, n);
                        break;
                    }
                case "++":
                    {
                        Var var = Primary() as Var;
                        if (var == null) return null;
                        ret = new Inc(parent, var);
                        break;
                    }
                case "--":
                    {
                        Var var = Primary() as Var;
                        if (var == null) return null;
                        ret = new Dec(parent, var);
                        break;
                    }
                default:
                    Rewind();
                    ret = Primary();
                    break;
            }
            return ret;
        }

        private IIntValue Primary()
        {
            IIntValue ret = Value();
            if (ret == null || !CanRead) return ret;

            switch (Read())
            {
                case "++":
                    {
                        Var var = ret as Var;
                        if (var == null) return null;
                        ret = new PostInc(parent, var);
                        break;
                    }
                case "--":
                    {
                        Var var = ret as Var;
                        if (var == null) return null;
                        ret = new PostDec(parent, var);
                        break;
                    }
                case ".":
                    {
                        break;
                    }
                default:
                    Rewind();
                    break;
            }
            return ret;
        }

        private IIntValue Value()
        {
            IntValue v = Integer();
            if (v != null) return v;

            StringValue sv = String();
            if (sv != null) return sv;

            string t = Read();
            switch (t)
            {
                case null:
                    return null;
                case "null":
                    return new Null(parent);
                case "__FUNCTION__":
                    return new StringValue(parent.GetName());
                case "__FILE__":
                    return new StringValue(parent.Root.Source);
            }

            Var.Declare vd = parent.GetVar(t);
            Var var = vd != null ? new Var(parent, vd) : null;

            Pointer.Declare p = parent.GetPointer(t);
            Pointer ptr = p != null ? new Pointer(parent, p) : null;

            switch (Read())
            {
                case null:
                    break;
                case ".":
                    {
                        Struct.Declare st = p as Struct.Declare;
                        if (var == null && st == null) return null;
                        Struct.Invoke inv;
                        Struct.Member m = Member(out inv);
                        if (m == null)
                        {
                            if (inv == null)
                                return null;
                            else if (var != null)
                                return new Struct.Invoke(inv, var);
                            else if (st != null)
                                return new Struct.Invoke(inv, ptr);
                        }
                        if (var != null)
                            m.Var = var;
                        else if (st != null)
                            m.Ptr = st;
                        if (inv == null) return m;
                        return new Struct.Invoke(inv, m);
                    }
                case "(":
                    {
                        IIntValue[] args = Arguments(",", ")", false);
                        if (args == null) return null;
                        if (var != null)
                            return new Call(parent, var, args);
                        return new Call(parent, t, args);
                    }
                default:
                    Rewind();
                    break;
            }

            if (var != null) return var;
            if (ptr != null) return ptr;

            int? i = parent.GetInt(t);
            if (i != null) return new IntValue((int)i);

            string s = parent.GetString(t);
            if (s != null) return new StringValue(s);

            Rewind();
            return null;
        }

        private IntValue Integer()
        {
            string t = Read();
            if (t == null) return null;

            foreach (char ch in t)
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
            string t = Read();
            if (t == null) return null;
            if (t.Length < 2 || !t.StartsWith("\"") || !t.EndsWith("\""))
            {
                Rewind();
                return null;
            }
            return new StringValue(GetString(t.Substring(1, t.Length - 2)));
        }

        private IIntValue[] Arguments(string sep, string end, bool mustSep)
        {
            string br = Read();
            if (br == null) return null;

            List<IIntValue> ret = new List<IIntValue>();
            if (br == end) return ret.ToArray();
            Rewind();

            while (CanRead)
            {
                IIntValue arg = Expression(0);
                if (arg == null) return null;
                ret.Add(arg);
                string t = Read();
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

        private Struct.Member Member(out Struct.Invoke inv)
        {
            inv = null;
            string name = Read();
            if (name == null) return null;

            Struct.Member child = null;
            switch (Read())
            {
                case null:
                    break;
                case ".":
                    child = Member(out inv);
                    break;
                case "(":
                    {
                        IIntValue[] args = Arguments(",", ")", false);
                        if (args != null)
                            inv = new Struct.Invoke(parent, name, args);
                        return null;
                    }
                default:
                    Rewind();
                    break;
            }
            return new Struct.Member(parent, name, child);
        }
    }
}
