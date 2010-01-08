using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class BreakBase : NodeBase
    {
        protected Pointer.Declare[] usingPointers;
        public Pointer.Declare[] UsingPointers { get { return usingPointers; } }

        public BreakBase() { }
        public BreakBase(BlockBase parent) : base(parent) { }
        public BreakBase(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            if (parent != null) usingPointers = parent.GetPointers();
        }
    }
}
