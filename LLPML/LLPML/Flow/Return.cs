using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Return : BreakBase
    {
        public bool IsLast = false;
        private IIntValue value;

        public Return(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                IIntValue[] v = IntValue.Read(parent, xr);
                if (v != null)
                {
                    if (v.Length > 1 || value != null)
                        throw Abort(xr, "multiple values");
                    value = v[0];
                    BlockBase f = parent.GetFunction();
                    Var.Declare retval = f.GetVar("__retval");
                    if (retval == null) new Var.Declare(f, "__retval");
                }
            });

            base.Read(xr);
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            if (value != null)
            {
                value.AddCodes(codes, m, "mov", null);
                Var retval = new Var(parent, "__retval");
                codes.Add(I386.Mov(retval.GetAddress(codes, m), Reg32.EAX));
            }
            BlockBase f = parent.GetFunction();
            BlockBase b = parent;
            Pointer.Declare[] ptrs = usingPointers;
            for (; ; ptrs = b.UsingPointers, b = b.Parent)
            {
                b.AddDestructors(codes, m, ptrs);
                if (b == f) break;
                b.AddExitCodes(codes, m);
            }
            if (!IsLast) codes.Add(I386.Jmp(b.Destruct));
        }
    }
}
