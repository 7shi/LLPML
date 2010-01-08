using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeOf : NodeBase, IIntValue
    {
        public VarBase Target { get; private set; }

        public TypeOf(BlockBase parent, VarBase target) : base(parent) { Target = target; }
        public TypeOf(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                var vs = IntValue.Read(parent, xr);
                if (vs == null) return;
                foreach (var v in vs)
                {
                    if (Target != null)
                        throw Abort(xr, "too many operands");
                    Target = v as VarBase;
                    if (Target == null)
                        throw Abort(xr, "variable required");
                }
            });
            if (Target == null)
                throw Abort(xr, "target required");
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            var t = Target.Type;
            if (t == null) t = "var";
            IntValue.AddCodes(codes, op, dest, m.GetString(t));
        }
    }
}
