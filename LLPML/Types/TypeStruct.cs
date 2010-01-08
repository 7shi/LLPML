using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeStruct : TypeBase
    {
        // type name
        protected string name;
        public override string Name { get { return name; } }

        // type size
        public override int Size { get { return GetStruct().GetSize(); } }

        // check value
        public override bool IsValue { get { return false; } }

        // functions
        public override Func GetFunc(string key)
        {
            return base.GetFunc(key);

            /// todo: operator overload
            //var f = Type.GetFunction("operator_" + key);
            //return null;
        }

        // conditions
        public override CondPair GetCond(string key)
        {
            /// todo: operator overload
            return base.GetCond(key);
        }

        // cast
        public override TypeBase Cast(TypeBase type)
        {
            if (type is TypeStruct && (type as TypeStruct).name == name)
                return this;
            return null;
        }

        public BlockBase Parent { get; protected set; }

        public Struct.Define GetStruct()
        {
            return Parent.GetStruct(name);
        }

        public TypeStruct(BlockBase parent, string name)
        {
            if (name.EndsWith("]"))
                throw new Exception("???");
            Parent = parent;
            this.name = name;
            //TypeIntBase.AddComparers(funcs, conds);
        }
    }
}
