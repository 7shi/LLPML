using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Call : NodeBase
    {
        public List<object> args;

        private string target;
        public string Target { get { return target; } }

        private Extern function;
        public Extern Function { get { return function; } }

        public Call() { }
        public Call(Root root, XmlTextReader xr) { Read(root, xr); }

        public override void Read(Root root, XmlTextReader xr)
        {
            target = xr["target"];
            function = root.GetFunction(target);
            args = new List<object>();
            Parse(xr, delegate
            {
                if (xr.NodeType == XmlNodeType.Element)
                {
                    switch (xr.Name)
                    {
                        case "int":
                            args.Add(root.ReadInt(xr));
                            break;
                        case "string":
                            args.Add(root.ReadString(xr));
                            break;
                        case "var-int":
                            args.Add(root.ReadVarInt(xr));
                            break;
                        case "ptr":
                            args.Add(root.ReadPointer(xr));
                            break;
                        default:
                            throw Abort(xr);
                    }
                }
            });
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            codes.AddRange(function.GetFunction(m).Invoke(args.ToArray()));
        }
    }
}
