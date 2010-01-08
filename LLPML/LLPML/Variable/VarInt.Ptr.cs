using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class VarInt : VarBase
    {
        public class Ptr : NodeBase, IIntValue
        {
            private VarInt src;

            public Ptr() { }

            public Ptr(Block parent, string name)
                : base(parent)
            {
                src = new VarInt(parent, name);
            }

            public Ptr(Block parent, XmlTextReader xr)
                : base(parent, xr)
            {
            }

            public override void Read(XmlTextReader xr)
            {
                src = new VarInt(parent, xr);
            }

            public Addr32 GetAddress(List<OpCode> codes, Module m)
            {
                return src.GetAddress(codes, m);
            }

            void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
            {
                codes.Add(I386.Lea(Reg32.EAX, GetAddress(codes, m)));
                IntValue.AddCodes(codes, op, dest);
            }
        }
    }
}
