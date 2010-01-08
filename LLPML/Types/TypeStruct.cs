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
        public override string Name { get { return typeName; } }

        // functions
        //public override Func GetFunc(string key)
        //{
        //    var f = type.GetFunction("operator_" + key);
        //    if (f == null) return base.GetFunc(key);
        //    return null;
        //}

        // conditions
        public override CondPair GetCond(string key)
        {
            return base.GetCond(key);
        }

        private string typeName;
        //public override int Size { get { return type.GetSize(); } }

        public TypeStruct(string typeName)
        {
            this.typeName = typeName;
            //TypeIntBase.AddComparers(funcs, conds);
        }

        // get value
        public override void AddGetCodes(OpCodes codes, string op, Addr32 dest, Addr32 src)
        {
            codes.Add(I386.Lea(Reg32.EAX, src));
            codes.AddCodes(op, dest);
        }
    }
}
