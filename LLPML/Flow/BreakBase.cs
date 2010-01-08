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
        public Var.Declare[] UsingPointers { get; protected set; }

        public BreakBase()
        {
        }

        public BreakBase(BlockBase parent)
            : base(parent)
        {
            if (parent != null)
                UsingPointers = parent.GetMembers<Var.Declare>();
        }

        public BreakBase(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            if (parent != null)
                UsingPointers = parent.GetMembers<Var.Declare>();
        }
    }
}
