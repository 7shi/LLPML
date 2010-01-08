using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Block : NodeBase
    {
        private List<NodeBase> sentences = new List<NodeBase>();

        public Block() { }
        public Block(Root root, XmlTextReader xr) { Read(root, xr); }

        protected void ReadBlock(Root root, XmlTextReader xr)
        {
            if (xr.NodeType == XmlNodeType.Element)
            {
                switch (xr.Name)
                {
                    case "extern":
                        new Extern(root, xr);
                        break;
                    case "call":
                        sentences.Add(new Call(root, xr));
                        break;
                    case "int":
                        root.ReadInt(xr);
                        break;
                    case "string":
                        root.ReadString(xr);
                        break;
                    case "var-int":
                        sentences.Add(new VarInt(root, xr));
                        break;
                    case "ptr":
                        sentences.Add(new Pointer(root, xr));
                        break;
                    case "loop":
                        sentences.Add(new Loop(root, xr));
                        break;
                    default:
                        throw Abort(xr);
                }
            }
            else if (xr.NodeType != XmlNodeType.Whitespace)
            {
                throw Abort(xr, "element required");
            }
        }

        public override void Read(Root root, XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                ReadBlock(root, xr);
            });
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            foreach (NodeBase child in sentences)
            {
                child.AddCodes(codes, m);
            }
        }
    }
}
