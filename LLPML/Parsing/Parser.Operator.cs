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
                    new VarOperator(this, "="  , 0, (dest, arg) => new Let(parent, dest, arg)),
                    new VarOperator(this, "+=" , 0, (dest, arg) => new Var.Add(parent, dest, arg)),
                    new VarOperator(this, "-=" , 0, (dest, arg) => new Var.Sub(parent, dest, arg)),
                    new VarOperator(this, "*=" , 0, (dest, arg) => new Var.Mul(parent, dest, arg)),
                    new VarOperator(this, "/=" , 0, (dest, arg) => new Var.Div(parent, dest, arg)),
                    new VarOperator(this, "%=" , 0, (dest, arg) => new Var.Mod(parent, dest, arg)),
                    new VarOperator(this, "&=" , 0, (dest, arg) => new Var.And(parent, dest, arg)),
                    new VarOperator(this, "|=" , 0, (dest, arg) => new Var.Or(parent, dest, arg)),
                    new VarOperator(this, "^=" , 0, (dest, arg) => new Var.Xor(parent, dest, arg)),
                    new VarOperator(this, "<<=", 0, (dest, arg) => new Var.ShiftLeft(parent, dest, arg)),
                    new VarOperator(this, ">>=", 0, (dest, arg) => new Var.ShiftRight(parent, dest, arg)),
                },
                new Operator[]
                {
                    new Operator("&&", 1, (arg1, arg2) => new AndAlso(parent, arg1, arg2)),
                    new Operator("||", 1, (arg1, arg2) => new OrElse(parent, arg1, arg2)),
                    new Operator("andalso", 1, (arg1, arg2) => new AndAlso(parent, arg1, arg2)),
                    new Operator("orelse" , 1, (arg1, arg2) => new OrElse(parent, arg1, arg2)),
                },
                new Operator[]
                {
                    new Operator("&", 1, (arg1, arg2) => new And(parent, arg1, arg2)),
                    new Operator("|", 1, (arg1, arg2) => new Or(parent, arg1, arg2)),
                    new Operator("^", 1, (arg1, arg2) => new Xor(parent, arg1, arg2)),
                    new Operator("and", 1, (arg1, arg2) => new And(parent, arg1, arg2)),
                    new Operator("or" , 1, (arg1, arg2) => new Or(parent, arg1, arg2)),
                    new Operator("xor", 1, (arg1, arg2) => new Xor(parent, arg1, arg2)),
                },
                new Operator[]
                {
                    new Operator("==", 1, (arg1, arg2) => new Equal(parent, arg1, arg2)),
                    new Operator("!=", 1, (arg1, arg2) => new NotEqual(parent, arg1, arg2)),
                },
                new Operator[]
                {
                    new Operator("<" , 1, (arg1, arg2) => new Less(parent, arg1, arg2)),
                    new Operator(">" , 1, (arg1, arg2) => new Greater(parent, arg1, arg2)),
                    new Operator("<=", 1, (arg1, arg2) => new LessEqual(parent, arg1, arg2)),
                    new Operator(">=", 1, (arg1, arg2) => new GreaterEqual(parent, arg1, arg2)),
                    new Operator("lt", 1, (arg1, arg2) => new Less(parent, arg1, arg2)),
                    new Operator("gt", 1, (arg1, arg2) => new Greater(parent, arg1, arg2)),
                    new Operator("le", 1, (arg1, arg2) => new LessEqual(parent, arg1, arg2)),
                    new Operator("ge", 1, (arg1, arg2) => new GreaterEqual(parent, arg1, arg2)),
                },
                new Operator[]
                {
                    new Operator("<<" , 1, (arg1, arg2) => new ShiftLeft(parent, arg1, arg2)),
                    new Operator(">>" , 1, (arg1, arg2) => new ShiftRight(parent, arg1, arg2)),
                    new Operator("shl", 1, (arg1, arg2) => new ShiftLeft(parent, arg1, arg2)),
                    new Operator("shr", 1, (arg1, arg2) => new ShiftRight(parent, arg1, arg2)),
                },
                new Operator[]
                {
                    new Operator("+", 1, (arg1, arg2) => new Add(parent, arg1, arg2)),
                    new Operator("-", 1, (arg1, arg2) => new Sub(parent, arg1, arg2)),
                },
                new Operator[]
                {
                    new Operator("*", 1, (arg1, arg2) => new Mul(parent, arg1, arg2)),
                    new Operator("/", 1, (arg1, arg2) => new Div(parent, arg1, arg2)),
                    new Operator("%", 1, (arg1, arg2) => new Mod(parent, arg1, arg2)),
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
            public int Associativity { get; private set; }
            public NodeDelegate Handler { get; protected set; }

            public Operator(string name, int assoc, NodeDelegate handler)
            {
                Name = name;
                Associativity = assoc;
                Handler = handler;
            }
        }

        private class VarOperator : Operator
        {
            private Parser parser;

            public VarOperator(Parser parser, string name, int assoc, VarDelegate handler)
                : base(name, assoc, null)
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
