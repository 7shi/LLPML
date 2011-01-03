using System;
using System.Collections.Generic;
using System.Text;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Extern : Function
    {
        private string module, alias;

        public static Extern New(BlockBase parent, string name, string module, string alias)
        {
            var ret = new Extern();
            ret.init2(parent, name, false);
            ret.module = module;
            ret.alias = alias;
            return ret;
        }

        public override void AddCodes(OpModule codes)
        {
            codes.Add(first);
            if (alias != null)
                codes.Add(I386.Jmp(codes.Module.GetFunction(module, alias)));
            else
                codes.Add(I386.Jmp(codes.Module.GetFunction(module, name)));
        }
    }
}
