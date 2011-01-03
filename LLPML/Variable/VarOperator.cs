using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;
using Girl.LLPML.Parsing;

namespace Girl.LLPML
{
    public abstract class VarOperator : NodeBase
    {
        public abstract string Tag { get; }

        protected NodeBase dest;
        protected List<NodeBase> values = new List<NodeBase>();

        protected static VarOperator Init0(VarOperator op, BlockBase parent, NodeBase dest, SrcInfo si)
        {
            op.Parent = parent;
            op.dest = dest;
            if (si != null) op.SrcInfo = si;
            return op;
        }

        protected static VarOperator Init1(VarOperator op, BlockBase parent, NodeBase dest, NodeBase arg)
        {
            op.Parent = parent;
            op.dest = dest;
            if (arg != null) op.values.Add(arg);
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
