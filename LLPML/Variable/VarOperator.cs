using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class VarOperator : NodeBase
    {
        public abstract string Tag { get; }

        protected NodeBase dest;
        protected List<NodeBase> values = new List<NodeBase>();
        public NodeBase[] GetValues() { return values.ToArray(); }

        public virtual int Min { get { return 1; } }
        public virtual int Max { get { return int.MaxValue; } }

        public VarOperator() { }
        public VarOperator(BlockBase parent, NodeBase dest)
            : base(parent)
        {
            this.dest = dest;
        }

        public VarOperator(BlockBase parent, NodeBase dest, params NodeBase[] values)
            : this(parent, dest)
        {
            if (values.Length < Min)
                throw Abort("too few operands");
            else if (values.Length > Max)
                throw Abort("too many operands");
            this.values.AddRange(values);
        }

        protected TypeBase.Func GetFunc()
        {
            var t = Type;
            var f = t.GetFunc(Tag);
            if (f == null)
                throw new Exception(Tag + ": " + t.Name + ": not supported");
            return f;
        }

        public override TypeBase Type { get { return dest.Type; } }
    }
}
