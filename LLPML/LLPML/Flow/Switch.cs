using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Switch : NodeBase
    {
        private class Case : Operator
        {
            public override int Min { get { return 1; } }
            public Case(Block parent, XmlTextReader xr) : base(parent, xr) { }
        }

        private class CaseBlock
        {
            public Case Case;
            public Block Block;

            public CaseBlock() { }
            public CaseBlock(Case c) : this(c, null) { }
            public CaseBlock(Block block) : this(null, block) { }

            public CaseBlock(Case c, Block block)
            {
                Case = c;
                Block = block;
            }
        }

        public IIntValue expr;
        private List<CaseBlock> blocks = new List<CaseBlock>();

        public Switch() { }
        public Switch(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
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
                                Parse(xr, delegate
                                {
                                    IIntValue v = IntValue.Read(parent, xr, false);
                                    if (v != null)
                                    {
                                        if (expr != null)
                                            throw Abort(xr, "multiple expressions");
                                        else
                                            expr = v;
                                    }
                                });
                                break;
                            case "case":
                                if (expr == null)
                                    throw Abort(xr, "expression required before case");
                                else if (stop)
                                    throw Abort(xr, "block terminated");
                                else if (cb != null)
                                    throw Abort(xr, "multiple cases");
                                cb = new CaseBlock(new Case(parent, xr));
                                break;
                            case "block":
                                if (expr == null)
                                    throw Abort(xr, "expression required before block");
                                else if (stop)
                                    throw Abort(xr, "block terminated");
                                else if (cb == null)
                                {
                                    blocks.Add(new CaseBlock(new Block(parent, xr)));
                                    stop = true;
                                }
                                else
                                {
                                    cb.Block = new Block(parent, xr);
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
            expr.AddCodes(codes, m, "push", null);
            Addr32 ad = new Addr32(Reg32.ESP);
            int len = blocks.Count;
            OpCode[] nexts = new OpCode[len];
            for (int i = 0; i < len; i++)
                nexts[i] = new OpCode();
            OpCode last = nexts[len - 1];
            for (int i = 0; i < len; i++)
            {
                OpCode next = nexts[i];
                CaseBlock cb = blocks[i];
                if (cb.Case != null)
                {
                    IIntValue[] values = cb.Case.GetValues();
                    int len2 = values.Length;
                    for (int j = 0; j < len2; j++)
                    {
                        values[j].AddCodes(codes, m, "mov", null);
                        codes.Add(I386.Cmp(ad, Reg32.EAX));
                        if (j < len2 - 1)
                            codes.Add(I386.Jcc(Cc.E, cb.Block.First));
                        else
                            codes.Add(I386.Jcc(Cc.NE, next.Address));
                    }
                }
                cb.Block.AddCodes(codes, m);
                if (last != next) codes.Add(I386.Jmp(last.Address));
                codes.Add(next);
            }
            codes.Add(I386.Add(Reg32.ESP, 4));
        }
    }
}
