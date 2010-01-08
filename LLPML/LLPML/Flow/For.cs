using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class For : Block
    {
        private Init init;
        private Cond cond;
        private Block block;

        public For() { }
        public For(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            bool loop = false;
            Parse(xr, delegate
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xr.Name)
                        {
                            case "init":
                                if (init != null)
                                    throw Abort(xr, "multiple initializers");
                                init = new Init(this, xr);
                                break;
                            case "cond":
                                if (cond != null)
                                    throw Abort(xr, "multiple conditions");
                                cond = new Cond(this, xr);
                                break;
                            case "loop":
                                if (loop)
                                    throw Abort(xr, "multiple loops");
                                Parse(xr, delegate { ReadBlock(xr); });
                                loop = true;
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
            if (block == null)
                throw Abort(xr, "block required");
        }

        protected override void BeforeAddCodes(List<OpCode> codes, Module m)
        {
            base.BeforeAddCodes(codes, m);
            if (init != null) init.AddCodes(codes, m);
            codes.Add(I386.Jmp(destruct.Address));
            block.AddCodes(codes, m);
        }

        protected override void AfterAddCodes(List<OpCode> codes, Module m)
        {
            cond.AddPostCodes(codes, m, block.First);
            base.AfterAddCodes(codes, m);
        }
    }
}
