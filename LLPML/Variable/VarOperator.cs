using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Girl.LLPML.Parsing;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class VarOperator : NodeBase
    {
        public abstract string Tag { get; }

        protected NodeBase dest;
        protected ArrayList values = new ArrayList();

        protected static VarOperator Init0(VarOperator op, BlockBase parent, NodeBase dest)
        {
            op.Parent = parent;
            op.dest = dest;
            return op;
        }

        protected static VarOperator Init1(VarOperator op, BlockBase parent, NodeBase dest, NodeBase arg)
        {
            op.Parent = parent;
            op.dest = dest;
            op.values.Add(arg);
            return op;
        }

        protected TypeBase CheckFunc()
        {
            var t = Type;
            if (!t.CheckFunc(Tag))
                throw new Exception(Tag + ": " + t.Name + ": not supported");
            return t;
        }

        public override TypeBase Type { get { return dest.Type; } }
    }
}
