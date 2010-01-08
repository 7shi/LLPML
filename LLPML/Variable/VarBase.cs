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
        public VarBase(BlockBase parent) : base(parent) { }
        public VarBase(BlockBase parent, string name) : base(parent, name) { }
        public VarBase(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
    }
}
