using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Do : Block
    {
        protected Cond cond;
        protected Block block;

        public override bool AcceptsBreak { get { return true; } }
        public override bool AcceptsContinue { get { return true; } }
        public override Val32 Continue { get { return block.Last; } }

        public Do(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xr.Name)
                        {
                            case "cond":
                                if (cond != null)
                                    throw Abort(xr, "multiple conditions");
                                cond = new Cond(this, xr);
                                break;
                            case "block":
                                if (block != null)
                                    throw Abort(xr, "multiple blocks");
                                block = new Block(this, xr);
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
            if (cond == null && block == null)
                throw Abort(xr, "condition and block required");
            else if (cond == null)
                throw Abort(xr, "condition required");
            else if (block == null)
                throw Abort(xr, "block required");
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            sentences.Clear();
            cond.First = block.First;
            sentences.Add(block);
            sentences.Add(cond);
            base.AddCodes(codes, m);
        }
    }
}
