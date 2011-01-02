using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class AddrOf : NodeBase
    {
        public NodeBase Target { get; private set; }

        public AddrOf(BlockBase parent, NodeBase target) : base(parent) { Target = target; }
        public AddrOf(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                var vs = IntValue.Read(Parent, xr);
                if (vs == null) return;
                foreach (var v in vs)
                {
                    if (Target != null)
                        throw Abort(xr, "too many operands");
                    Target = v;
                }
            });
            if (Target == null)
                throw Abort(xr, "target required");
        }

        public override TypeBase Type { get { return new TypePointer(Target.Type); } }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            var t = Var.Get(Target);
            if (t == null)
                throw Abort("addrof: variable required");
            var ad = t.GetAddress(codes);
            codes.Add(I386.Lea(Reg32.EAX, ad));
            codes.AddCodes(op, dest);
        }
    }
}
