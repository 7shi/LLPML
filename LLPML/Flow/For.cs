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
        public Block Init;
        public Cond Cond;
        public Block Loop, Block;

        public override bool AcceptsBreak { get { return true; } }
        public override bool AcceptsContinue { get { return true; } }
        public override Val32 Continue { get { return Block.Last; } }

        public For(BlockBase parent) : base(parent) { }
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
                                if (Init != null)
                                    throw Abort(xr, "multiple initializers");
                                Init = new Block(this);
                                Init.Target = this;
                                Init.SetLine(xr);
                                Init.Read(xr);
                                break;
                            case "cond":
                                if (Cond != null)
                                    throw Abort(xr, "multiple conditions");
                                Cond = new Cond(this, xr);
                                break;
                            case "loop":
                                if (Loop != null)
                                    throw Abort(xr, "multiple loops");
                                Loop = new Block(this, xr);
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
            if (Block == null) throw Abort(xr, "block required");
        }

        protected override void BeforeAddCodes(OpCodes codes)
        {
            base.BeforeAddCodes(codes);
            if (Init != null) Init.AddCodes(codes);
            if (Loop != null)
                codes.Add(I386.Jmp(Loop.Last));
            else
                codes.Add(I386.Jmp(Block.Last));
        }

        public override void AddCodes(OpCodes codes)
        {
            sentences.Clear();
            sentences.Add(Block);
            if (Loop != null) sentences.Add(Loop);
            if (Cond == null)
                Cond = new Cond(this, new IntValue(1));
            Cond.First = Block.First;
            sentences.Add(Cond);
            base.AddCodes(codes);
        }
    }
}
