using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Extern : NodeBase
    {
        public string Name, Module, Alias;
        public CallType Type;

        private Function function = null;

        public Extern() { }
        public Extern(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            Name = xr["name"];
            Module = xr["module"];
            Alias = xr["alias"];
            Type = CallType.CDecl;
            if (xr["type"] == "std") Type = CallType.Std;
            function = null;
            Parse(xr, null);

            string name = Alias == null ? Name : Alias;
            root.SetFunction(name, this);
        }

        public Function GetFunction(Module m)
        {
            if (function == null)
            {
                function = m.GetFunction(Type, Module, Name);
            }
            return function;
        }

        public void Set(Extern src)
        {
            Name = src.Name;
            Module = src.Module;
            Alias = src.Alias;
            Type = src.Type;
            function = src.function;
            base.Set(src);
        }
    }
}
