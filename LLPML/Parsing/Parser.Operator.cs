﻿using System;
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
                    new VarOperator("+=" , (dest, arg) => new VarAdd(parent, dest, arg)),
                    new VarOperator("-=" , (dest, arg) => new VarSub(parent, dest, arg)),
                    new VarOperator("*=" , (dest, arg) => new VarMul(parent, dest, arg)),
                    new VarOperator("/=" , (dest, arg) => new VarDiv(parent, dest, arg)),
                    new VarOperator("%=" , (dest, arg) => new VarMod(parent, dest, arg)),
                    new VarOperator("&=" , (dest, arg) => new VarAnd(parent, dest, arg)),
                    new VarOperator("|=" , (dest, arg) => new VarOr(parent, dest, arg)),
                    new VarOperator("^=" , (dest, arg) => new VarXor(parent, dest, arg)),
                    new VarOperator("<<=", (dest, arg) => new VarShiftLeft(parent, dest, arg)),
                    new VarOperator(">>=", (dest, arg) => new VarShiftRight(parent, dest, arg)),
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
                    new Operator("is", (arg1, arg2) => new Struct.Is(parent, arg1, arg2)),
                    new Operator("as", (arg1, arg2) => new Struct.As(parent, arg1, arg2)),
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
                    new WrapOperator("++", (target, order) => new PostInc(parent, target)),
                    new WrapOperator("--", (target, order) => new PostDec(parent, target)),
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
            reserved.AddRange(new[] { "//", "/*", "=>", "::" });
            tokenizer.Reserved = reserved.ToArray();
        }

        private delegate NodeBase NodeDelegate(NodeBase arg1, NodeBase arg2);
        private delegate NodeBase VarDelegate(NodeBase dest, NodeBase value);
        private delegate NodeBase WrapDelegate(NodeBase arg, int order);

        private abstract class OperatorBase
        {
            public string Name { get; private set; }
            protected int assoc;

            protected OperatorBase(string name, int assoc)
            {
                Name = name;
                this.assoc = assoc;
            }

            public abstract NodeBase Read(NodeBase arg, int order);
        }

        private class Operator : OperatorBase
        {
            private NodeDelegate handler;

            public Operator(string name, NodeDelegate handler)
                : base(name, 1)
            {
                this.handler = handler;
            }

            public override NodeBase Read(NodeBase arg, int order)
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

            public override NodeBase Read(NodeBase arg, int order)
            {
                Parser parser = handler.Target as Parser;
                return handler(arg, parser.Expression(order + assoc));
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

            public override NodeBase Read(NodeBase target, int order)
            {
                return handler(target, order);
            }
        }
    }
}
