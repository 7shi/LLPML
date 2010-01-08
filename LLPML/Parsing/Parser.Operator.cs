using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.LLPML.Parsing
{
    public partial class Parser
    {
        private Operator[][] operators;
        private Dictionary<string, Operator>[] orders;

        private void Init()
        {
            operators = new Operator[][]
            {
                new Operator[]
                {
                    new VarOperator(this, "="  , (dest, arg) => new Let(parent, dest, arg)),
                    new VarOperator(this, "+=" , (dest, arg) => new Var.Add(parent, dest, arg)),
                    new VarOperator(this, "-=" , (dest, arg) => new Var.Sub(parent, dest, arg)),
                    new VarOperator(this, "*=" , (dest, arg) => new Var.Mul(parent, dest, arg)),
                    new VarOperator(this, "/=" , (dest, arg) => new Var.Div(parent, dest, arg)),
                    new VarOperator(this, "%=" , (dest, arg) => new Var.Mod(parent, dest, arg)),
                    new VarOperator(this, "&=" , (dest, arg) => new Var.And(parent, dest, arg)),
                    new VarOperator(this, "|=" , (dest, arg) => new Var.Or(parent, dest, arg)),
                    new VarOperator(this, "^=" , (dest, arg) => new Var.Xor(parent, dest, arg)),
                    new VarOperator(this, "<<=", (dest, arg) => new Var.ShiftLeft(parent, dest, arg)),
                    new VarOperator(this, ">>=", (dest, arg) => new Var.ShiftRight(parent, dest, arg)),
                },
                new Operator[]
                {
                    new Operator("&&", (arg1, arg2) => new AndAlso(parent, arg1, arg2)),
                    new Operator("||", (arg1, arg2) => new OrElse(parent, arg1, arg2)),
                    new Operator("andalso", (arg1, arg2) => new AndAlso(parent, arg1, arg2)),
                    new Operator("orelse" , (arg1, arg2) => new OrElse(parent, arg1, arg2)),
                },
                new Operator[]
                {
                    new Operator("&", (arg1, arg2) => new And(parent, arg1, arg2)),
                    new Operator("|", (arg1, arg2) => new Or(parent, arg1, arg2)),
                    new Operator("^", (arg1, arg2) => new Xor(parent, arg1, arg2)),
                    new Operator("and", (arg1, arg2) => new And(parent, arg1, arg2)),
                    new Operator("or" , (arg1, arg2) => new Or(parent, arg1, arg2)),
                    new Operator("xor", (arg1, arg2) => new Xor(parent, arg1, arg2)),
                },
                new Operator[]
                {
                    new Operator("==", (arg1, arg2) => new Equal(parent, arg1, arg2)),
                    new Operator("!=", (arg1, arg2) => new NotEqual(parent, arg1, arg2)),
                },
                new Operator[]
                {
                    new Operator("<" , (arg1, arg2) => new Less(parent, arg1, arg2)),
                    new Operator(">" , (arg1, arg2) => new Greater(parent, arg1, arg2)),
                    new Operator("<=", (arg1, arg2) => new LessEqual(parent, arg1, arg2)),
                    new Operator(">=", (arg1, arg2) => new GreaterEqual(parent, arg1, arg2)),
                    new Operator("lt", (arg1, arg2) => new Less(parent, arg1, arg2)),
                    new Operator("gt", (arg1, arg2) => new Greater(parent, arg1, arg2)),
                    new Operator("le", (arg1, arg2) => new LessEqual(parent, arg1, arg2)),
                    new Operator("ge", (arg1, arg2) => new GreaterEqual(parent, arg1, arg2)),
                },
                new Operator[]
                {
                    new Operator("<<" , (arg1, arg2) => new ShiftLeft(parent, arg1, arg2)),
                    new Operator(">>" , (arg1, arg2) => new ShiftRight(parent, arg1, arg2)),
                    new Operator("shl", (arg1, arg2) => new ShiftLeft(parent, arg1, arg2)),
                    new Operator("shr", (arg1, arg2) => new ShiftRight(parent, arg1, arg2)),
                },
                new Operator[]
                {
                    new Operator("+", (arg1, arg2) => new Add(parent, arg1, arg2)),
                    new Operator("-", (arg1, arg2) => new Sub(parent, arg1, arg2)),
                },
                new Operator[]
                {
                    new Operator("*", (arg1, arg2) => new Mul(parent, arg1, arg2)),
                    new Operator("/", (arg1, arg2) => new Div(parent, arg1, arg2)),
                    new Operator("%", (arg1, arg2) => new Mod(parent, arg1, arg2)),
                },
            };

            orders = new Dictionary<string, Operator>[operators.Length];
            var reserved = new List<string>();
            for (var i = 0; i < operators.Length; i++)
            {
                orders[i] = new Dictionary<string, Operator>();
                foreach (var op in operators[i])
                {
                    orders[i].Add(op.Name, op);
                    if (op.Name.Length > 1) reserved.Add(op.Name);
                }
            }
            reserved.AddRange(new string[] { "++", "--", "//", "/*" });
            tokenizer.Reserved = reserved.ToArray();
        }

        private delegate IIntValue NodeDelegate(IIntValue arg1, IIntValue arg2);
        private delegate IIntValue VarDelegate(Var dest, IIntValue value);

        private class Operator
        {
            public string Name { get; private set; }
            public NodeDelegate Handler { get; protected set; }

            public Operator(string name, NodeDelegate handler)
            {
                Name = name;
                Handler = handler;
            }
        }

        private class VarOperator : Operator
        {
            private Parser parser;

            public VarOperator(Parser parser, string name, VarDelegate handler)
                : base(name, null)
            {
                this.parser = parser;
                Handler = (arg1, arg2) =>
                {
                    var dest = arg1 as Var;
                    if (dest == null)
                        throw parser.Abort("ç∂ï”Ç…ë„ì¸Ç≈Ç´Ç‹ÇπÇÒ: {0}", Name);
                    return handler(dest, arg2);
                };
            }
        }
    }
}
