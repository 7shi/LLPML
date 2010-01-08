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

        private void SetUsingPointers()
        {
            if (parent == null) return;
            var list = new List<Var.Declare>();
            foreach (var obj in parent.GetMembers<Var.Declare>())
                if (!(obj is Arg)) list.Add(obj);
            UsingPointers = list.ToArray();
        }

        public BreakBase()
        {
        }

        public BreakBase(BlockBase parent)
            : base(parent)
        {
            SetUsingPointers();
        }

        public BreakBase(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            SetUsingPointers();
        }
    }
}
