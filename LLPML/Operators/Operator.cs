using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class Operator : NodeBase
    {
        public abstract string Tag { get; }

        protected List<NodeBase> values = new List<NodeBase>();
        public NodeBase[] GetValues() { return values.ToArray(); }

        public virtual int Min { get { return 2; } }
        public virtual int Max { get { return int.MaxValue; } }

        public Operator(BlockBase parent, params NodeBase[] values)
            : base(parent)
        {
            if (values.Length < Min)
                throw Abort("too few operands");
            else if (values.Length > Max)
                throw Abort("too many operands");
            this.values.AddRange(values);
        }

        public Operator(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                var vs = IntValue.Read(Parent, xr);
                if (vs == null) return;
                foreach (NodeBase v in vs)
                {
                    if (values.Count == Max)
                        throw Abort(xr, "too many operands");
                    values.Add(v);
                }
            });
            if (Min > 0 && values.Count == 0)
                throw Abort(xr, "no value specified");
            else if (values.Count < Min)
                throw Abort(xr, "too few operands");
        }

        protected TypeBase.Func GetFunc()
        {
            var t = values[0].Type ?? TypeVar.Instance;
            var f = t.GetFunc(Tag);
            if (f == null)
                throw Abort("{0}: {1}: not supported", Tag, t.Name);
            return f;
        }

        protected CondPair GetCond()
        {
            var t = values[0].Type ?? TypeVar.Instance;
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
                return values[0].Type;
            }
        }

        public abstract IntValue GetConst();

        protected bool AddConstCodes(OpModule codes, string op, Addr32 dest)
        {
            var v = GetConst();
            if (v == null) return false;

            v.AddCodes(codes, op, dest);
            return true;
        }
    }
}
