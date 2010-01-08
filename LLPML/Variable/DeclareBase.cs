using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class DeclareBase : NodeBase
    {
        protected Addr32 address;
        public Addr32 Address
        {
            get { return address; }
            set { address = value; }
        }

        protected Var thisptr;

        public DeclareBase() { }
        public DeclareBase(BlockBase parent, string name) : base(parent, name) { Init(); }
        public DeclareBase(BlockBase parent, XmlTextReader xr) : base(parent, xr) { Init(); }

        protected virtual void Init()
        {
            if (parent is Struct.Define)
                thisptr = new Struct.This(parent);
        }

        public bool HasThis
        {
            get
            {
                return thisptr != null;
            }
        }

        public Addr32 GetAddress(OpCodes codes, BlockBase scope)
        {
            const Reg32 dest = Reg32.EDX;

            if (HasThis)
            {
                codes.Add(I386.Mov(dest, thisptr.GetAddress(codes)));
                return new Addr32(address);
            }

            int plv = scope.Level, lv = parent.Level;
            if (plv == lv || address.IsAddress)
                return new Addr32(address);
            if (lv <= 0 || lv >= plv)
                throw Abort("Invalid variable scope: " + name);
            codes.Add(I386.Mov(dest, new Addr32(Reg32.EBP, -lv * 4)));
            return new Addr32(dest, address.Disp);
        }
    }
}
