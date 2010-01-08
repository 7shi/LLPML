using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Extern : Function
    {
        private string module, alias;

        public Extern() { }
        public Extern(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override DeclareBase[] GetArgs() { return null; }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);

            module = xr["module"];
            alias = xr["alias"];
            base.Read(xr);
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            codes.Add(first);
            string n = alias != null ? alias : name;
            codes.Add(I386.Jmp(m.GetFunction(CallType, module, n).Address));
        }
    }
}
