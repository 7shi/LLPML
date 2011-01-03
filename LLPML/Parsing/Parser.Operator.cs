using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Girl.LLPML.Struct;

namespace Girl.LLPML.Parsing
{
    public partial class Parser
    {
        private OperatorBase[][] operators;
        private Hashtable[] orders;

        private void Init()
        {
            operators = new OperatorBase[][]
            {
                new[]
                {
                    new VarOperator("="  , (dest, arg) => Set.New(parent, dest, arg)),
                    new VarOperator("+=" , (dest, arg) => VarAdd.New(parent, dest, arg)),
                    new VarOperator("-=" , (dest, arg) => VarSub.New(parent, dest, arg)),
                    new VarOperator("*=" , (dest, arg) => VarMul.New(parent, dest, arg)),
                    new VarOperator("/=" , (dest, arg) => VarDiv.New(parent, dest, arg)),
                    new VarOperator("%=" , (dest, arg) => VarMod.New(parent, dest, arg)),
                    new VarOperator("&=" , (dest, arg) => VarAnd.New(parent, dest, arg)),
                    new VarOperator("|=" , (dest, arg) => VarOr.New(parent, dest, arg)),
                    new VarOperator("^=" , (dest, arg) => VarXor.New(parent, dest, arg)),
                    new VarOperator("<<=", (dest, arg) => VarShiftLeft.New(parent, dest, arg)),
                    new VarOperator(">>=", (dest, arg) => VarShiftRight.New(parent, dest, arg)),
                },
                new[]
                {
                    new Operator("|>", PipeForward),
                    new Operator("|<", PipeBack),
                },
                new[]
                {
                    new Operator("&&"     , (arg1, arg2) => AndAlso.New(parent, arg1, arg2)),
                    new Operator("||"     , (arg1, arg2) => OrElse.New(parent, arg1, arg2)),
                    new Operator("andalso", (arg1, arg2) => AndAlso.New(parent, arg1, arg2)),
                    new Operator("orelse" , (arg1, arg2) => OrElse.New(parent, arg1, arg2)),
                },
                new[]
                {
                    new Operator("&"  , (arg1, arg2) => And.New(parent, arg1, arg2)),
                    new Operator("|"  , (arg1, arg2) => Or.New(parent, arg1, arg2)),
                    new Operator("^"  , (arg1, arg2) => Xor.New(parent, arg1, arg2)),
                    new Operator("and", (arg1, arg2) => And.New(parent, arg1, arg2)),
                    new Operator("or" , (arg1, arg2) => Or.New(parent, arg1, arg2)),
                    new Operator("xor", (arg1, arg2) => Xor.New(parent, arg1, arg2)),
                },
                new[]
                {
                    new Operator("==", (arg1, arg2) => Equal.New(parent, arg1, arg2)),
                    new Operator("!=", (arg1, arg2) => NotEqual.New(parent, arg1, arg2)),
                },
                new[]
                {
                    new Operator("<" , (arg1, arg2) => Less.New(parent, arg1, arg2)),
                    new Operator(">" , (arg1, arg2) => Greater.New(parent, arg1, arg2)),
                    new Operator("<=", (arg1, arg2) => LessEqual.New(parent, arg1, arg2)),
                    new Operator(">=", (arg1, arg2) => GreaterEqual.New(parent, arg1, arg2)),
                    new Operator("is", (arg1, arg2) => Is.New(parent, arg1, arg2)),
                    new Operator("as", (arg1, arg2) => As.New(parent, arg1, arg2)),
                },
                new[]
                {
                    new Operator("<<" , (arg1, arg2) => ShiftLeft.New(parent, arg1, arg2)),
                    new Operator(">>" , (arg1, arg2) => ShiftRight.New(parent, arg1, arg2)),
                    new Operator("shl", (arg1, arg2) => ShiftLeft.New(parent, arg1, arg2)),
                    new Operator("shr", (arg1, arg2) => ShiftRight.New(parent, arg1, arg2)),
                },
                new[]
                {
                    new Operator("+", (arg1, arg2) => Add.New(parent, arg1, arg2)),
                    new Operator("-", (arg1, arg2) => Sub.New(parent, arg1, arg2)),
                },
                new[]
                {
                    new Operator("*", (arg1, arg2) => Mul.New(parent, arg1, arg2)),
                    new Operator("/", (arg1, arg2) => Div.New(parent, arg1, arg2)),
                    new Operator("%", (arg1, arg2) => Mod.New(parent, arg1, arg2)),
                },
                new[]
                {
                    new WrapOperator("(", ReadCall),
                    new WrapOperator(".", ReadMember),
                    new WrapOperator("[", ReadIndex),
                    new WrapOperator("++", (target, order) => PostInc.New(parent, target)),
                    new WrapOperator("--", (target, order) => PostDec.New(parent, target)),
                },
            };

            orders = new Hashtable[operators.Length];
            var reserved = new List<string>();
            for (int i = 0; i < operators.Length; i++)
            {
                orders[i] = new Hashtable();
                for (int j = 0; j < operators[i].Length; j++)
                {
                    var op = operators[i][j];
                    orders[i].Add(op.Name, op);
                    if (op.Name.Length > 1) reserved.Add(op.Name);
                }
            }
            reserved.Add("//");
            reserved.Add("/*");
            reserved.Add("=>");
            reserved.Add("::");
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
                return handler(arg, parser.ReadExpressionOrder(order + assoc));
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
                return handler(arg, parser.ReadExpressionOrder(order + assoc));
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
