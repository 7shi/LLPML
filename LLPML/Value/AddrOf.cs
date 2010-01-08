using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class AddrOf : NodeBase, IIntValue
    {
        public Var Target { get; private set; }

        public AddrOf(BlockBase parent, Var target) : base(parent) { Target = target; }
        public AddrOf(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

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
                    Target = v as Var;
                    if (Target == null)
                        throw Abort(xr, "variable required");
                }
            });
            if (Target == null)
                throw Abort(xr, "target required");
        }

        public TypeBase Type { get { return new TypePointer(Target.Type); } }

        public void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            var ad = Target.GetAddress(codes);
            codes.Add(I386.Lea(Reg32.EAX, ad));
            codes.AddCodes(op, dest);
        }
    }
}
