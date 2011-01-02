using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;
using Girl.LLPML.Parsing;

namespace Girl.LLPML
{
    public class Block : BlockBase
    {
        public Block() { }
        public Block(BlockBase parent) : base(parent) { }

        public BlockBase Target { get; set; }

        public static NodeBase[] ReadText(BlockBase parent, Tokenizer token)
        {
            Parser parser = new Parser(token, parent);
            NodeBase[] ret = parser.Parse();
            if (token.CanRead) ret = null;
            return ret;
        }

        public void ReadText(string file, string src)
        {
            var t = new Tokenizer(file, src);
            var sents = Block.ReadText(Target ?? this, t);
            if (sents != null) AddSentences(sents);
        }
    }
}
