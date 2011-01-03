using System;
using System.Collections;
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

        protected void init(BlockBase parent)
        {
            Parent = parent;
            if (parent != null)
                UsingPointers = parent.GetUsingPointers();
        }
    }
}
