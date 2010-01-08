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
                parent.GetFunction().AddTypeInfo(value);
            }
        }

        public Return(BlockBase parent) : base(parent) { }
        public Return(BlockBase parent, IIntValue value) : this(parent) { Value = value; }
        public Return(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                var v = IntValue.Read(parent, xr);
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
            ///if (castFailed != null) throw Abort(castFailed);
            var f = parent.GetFunction();
            if (value != null)
            {
                value.AddCodes(codes, "mov", null);
                var retval = f.GetRetVal(parent);
                var dest = retval.GetAddress(codes);
                var rt = f.ReturnType as TypeReference;
                if (rt != null)
                {
                    codes.Add(I386.Mov(dest, (Val32)0));
                    rt.AddSetCodes(codes, dest);
                }
                else
                    codes.Add(I386.Mov(dest, Reg32.EAX));
            }
            var b = parent;
            var ptrs = UsingPointers;
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
