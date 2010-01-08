using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML
{
    public class Pointer : NodeBase
    {
        string name;
        public string Name { get { return name; } }

        Ref<uint> ptr = null;
        int length = 0;

        public Pointer() { }
        public Pointer(Root root, XmlTextReader xr) { Read(root, xr); }

        public override void Read(Root root, XmlTextReader xr)
        {
            name = xr["name"];
            ptr = root.GetPointer(name);
            Parse(xr, delegate
            {
                if (xr.NodeType == XmlNodeType.Element)
                {
                    int c = 1;
                    string count = xr["count"];
                    if (count != null) c = int.Parse(count);
                    switch (xr.Name)
                    {
                        case "int":
                            length = 4 * c;
                            break;
                        case "byte":
                            length = c;
                            break;
                        default:
                            throw Abort(xr);
                    }
                }
            });
        }

        public override void AddCodes(List<OpCode> codes, Girl.PE.Module m)
        {
            if (!ptr.IsInitialized && length > 0) ptr.Set(m.GetBuffer(name, length));
        }
    }
}
