using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Switch : BlockBase
    {
        private class Expression : NodeBase
        {
            private IIntValue value;

            public Expression(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

            public override void Read(XmlTextReader xr)
            {
                Parse(xr, delegate
                {
                    IIntValue[] v = IntValue.Read(parent, xr);
                    if (v != null)
                    {
                        if (v.Length > 1 || value != null)
                            throw Abort(xr, "multiple expressions");
                        else
                            value = v[0];
                    }
                });
                if (value == null)
                    throw Abort(xr, "expression required");
            }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                value.AddCodes(codes, m, "mov", null);
                codes.Add(I386.Mov(Reg32.EDX, Reg32.EAX));
            }
        }

        private class Case : NodeBase
        {
            private List<IIntValue> values = new List<IIntValue>();
            public Block Block;
            public bool IsLast;

            public Case(BlockBase parent) : base(parent) { }
            public Case(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

            public override void Read(XmlTextReader xr)
            {
                Parse(xr, delegate
                {
                    IIntValue[] vs = IntValue.Read(parent, xr);
                    if (vs != null)
                    {
                        foreach (IIntValue v in vs)
                        {
                            if (!(v is IntValue))
                                throw Abort(xr, "constant required");
                            values.Add(v);
                        }
                    }
                });
                if (values.Count == 0)
                    throw Abort(xr, "value(s) required");
            }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                int len = values.Count;
                if (len == 0)
                {
                    codes.Add(I386.Jmp(Block.First));
                }
                else
                {
                    for (int i = 0; i < len; i++)
                    {
                        values[i].AddCodes(codes, m, "mov", null);
                        codes.Add(I386.Cmp(Reg32.EDX, Reg32.EAX));
                        codes.Add(I386.Jcc(Cc.E, Block.First));
                    }
                    if (IsLast) codes.Add(I386.Jmp(parent.Destruct));
                }
            }
        }

        private class CaseBlock
        {
            public Case Case;
            public Block Block;

            public CaseBlock(Case c) : this(c, null) { }
            public CaseBlock(Block block) : this(null, block) { }

            public CaseBlock(Case c, Block block)
            {
                Case = c;
                Block = block;
            }
        }

        private Expression expr;
        private List<CaseBlock> blocks = new List<CaseBlock>();

        public override bool AcceptsBreak { get { return true; } }

        public Switch(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            base.Read(xr);
            CaseBlock cb = null;
            bool stop = false;
            Parse(xr, delegate
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xr.Name)
                        {
                            case "expr":
                                {
                                    expr = new Expression(this, xr);
                                    break;
                                }
                            case "case":
                                if (expr == null)
                                    throw Abort(xr, "expression required before case");
                                else if (stop)
                                    throw Abort(xr, "block terminated");
                                else if (cb != null)
                                    throw Abort(xr, "multiple cases");
                                cb = new CaseBlock(new Case(this, xr));
                                break;
                            case "block":
                                if (expr == null)
                                    throw Abort(xr, "expression required before block");
                                else if (stop)
                                    throw Abort(xr, "block terminated");
                                else if (cb == null)
                                {
                                    blocks.Add(new CaseBlock(
                                        new Case(this), new Block(this, xr)));
                                    stop = true;
                                }
                                else
                                {
                                    cb.Block = new Block(this, xr);
                                    blocks.Add(cb);
                                    cb = null;
                                }
                                break;
                            default:
                                throw Abort(xr);
                        }
                        break;

                    case XmlNodeType.Whitespace:
                    case XmlNodeType.Comment:
                        break;

                    default:
                        throw Abort(xr, "element required");
                }
            });
            if (expr == null)
                throw Abort(xr, "expression required");
            else if (cb != null)
                throw Abort(xr, "block required");
            else if (blocks.Count == 0)
                throw Abort(xr, "case and block required");
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            sentences.Clear();
            sentences.Add(expr);
            int len = blocks.Count;
            for (int i = 0; i < len; i++)
            {
                CaseBlock cb = blocks[i];
                cb.Case.Block = cb.Block;
                cb.Case.IsLast = i == len - 1;
                sentences.Add(cb.Case);
            }
            foreach (CaseBlock cb in blocks)
            {
                sentences.Add(cb.Block);
            }
            base.AddCodes(codes, m);
        }
    }
}
