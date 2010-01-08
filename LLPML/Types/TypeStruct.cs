using System;
using System.Collections.Generic;
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
            if (type is TypeStruct)
            {
                if (GetStruct().CanUpCast((type as TypeStruct).GetStruct()))
                    return type;
                return null;
            }
            else if (type is TypeReference)
            {
                if (Cast(type.Type) != null) return type;
                return null;
            }
            else if (type is TypeVar)
                return type;
            else
                return null;
        }

        // type constructor
        public override bool NeedsCtor
        {
            get
            {
                var st = GetStruct();
                return st.NeedsInit || st.NeedsCtor;
            }
        }
        public override void AddConstructor(OpModule codes)
        {
            var st = GetStruct();
            var f1 = st.GetFunction(Struct.Define.Initializer);
            var f2 = st.GetFunction(Struct.Define.Constructor);
            codes.AddRange(new[]
            {
                I386.Call(f1.First),
                I386.Call(f2.First),
            });
        }

        // type destructor
        public override bool NeedsDtor { get { return GetStruct().NeedsDtor; } }
        public override void AddDestructor(OpModule codes)
        {
            var dtor = GetStruct().GetFunction(Struct.Define.Destructor);
            codes.Add(I386.Call(dtor.First));
        }

        public Struct.Define GetStruct()
        {
            var ret = Parent.GetStruct(name);
            if (ret != null) return ret;
            throw Parent.Abort("can not find struct: {0}", name);
        }

        public bool IsClass
        {
            get
            {
                var st = GetStruct();
                if (st != null) return st.IsClass;
                return false;
            }
        }

        public TypeStruct(BlockBase parent, string name)
        {
            if (name.EndsWith("]"))
                throw new Exception("TypeStruct: invalid type: " + name);
            Parent = parent;
            this.name = name;
            //TypeIntBase.AddComparers(funcs, conds);
        }
    }
}
