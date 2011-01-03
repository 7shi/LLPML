using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.LLPML.Parsing;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class Operator : NodeBase
    {
        public abstract string Tag { get; }

        protected ArrayList values = new ArrayList();

        protected static Operator Init1(Operator op, BlockBase parent, NodeBase arg1)
        {
            return Init2(op, parent, arg1, null);
        }

        protected static Operator Init2(Operator op, BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            op.Parent = parent;
            if (arg1 != null) op.values.Add(arg1);
            if (arg2 != null) op.values.Add(arg2);
            return op;
        }

        protected TypeBase CheckFunc()
        {
            var t = (values[0] as NodeBase).Type;
            if (t == null) t = TypeVar.Instance;
            if (!t.CheckFunc(Tag))
                throw Abort("{0}: {1}: not supported", Tag, t.Name);
            return t;
        }

        protected CondPair GetCond()
        {
            var t = (values[0] as NodeBase).Type;
            if (t == null) t = TypeVar.Instance;
            var c = t.GetCond(Tag);
            if (c == null)
                throw Abort("{0}: {1}: no conditions", Tag, t.Name);
            return c;
        }

        public override TypeBase Type
        {
            get
            {
                if (values.Count == 0) throw new Exception(Tag + ": no arguments");
                return (values[0] as NodeBase).Type;
            }
        }

        public abstract IntValue GetConst();

        protected bool AddConstCodes(OpModule codes, string op, Addr32 dest)
        {
            var v = GetConst();
            if (v == null) return false;

            v.AddCodesV(codes, op, dest);
            return true;
        }
    }
}
