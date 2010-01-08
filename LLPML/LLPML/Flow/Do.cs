using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Do : NodeBase
    {
        private Block block;
        private Cond cond;

        public Do() { }
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
                            case "block":
                                if (block != null)
                                    throw Abort(xr, "multiple blocks");
                                block = new Block(parent, xr);
                                break;
                            case "cond":
                                if (block == null)
                                    throw Abort(xr, "block required before condition");
                                else if (cond != null)
                                    throw Abort(xr, "multiple conditions");
                                cond = new Cond(parent, xr);
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
            if (block == null)
                throw Abort(xr, "block and condition required");
            else if (cond == null)
                throw Abort(xr, "condition required");
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            block.AddCodes(codes, m);
            cond.AddPostCodes(codes, m, block.First);
        }
    }
}
