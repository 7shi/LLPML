using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypePointer : TypeBase
    {
        // type name
        public override string Name { get { return type.Name; } }

        // functions
        public override Func GetFunc(string key)
        {
            var f = type.GetFunction("operator_" + key);
            if (f == null) return base.GetFunc(key);
            return null;
        }

        // conditions
        public override CondPair GetCond(string key)
        {
            return base.GetCond(key);
        }

        private Struct.Define type;
        public override int Size { get { return type.GetSize(); } }

        public TypePointer(Struct.Define type)
        {
            this.type = type;
            TypeIntBase.AddComparers(funcs, conds);
        }
    }
}
