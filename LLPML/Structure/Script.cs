using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.LLPML.Parsing;

namespace Girl.LLPML
{
    public class Script : NodeBase
    {
        public Script(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Comment:
                        {
                            var t = new Tokenizer(parent.Root.Source, xr.Value,
                                xr.LineNumber, xr.LinePosition);
                            var sents = Block.ReadText(parent, t);
                            if (sents != null) parent.Sentences.AddRange(sents);
                            break;
                        }

                    case XmlNodeType.Whitespace:
                        break;

                    default:
                        throw Abort(xr, "script in comment required");
                }
            });
        }
    }
}
