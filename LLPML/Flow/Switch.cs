using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{

    public class Case : NodeBase
    {
        private List<NodeBase> values = new List<NodeBase>();
        public List<NodeBase> Values { get { return values; } }

        public Block Block;
        public bool IsLast;

        public static Case New(BlockBase parent)
        {
            var ret = new Case();
            ret.Parent = parent;
            return ret;
        }

        public override void AddCodes(OpModule codes)
        {
            int len = values.Count;
            if (len == 0)
            {
                codes.Add(I386.JmpD(Block.First));
                return;
            }

            foreach (var v in values)
            {
                codes.Add(I386.Push(Reg32.EDX));
                if (v.Type is TypeString)
                {
                    v.AddCodesV(codes, "push", null);
                    codes.Add(codes.GetCall("case", TypeString.Equal));
                    codes.Add(I386.AddR(Reg32.ESP, Val32.New(8)));
                    codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
                    codes.Add(I386.Jcc(Cc.NZ, Block.First));
                }
                else
                {
                    v.AddCodesV(codes, "mov", null);
                    codes.Add(I386.Pop(Reg32.EDX));
                    codes.Add(I386.Cmp(Reg32.EDX, Reg32.EAX));
                    codes.Add(I386.Jcc(Cc.E, Block.First));
                }
            }
            if (IsLast) codes.Add(I386.JmpD(Parent.Destruct));
        }
    }

    public class CaseBlock
    {
        public Case Case { get; set; }
        public Block Block { get; set; }
    }

    public class Switch : BlockBase
    {
        private class Expression : NodeBase
        {
            private NodeBase value;

            public static Expression New(BlockBase parent, NodeBase value)
            {
                var ret = new Expression();
                ret.Parent = parent;
                ret.value = value;
                return ret;
            }

            public override void AddCodes(OpModule codes)
            {
                value.AddCodesV(codes, "mov", null);
                codes.Add(I386.Mov(Reg32.EDX, Reg32.EAX));
            }
        }

        private Expression expr;

        private List<CaseBlock> blocks = new List<CaseBlock>();
        public List<CaseBlock> Blocks { get { return blocks; } }

        public override bool AcceptsBreak { get { return true; } }

        public static Switch New(BlockBase parent, NodeBase expr)
        {
            var ret = new Switch();
            ret.init(parent);
            ret.expr = Expression.New(ret, expr);
            return ret;
        }

        public override void AddCodes(OpModule codes)
        {
            sentences.Clear();
            AddSentence(expr);

            CaseBlock def = null;
            var list = new List<CaseBlock>();
            foreach (var cb in blocks)
            {
                if (cb.Case.Values.Count == 0)
                {
                    if (def != null)
                        throw cb.Case.Abort("switch: multiple defaults");
                    else
                        def = cb;
                }
                else
                    list.Add(cb);
            }
            if (def != null) list.Add(def);

            int len = list.Count;
            for (int i = 0; i < len; i++)
            {
                var cb = list[i];
                cb.Case.Block = cb.Block;
                cb.Case.IsLast = i == len - 1;
                AddSentence(cb.Case);
            }
            foreach (var cb in list)
                AddSentence(cb.Block);
            base.AddCodes(codes);
        }
    }
}
