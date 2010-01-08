using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Operands : NodeBase
    {
        protected List<IIntValue> values = new List<IIntValue>();

        public Operands() { }
        public Operands(Block parent, string name) : base(parent, name) { }

        public Operands(Block parent, string name, IntValue[] values)
            : this(parent, name)
        {
            this.values.AddRange(values);
        }

        public Operands(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                IIntValue v = IntValue.Read(parent, xr, true);
                if (v != null) values.Add(v);
            });
            if (values.Count == 0) throw Abort(xr, "operand required");
        }
    }
}
