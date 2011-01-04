using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;
using Girl.LLPML.Parsing;

namespace Girl.LLPML
{
    public class Block : BlockBase
    {
        public static Block New(BlockBase parent)
        {
            var ret = new Block();
            ret.init(parent);
            return ret;
        }

        public BlockBase Target { get; set; }

        public static NodeBase[] Parse(BlockBase parent, Tokenizer token)
        {
            var parser = Parser.Create(token, parent);
            var ret = parser.Parse();
            if (token.CanRead) ret = null;
            return ret;
        }

        public void ReadText(string file, string src)
        {
            var t = Tokenizer.New(file, src);
            var target = Target;
            if (target == null) target = this;
            var sents = Block.Parse(target, t);
            if (sents != null) AddSentences(sents);
        }
    }
}
