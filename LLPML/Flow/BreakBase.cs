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
        public VarDeclare[] UsingPointers { get; protected set; }

        private void SetUsingPointers()
        {
            if (Parent == null) return;
            var list = new List<VarDeclare>();
            foreach (var obj in Parent.GetMembers<VarDeclare>())
                if (!(obj is Arg)) list.Add(obj);
            UsingPointers = list.ToArray();
        }

        protected void init(BlockBase parent)
        {
            Parent = parent;
            SetUsingPointers();
        }
    }
}
