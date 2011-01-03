using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Girl.LLPML.Struct;

namespace Girl.LLPML.Parsing
{
    public partial class Parser
    {
        private StringCollection[] orders;

        private void InitOperator()
        {
            orders = new StringCollection[10];
            for (int i = 0; i < orders.Length; i++)
                orders[i] = new StringCollection();

            orders[0].Add("=");
            orders[0].Add("+=");
            orders[0].Add("-=");
            orders[0].Add("*=");
            orders[0].Add("/=");
            orders[0].Add("%=");
            orders[0].Add("&=");
            orders[0].Add("|=");
            orders[0].Add("^=");
            orders[0].Add("<<=");
            orders[0].Add(">>=");

            orders[1].Add("|>");
            orders[1].Add("|<");

            orders[2].Add("&&");
            orders[2].Add("||");

            orders[3].Add("&");
            orders[3].Add("|");
            orders[3].Add("^");

            orders[4].Add("==");
            orders[4].Add("!=");

            orders[5].Add("<");
            orders[5].Add(">");
            orders[5].Add("<=");
            orders[5].Add(">=");
            orders[5].Add("is");
            orders[5].Add("as");

            orders[6].Add("<<");
            orders[6].Add(">>");

            orders[7].Add("+");
            orders[7].Add("-");

            orders[8].Add("*");
            orders[8].Add("/");
            orders[8].Add("%");

            orders[9].Add("(");
            orders[9].Add(".");
            orders[9].Add("[");
            orders[9].Add("++");
            orders[9].Add("--");

            var reserved = new List<string>();
            for (int i = 0; i < orders.Length; i++)
            {
                var ops = orders[i];
                for (int j = 0; j < ops.Count; j++)
                {
                    var op = ops[j];
                    if (op.Length > 1) reserved.Add(op);
                }
            }
            reserved.Add("//");
            reserved.Add("/*");
            reserved.Add("=>");
            reserved.Add("::");
            tokenizer.Reserved = reserved.ToArray();
        }

        public NodeBase ReadOperator(int order, string op, NodeBase arg1)
        {
            if (order == 0)
            {
                var arg2 = ReadExpressionOrder(order);
                switch (op)
                {
                    case "=": return Set.New(parent, arg1, arg2);
                    case "+=": return VarAdd.New(parent, arg1, arg2);
                    case "-=": return VarSub.New(parent, arg1, arg2);
                    case "*=": return VarMul.New(parent, arg1, arg2);
                    case "/=": return VarDiv.New(parent, arg1, arg2);
                    case "%=": return VarMod.New(parent, arg1, arg2);
                    case "&=": return VarAnd.New(parent, arg1, arg2);
                    case "|=": return VarOr.New(parent, arg1, arg2);
                    case "^=": return VarXor.New(parent, arg1, arg2);
                    case "<<=": return VarShiftLeft.New(parent, arg1, arg2);
                    case ">>=": return VarShiftRight.New(parent, arg1, arg2);
                }
            }
            else if (order < 9)
            {
                var arg2 = ReadExpressionOrder(order + 1);
                switch (op)
                {
                    case "|>": return PipeForward(arg1, arg2);
                    case "|<": return PipeBack(arg1, arg2);
                    case "&&": return AndAlso.New(parent, arg1, arg2);
                    case "||": return OrElse.New(parent, arg1, arg2);
                    case "&": return And.New(parent, arg1, arg2);
                    case "|": return Or.New(parent, arg1, arg2);
                    case "^": return Xor.New(parent, arg1, arg2);
                    case "==": return Equal.New(parent, arg1, arg2);
                    case "!=": return NotEqual.New(parent, arg1, arg2);
                    case "<": return Less.New(parent, arg1, arg2);
                    case ">": return Greater.New(parent, arg1, arg2);
                    case "<=": return LessEqual.New(parent, arg1, arg2);
                    case ">=": return GreaterEqual.New(parent, arg1, arg2);
                    case "is": return Is.New(parent, arg1, arg2);
                    case "as": return As.New(parent, arg1, arg2);
                    case "<<": return ShiftLeft.New(parent, arg1, arg2);
                    case ">>": return ShiftRight.New(parent, arg1, arg2);
                    case "+": return Add.New(parent, arg1, arg2);
                    case "-": return Sub.New(parent, arg1, arg2);
                    case "*": return Mul.New(parent, arg1, arg2);
                    case "/": return Div.New(parent, arg1, arg2);
                    case "%": return Mod.New(parent, arg1, arg2);
                }
            }
            else
            {
                switch (op)
                {
                    case "(": return ReadCall(arg1);
                    case ".": return ReadMember(arg1);
                    case "[": return ReadIndex(arg1);
                    case "++": return PostInc.New(parent, arg1);
                    case "--": return PostDec.New(parent, arg1);
                }
            }
            return null;
        }
    }
}
