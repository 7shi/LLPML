using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Function : Block
    {
        public class Ptr : VarBase
        {
            public Ptr() { }
            public Ptr(Block parent, string name) : base(parent, name) { }
            public Ptr(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void Read(XmlTextReader xr)
            {
                NoChild(xr);

                name = xr["name"];
                if (name == null) throw Abort(xr, "name required");
            }

            public Val32 GetAddress(Module m)
            {
                Function f = parent.GetFunction(name);
                if (f == null)
                    throw new Exception("undefined function: " + name);
                return new Val32(m.Specific.ImageBase, f.Address);
            }
        }
    }
}
