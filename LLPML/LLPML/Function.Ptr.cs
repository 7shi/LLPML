using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Function : Block
    {
        public class Ptr : VarBase, IIntValue
        {
            public Ptr() { }
            public Ptr(BlockBase parent, string name) : base(parent, name) { }
            public Ptr(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

            public override void Read(XmlTextReader xr)
            {
                NoChild(xr);
                RequiresName(xr);
            }

            public Val32 GetAddress(Module m)
            {
                Function f = parent.GetFunction(name);
                if (f == null)
                    throw Abort("undefined function: " + name);
                return new Val32(m.Specific.ImageBase, f.First);
            }

            void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
            {
                IntValue.AddCodes(codes, op, dest, GetAddress(m));
            }
        }
    }
}
