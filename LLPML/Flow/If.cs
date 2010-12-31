using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class If : BlockBase
    {
        public class CondBlock : NodeBase
        {
            public Cond Cond { get; set; }
            public Block Block { get; set; }
            public CondBlock Next { get; set; }

            private OpCode first = new OpCode();
            public Val32 First { get { return first.Address; } }

            public CondBlock(If parent) : base(parent) { }
            public CondBlock(If parent, Cond cond) : this(parent, cond, null) { }
            public CondBlock(If parent, Block block) : this(parent, null, block) { }

            public CondBlock(If parent, Cond cond, Block block)
                : this(parent)
            {
                Cond = cond;
                Block = block;
            }

            public override void AddCodes(OpModule codes)
            {
                OpCode next = new OpCode();
                codes.Add(first);
                if (Cond != null)
                {
                    Cond.Next = next.Address;
                    Cond.AddCodes(codes);
                }
                Block.AddCodes(codes);
                if (Next != null)
                    codes.Add(I386.JmpD(Parent.Destruct));
                codes.Add(next);
            }
        }

        private List<CondBlock> blocks = new List<CondBlock>();
        public List<CondBlock> Blocks { get { return blocks; } }

        public If(BlockBase parent) : base(parent) { }
        public If(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            base.Read(xr);
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
                                cb = new CondBlock(this, new Cond(this, xr));
                                break;
                            case "block":
                                if (stop)
                                    throw Abort(xr, "block terminated");
                                else if (cb == null)
                                {
                                    if (blocks.Count == 0)
                                        throw Abort(xr, "condition required");
                                    blocks.Add(new CondBlock(this, new Block(this, xr)));
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
            if (cb != null)
                throw Abort(xr, "block required");
            else if (blocks.Count == 0)
                throw Abort(xr, "condition and block required");
        }

        public override void AddCodes(OpModule codes)
        {
            sentences.Clear();
            int len = blocks.Count;
            for (int i = 0; i < len; i++)
            {
                CondBlock cb = blocks[i];
                cb.Next = i < len - 1 ? blocks[i + 1] : null;
                AddSentence(cb);
            }
            base.AddCodes(codes);
        }
    }
}
