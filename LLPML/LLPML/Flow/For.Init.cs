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
        public class Init : NodeBase
        {
            private List<NodeBase> sentences = new List<NodeBase>();

            public Init(Block parent) : base(parent) { }
            public Init(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void Read(XmlTextReader xr)
            {
                Parse(xr, delegate
                {
                    switch (xr.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (xr.Name)
                            {
                                case "var-declare":
                                    sentences.Add(new Var.Declare(parent, xr));
                                    break;
                                case "let":
                                    sentences.Add(new Let(parent, xr));
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
            }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                foreach (NodeBase n in sentences)
                {
                    n.AddCodes(codes, m);
                }
            }
        }
    }
}
