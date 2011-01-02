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
        protected Block() { }
        public Block(BlockBase parent) { init(parent); }

        public BlockBase Target { get; set; }

        public static NodeBase[] ReadText(BlockBase parent, Tokenizer token)
        {
            var parser = Parser.New(token, parent);
            var ret = parser.Parse();
            if (token.CanRead) ret = null;
            return ret;
        }

        public void ReadText(string file, string src)
        {
            var t = Tokenizer.New(file, src);
            var sents = Block.ReadText(Target ?? this, t);
            if (sents != null) AddSentences(sents);
        }
    }
}
