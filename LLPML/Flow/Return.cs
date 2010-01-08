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
        public IIntValue Value
        {
            get { return value; }
            set
            {
                this.value = value;
                BlockBase f = parent.GetFunction();
                Var.Declare retval = f.GetVar("__retval");
                if (retval == null) new Var.Declare(f, "__retval");
            }
        }

        public Return(BlockBase parent) : base(parent) { }
        public Return(BlockBase parent, IIntValue value) : this(parent) { Value = value; }
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
                    Value = v[0];
                }
            });

            base.Read(xr);
        }

        public override void AddCodes(OpCodes codes)
        {
            if (value != null)
            {
                value.AddCodes(codes, "mov", null);
                Var retval = new Var(parent, "__retval");
                codes.Add(I386.Mov(retval.GetAddress(codes), Reg32.EAX));
            }
            BlockBase f = parent.GetFunction();
            BlockBase b = parent;
            Pointer.Declare[] ptrs = UsingPointers;
            for (; ; ptrs = b.UsingPointers, b = b.Parent)
            {
                b.AddDestructors(codes, ptrs);
                if (b == f) break;
                b.AddExitCodes(codes);
            }
            if (!IsLast) codes.Add(I386.Jmp(b.Destruct));
        }
    }
}
