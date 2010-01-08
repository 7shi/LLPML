using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class If : NodeBase
    {
        private class CondBlock
        {
            public Cond Cond;
            public Block Block;

            public CondBlock() { }
            public CondBlock(Cond cond) : this(cond, null) { }
            public CondBlock(Block block) : this(null, block) { }

            public CondBlock(Cond cond, Block block)
            {
                Cond = cond;
                Block = block;
            }
        }

        private List<CondBlock> blocks = new List<CondBlock>();

        public If() { }
        public If(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            CondBlock cb = null;
            bool stop = false;
            Parse(xr, delegate
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xr.Name)
                        {
                            case "cond":
                                if (stop)
                                    throw Abort(xr, "block terminated");
                                else if (cb != null)
                                    throw Abort(xr, "multiple contitions");
                                cb = new CondBlock(new Cond(parent, xr));
                                break;
                            case "block":
                                if (stop)
                                    throw Abort(xr, "block terminated");
                                else if (cb == null)
                                {
                                    if (blocks.Count == 0)
                                        throw Abort(xr, "condition required");
                                    blocks.Add(new CondBlock(new Block(parent, xr)));
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
            if (cb != null)
                throw Abort(xr, "block required");
            else if (blocks.Count == 0)
                throw Abort(xr, "condition and block required");
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            int len = blocks.Count;
            OpCode[] nexts = new OpCode[len];
            for (int i = 0; i < len; i++)
                nexts[i] = new OpCode();
            OpCode last = nexts[len - 1];
            for (int i = 0; i < len; i++)
            {
                OpCode next = nexts[i];
                CondBlock cb = blocks[i];
                if (cb.Cond != null)
                    cb.Cond.AddPreCodes(codes, m, next.Address);
                cb.Block.AddCodes(codes, m);
                if (last != next) codes.Add(I386.Jmp(last.Address));
                codes.Add(next);
            }
        }
    }
}
