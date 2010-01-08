using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Pointer : VarBase, IIntValue
    {
        public Declare Reference { get; private set; }

        private int length = 0;
        public int Length { get { return length; } }

        public Pointer() { }

        public Pointer(BlockBase parent, Declare ptr)
            : base(parent, ptr.Name)
        {
            Reference = ptr;
        }

        public Pointer(BlockBase parent, string name)
            : base(parent, name)
        {
            Reference = parent.GetPointer(name);
            if (Reference == null)
                throw Abort("undefined pointer: " + name);
        }

        public Pointer(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequiresName(xr);

            Reference = parent.GetPointer(name);
            if (Reference == null)
                throw Abort(xr, "undefined pointer: " + name);
        }

        public virtual string Type
        {
            get
            {
                Struct.Declare st = Reference as Struct.Declare;
                return st == null ? null : st.Type;
            }
        }

        public virtual void GetValue(List<OpCode> codes, Module m)
        {
            codes.Add(I386.Lea(Reg32.EAX, Reference.GetAddress(codes, m, parent)));
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            GetValue(codes, m);
            IntValue.AddCodes(codes, op, dest);
        }
    }
}
