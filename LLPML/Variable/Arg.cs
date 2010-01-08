using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Arg : Var.Declare
    {
        public Arg(BlockBase parent, string name, TypeBase type) : base(parent, name, type) { }
        public Arg(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }
        protected override void Init() { }

        public override void Read(XmlTextReader xr)
        {
            if (!(Parent is Function))
                throw Abort(xr, "arg must be in function root");
            else if (Parent.ThisStruct != null)
            {
                if (Parent.Name == Struct.Define.Constructor)
                    throw Abort(xr, "constructor can not have any arguments");
                else if (Parent.Name == Struct.Define.Destructor)
                    throw Abort(xr, "destructor can not have any arguments");
            }

            NoChild(xr);
            RequiresName(xr);

            var t = Types.GetType(Parent, xr["type"]);
            if (t != null) type = t;
            AddToParent();
        }
    }

    public class ArgPtr : Arg
    {
        public ArgPtr(BlockBase parent, string name) : base(parent, name, null) { }
        public ArgPtr(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        protected override void Init()
        {
            type = new TypeArray(TypeInt.Instance, 1);
        }
    }
}
