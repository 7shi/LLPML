using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Do : BlockBase
    {
        public Cond Cond { get; set; }
        public Block Block { get; set; }

        public override bool AcceptsBreak { get { return true; } }
        public override bool AcceptsContinue { get { return true; } }
        public override Val32 Continue { get { return Block.Last; } }

        public Do(BlockBase parent) : base(parent) { }
        public Do(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            base.Read(xr);
            Parse(xr, delegate
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xr.Name)
                        {
                            case "cond":
                                if (Cond != null)
                                    throw Abort(xr, "multiple conditions");
                                Cond = new Cond(this, xr);
                                break;
                            case "block":
                                if (Block != null)
                                    throw Abort(xr, "multiple blocks");
                                Block = new Block(this, xr);
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
            if (Cond == null && Block == null)
                throw Abort(xr, "condition and block required");
            else if (Cond == null)
                throw Abort(xr, "condition required");
            else if (Block == null)
                throw Abort(xr, "block required");
        }

        public override void AddCodes(OpModule codes)
        {
            sentences.Clear();
            Cond.First = Block.First;
            AddSentence(Block);
            AddSentence(Cond);
            base.AddCodes(codes);
        }
    }
}
