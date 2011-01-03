using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.LLPML.Struct;
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

        // conditions
        public override CondPair GetCond(string op)
        {
            /// todo: operator overload
            return base.GetCond(op);
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
            var f1 = st.GetFunction(Define.Initializer);
            var f2 = st.GetFunction(Define.Constructor);
            codes.Add(I386.CallD(f1.First));
            codes.Add(I386.CallD(f2.First));
        }

        // type destructor
        public override bool NeedsDtor { get { return GetStruct().NeedsDtor; } }
        public override void AddDestructor(OpModule codes)
        {
            var dtor = GetStruct().GetFunction(Define.Destructor);
            codes.Add(I386.CallD(dtor.First));
        }

        // type check
        public override bool Check()
        {
            var ret = Parent.GetStruct(name);
            return ret != null;
        }

        public Define GetStruct()
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

        public static TypeStruct New(BlockBase parent, string name)
        {
            if (name.EndsWith("]"))
                throw new Exception("TypeStruct: invalid type: " + name);
            var ret = new TypeStruct();
            ret.Parent = parent;
            ret.name = name;
            return ret;
        }
    }
}
