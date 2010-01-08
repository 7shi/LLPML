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

            public TypeBase Type
            {
                get
                {
                    var f = GetFunction();
                    if (f != null) return f.Type;

                    var g = GetGetter();
                    if (g != null) return g.ReturnType;

                    throw Abort("can not find: {0}", name);
                }
            }

            public void AddCodes(OpModule codes, string op, Addr32 dest)
            {
                Val32 v;
                var m = codes.Module;
                if (address != null)
                    v = new Val32(m.Specific.ImageBase, address);
                else if (func != null)
                    v = func.GetAddress(m);
                else
                {
                    var f = GetFunction();
                    if (f == null)
                    {
                        var g = GetGetter();
                        if (g != null)
                        {
                            new Call(Parent, g.Name).AddCodes(codes, op, dest);
                            return;
                        }
                        throw Abort("undefined function: " + name);
                    }
                    v = f.GetAddress(m);
                }
                codes.AddCodes(op, dest, v);
            }

            public Function GetFunction()
            {
                if (func != null) return func;
                return Parent.GetFunction(name);
            }

            public Function GetGetter()
            {
                if (GetFunction() != null) return null;
                return Parent.GetFunction("get_" + name);
            }

            public Function GetSetter()
            {
                if (GetFunction() != null) return null;
                return Parent.GetFunction("set_" + name);
            }

            public bool IsGetter
            {
                get { return GetGetter() != null; }
            }

            public bool IsSetter
            {
                get { return GetSetter() != null; }
            }
        }
    }
}
