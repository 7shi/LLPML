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
        public override bool NeedsCtor { get { return GetStruct().NeedsCtor; } }
        public override void AddConstructor(OpCodes codes, Addr32 ad)
        {
            if (ad != null)
                codes.Add(I386.Lea(Reg32.EAX, ad));
            var st = GetStruct();
            codes.Add(I386.Push(Reg32.EAX));
            st.AddInit(codes, null);
            codes.Add(I386.Pop(Reg32.EAX));
            st.AddConstructor(codes, null);
        }

        // type destructor
        public override bool NeedsDtor { get { return GetStruct().NeedsDtor; } }
        public override void AddDestructor(OpCodes codes, Addr32 ad)
        {
            if (ad != null)
                codes.AddRange(new[]
                {
                    I386.Lea(Reg32.EAX, ad),
                    I386.Push(Reg32.EAX),
                });
            else
                codes.Add(I386.Push(Reg32.EAX));
            var dtor = GetStruct().GetFunction(Struct.Define.Destructor);
            codes.Add(I386.Call(dtor.First));
            if (dtor.CallType == CallType.CDecl)
                codes.Add(I386.Add(Reg32.ESP, 4));
        }

        public BlockBase Parent { get; protected set; }

        public Struct.Define GetStruct()
        {
            return Parent.GetStruct(name);
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
