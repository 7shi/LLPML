using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.LLPML.Parsing
{
    public partial class Parser
    {
        private OperatorBase[][] operators;
        private Dictionary<string, OperatorBase>[] orders;

        private void Init()
        {
            operators = new OperatorBase[][]
            {
                new[]
                {
                    new VarOperator("="  , (dest, arg) => new Set(parent, dest, arg)),
                    new VarOperator("+=" , (dest, arg) => new Var.Add(parent, dest, arg)),
                    new VarOperator("-=" , (dest, arg) => new Var.Sub(parent, dest, arg)),
                    new VarOperator("*=" , (dest, arg) => new Var.Mul(parent, dest, arg)),
                    new VarOperator("/=" , (dest, arg) => new Var.Div(parent, dest, arg)),
                    new VarOperator("%=" , (dest, arg) => new Var.Mod(parent, dest, arg)),
                    new VarOperator("&=" , (dest, arg) => new Var.And(parent, dest, arg)),
                    new VarOperator("|=" , (dest, arg) => new Var.Or(parent, dest, arg)),
                    new VarOperator("^=" , (dest, arg) => new Var.Xor(parent, dest, arg)),
                    new VarOperator("<<=", (dest, arg) => new Var.ShiftLeft(parent, dest, arg)),
                    new VarOperator(">>=", (dest, arg) => new Var.ShiftRight(parent, dest, arg)),
                },
                new[]
                {
                    new Operator("|>", PipeForward),
                    new Operator("|<", PipeBack),
                },
                new[]
                {
                    new Operator("&&"     , (arg1, arg2) => new AndAlso(parent, arg1, arg2)),
                    new Operator("||"     , (arg1, arg2) => new OrElse(parent, arg1, arg2)),
                    new Operator("andalso", (arg1, arg2) => new AndAlso(parent, arg1, arg2)),
                    new Operator("orelse" , (arg1, arg2) => new OrElse(parent, arg1, arg2)),
                },
                new[]
                {
                    new Operator("&"  , (arg1, arg2) => new And(parent, arg1, arg2)),
                    new Operator("|"  , (arg1, arg2) => new Or(parent, arg1, arg2)),
                    new Operator("^"  , (arg1, arg2) => new Xor(parent, arg1, arg2)),
                    new Operator("and", (arg1, arg2) => new And(parent, arg1, arg2)),
                    new Operator("or" , (arg1, arg2) => new Or(parent, arg1, arg2)),
                    new Operator("xor", (arg1, arg2) => new Xor(parent, arg1, arg2)),
                },
                new[]
                {
                    new Operator("==", (arg1, arg2) => new Equal(parent, arg1, arg2)),
                    new Operator("!=", (arg1, arg2) => new NotEqual(parent, arg1, arg2)),
                },
                new[]
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
                new[]
                {
                    new Operator("<<" , (arg1, arg2) => new ShiftLeft(parent, arg1, arg2)),
                    new Operator(">>" , (arg1, arg2) => new ShiftRight(parent, arg1, arg2)),
                    new Operator("shl", (arg1, arg2) => new ShiftLeft(parent, arg1, arg2)),
                    new Operator("shr", (arg1, arg2) => new ShiftRight(parent, arg1, arg2)),
                },
                new[]
                {
                    new Operator("+", (arg1, arg2) => new Add(parent, arg1, arg2)),
                    new Operator("-", (arg1, arg2) => new Sub(parent, arg1, arg2)),
                },
                new[]
                {
                    new Operator("*", (arg1, arg2) => new Mul(parent, arg1, arg2)),
                    new Operator("/", (arg1, arg2) => new Div(parent, arg1, arg2)),
                    new Operator("%", (arg1, arg2) => new Mod(parent, arg1, arg2)),
                },
                new[]
                {
                    new WrapOperator("(", Call),
                    new WrapOperator(".", Member),
                    new WrapOperator("[", Index),
                    new WrapOperator("++", PostInc),
                    new WrapOperator("--", PostDec),
                },
            };

            orders = new Dictionary<string, OperatorBase>[operators.Length];
            var reserved = new List<string>();
            for (var i = 0; i < operators.Length; i++)
            {
                orders[i] = new Dictionary<string, OperatorBase>();
                foreach (var op in operators[i])
                {
                    orders[i].Add(op.Name, op);
                    if (op.Name.Length > 1) reserved.Add(op.Name);
                }
            }
            reserved.AddRange(new[] { "//", "/*", "=>" });
            tokenizer.Reserved = reserved.ToArray();
        }

        private delegate IIntValue NodeDelegate(IIntValue arg1, IIntValue arg2);
        private delegate IIntValue VarDelegate(Var dest, IIntValue value);
        private delegate IIntValue WrapDelegate(IIntValue arg, int order);

        private abstract class OperatorBase
        {
            public string Name { get; private set; }
            protected int assoc;

            protected OperatorBase(string name, int assoc)
            {
                Name = name;
                this.assoc = assoc;
            }

            public abstract IIntValue Read(IIntValue arg, int order);
        }

        private class Operator : OperatorBase
        {
            private NodeDelegate handler;

            public Operator(string name, NodeDelegate handler)
                : base(name, 1)
            {
                this.handler = handler;
            }

            public override IIntValue Read(IIntValue arg, int order)
            {
                Parser parser = handler.Target as Parser;
                return handler(arg, parser.Expression(order + assoc));
            }
        }

        private class VarOperator : OperatorBase
        {
            private VarDelegate handler;

            public VarOperator(string name, VarDelegate handler)
                : base(name, 0)
            {
                this.handler = handler;
            }

            public override IIntValue Read(IIntValue arg, int order)
            {
                Parser parser = handler.Target as Parser;
                var dest = arg as Var;
                if (arg is Function.Ptr)
                {
                    var fp = arg as Function.Ptr;
                    if (fp.GetSetter() != null)
                    {
                        var m = new Struct.Member(fp.Parent, fp.Name);
                        m.Target = new Struct.This(fp.Parent);
                        dest = m;
                    }
                }
                if (dest == null)
                    throw parser.Abort("ç∂ï”Ç…ë„ì¸Ç≈Ç´Ç‹ÇπÇÒ: {0}", Name);
                return handler(dest, parser.Expression(order + assoc));
            }
        }

        private class WrapOperator : OperatorBase
        {
            private WrapDelegate handler;

            public WrapOperator(string name, WrapDelegate handler)
                : base(name, 0)
            {
                this.handler = handler;
            }

            public override IIntValue Read(IIntValue target, int order)
            {
                return handler(target, order);
            }
        }
    }
}
