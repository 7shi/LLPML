using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Function
    {
        public class Ptr : NodeBase, IIntValue
        {
            private Function func;
            private Val32 address;

            public Ptr(BlockBase parent, string name) : base(parent, name) { }
            public Ptr(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
            public Ptr(Val32 address) { this.address = address; }
            public Ptr(Function func) { this.func = func; }

            public override void Read(XmlTextReader xr)
            {
                NoChild(xr);
                RequiresName(xr);
            }

            public Val32 GetAddress(Module m)
            {
                var f = parent.GetFunction(name);
                if (f == null)
                    throw Abort("undefined function: " + name);
                return f.GetAddress(m);
            }

            public TypeBase Type { get { return null; } }

            public void AddCodes(OpCodes codes, string op, Addr32 dest)
            {
                Val32 v;
                var m = codes.Module;
                if (address != null)
                    v = new Val32(m.Specific.ImageBase, address);
                else if (func != null)
                    v = func.GetAddress(m);
                else
                    v = GetAddress(m);
                codes.AddCodes(op, dest, v);
            }

            public Function GetGetter()
            {
                var f = parent.GetFunction(name);
                if (f != null) return null;
                return parent.GetFunction("get_" + name);
            }

            public Function GetSetter()
            {
                var f = parent.GetFunction(name);
                if (f != null) return null;
                return parent.GetFunction("set_" + name);
            }
        }
    }
}
