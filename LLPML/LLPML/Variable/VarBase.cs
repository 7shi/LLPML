using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.X86;

namespace Girl.LLPML
{
    public class VarBase : NodeBase
    {
        public VarBase() { }
        public VarBase(Block parent, string name) : base(parent, name) { }
        public VarBase(Block parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
