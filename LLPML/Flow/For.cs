using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class For : BlockBase
    {
        private Init init;
        private Cond cond;
        private Block loop, block;

        public override bool AcceptsBreak { get { return true; } }
        public override bool AcceptsContinue { get { return true; } }
        public override Val32 Continue { get { return block.Last; } }

        public For(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

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
                                if (loop != null)
                                    throw Abort(xr, "multiple loops");
                                loop = new Block(this, xr);
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
            if (block == null) throw Abort(xr, "block required");
        }

        protected override void BeforeAddCodes(List<OpCode> codes, Module m)
        {
            base.BeforeAddCodes(codes, m);
            if (init != null) init.AddCodes(codes, m);
            if (loop != null)
                codes.Add(I386.Jmp(loop.Last));
            else
                codes.Add(I386.Jmp(block.Last));
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            sentences.Clear();
            sentences.Add(block);
            if (loop != null) sentences.Add(loop);
            if (cond == null)
                cond = new Cond(this, new IntValue(1));
            cond.First = block.First;
            sentences.Add(cond);
            base.AddCodes(codes, m);
        }
    }
}
