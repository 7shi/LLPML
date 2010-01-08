using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeReference : TypeVarBase
    {
        // type name
        public override string Name { get { return "var:" + Type.Name; } }

        // functions
        public override Func GetFunc(string key)
        {
            var ret = Type.GetFunc(key);
            if (ret != null) return ret;

            if (key == "equal" || key == "not-equal")
                return TypeInt.Instance.GetFunc(key);
            return null;
        }

        // conditions
        public override CondPair GetCond(string key)
        {
            var ret = Type.GetCond(key);
            if (ret != null) return ret;

            if (key == "equal" || key == "not-equal")
                return TypeInt.Instance.GetCond(key);
            return null;
        }

        // cast
        public override TypeBase Cast(TypeBase type)
        {
            if (type is TypeVar) return type;
            if (!(type is TypeReference)) return null;
            return base.Cast(type);
        }

        public TypeReference(BlockBase parent, TypeBase type)
        {
            Parent = parent;
            Type = type;
        }
    }
}
