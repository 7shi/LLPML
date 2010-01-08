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
        public const string Delete = "__operator_delete";
        public const string Dereference = "__dereference";

        // type name
        public override string Name
        {
            get
            {
                var t = "var:" + Type.Name;
                if (IsArray) return t + "[]";
                return t;
            }
        }

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
            if (type is TypeVar)
                return type;
            else if (type is TypeReference)
                return Type.Cast(type.Type);
            else
                return null;
        }

        // set value
        public override void AddSetCodes(OpCodes codes, Addr32 ad)
        {
            if (UseGC)
            {
                var flag = !ad.IsAddress && ad.Register == Var.DestRegister;
                if (flag) codes.Add(I386.Push(ad.Register));
                var label = new OpCode();
                codes.AddRange(new[]
                {
                    I386.Push(Reg32.EAX),
                    I386.Push(ad),
                    GetCall("var", Dereference),
                    I386.Add(Reg32.ESP, 4),
                    I386.Pop(Reg32.EAX),
                    I386.Test(Reg32.EAX, Reg32.EAX),
                    I386.Jcc(Cc.Z, label.Address),
                    I386.Inc(new Addr32(Reg32.EAX, -12)),
                    label,
                });
                if (flag) codes.Add(I386.Pop(ad.Register));
            }
            base.AddSetCodes(codes, ad);
        }

        // recursive type
        private bool doneInferType = false;
        public override TypeBase Type
        {
            get
            {
                if (doneInferType) return base.Type;

                doneInferType = true;
                if (IsArray)
                {
                    var ts = base.Type as TypeStruct;
                    if (ts != null && ts.IsClass)
                        Type = new TypeReference(ts.Parent, ts);
                }
                return base.Type;
            }
        }

        // type constructor
        public override bool NeedsCtor { get { return UseGC; } }
        public override void AddConstructor(OpCodes codes)
        {
            if (!NeedsCtor) return;
            codes.AddRange(new[]
            {
                I386.Mov(Reg32.EAX, new Addr32(Reg32.ESP)),
                I386.Mov(new Addr32(Reg32.EAX), (Val32)0),
            });
        }

        // type destructor
        public override bool NeedsDtor { get { return UseGC; } }
        public override void AddDestructor(OpCodes codes)
        {
            if (!NeedsDtor) return;
            codes.AddRange(new[]
            {
                I386.Mov(Reg32.EAX, new Addr32(Reg32.ESP)),
                I386.Push(new Addr32(Reg32.EAX)),
                GetCall("var", Dereference),
                I386.Add(Reg32.ESP, 4),
            });
        }

        public bool UseGC
        {
            get
            {
                if (IsArray) return true;
                var t = Type as TypeStruct;
                return t != null && t.IsClass;
            }
        }

        private bool isArray = false;
        public override bool IsArray { get { return isArray; } }

        public TypeReference(BlockBase parent, TypeBase type)
            : this(parent, type, false)
        {
        }

        public TypeReference(BlockBase parent, TypeBase type, bool isArray)
        {
            Parent = parent;
            Type = type;
            this.isArray = isArray;
        }
    }
}
