using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Var : VarBase, IIntValue
    {
        public class Operator : NodeBase
        {
            protected Var dest;
            protected List<IIntValue> values = new List<IIntValue>();
            public IIntValue[] GetValues() { return values.ToArray(); }

            public virtual int Min { get { return 1; } }
            public virtual int Max { get { return int.MaxValue; } }

            public Operator() { }
            public Operator(BlockBase parent, Var dest)
                : base(parent)
            {
                this.dest = dest;
            }

            public Operator(BlockBase parent, Var dest, IntValue[] values)
                : this(parent, dest)
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
                    IIntValue[] vs = IntValue.Read(parent, xr);
                    if (vs != null)
                    {
                        foreach (IIntValue v in vs)
                        {
                            if (dest == null)
                            {
                                if (!(v is Var)) throw Abort(xr, "no variable specified");
                                dest = v as Var;
                            }
                            else
                            {
                                if (values.Count == Max)
                                    throw Abort(xr, "too many operands");
                                values.Add(v);
                            }
                        }
                    }
                });
                if (dest == null)
                    throw Abort(xr, "no variable specified");
                else if (Min > 0 && values.Count == 0)
                    throw Abort(xr, "no value specified");
                else if (values.Count < Min)
                    throw Abort(xr, "too few operands");
            }
        }
    }
}
