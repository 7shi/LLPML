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

        public Extern()
        {
        }

        public Extern(BlockBase parent, string name, string module, string alias)
            : base(parent, name, false)
        {
            this.module = module;
            this.alias = alias;
        }

        public override void AddCodes(OpModule codes)
        {
            codes.Add(first);
            string n = alias != null ? alias : name;
            codes.Add(I386.Jmp(codes.Module.GetFunction(module, n)));
        }
    }
}
