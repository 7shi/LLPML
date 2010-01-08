#define INLINE_REFCOUNT

using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeReference : TypeVarBase
    {
        public const string Delete = "__operator_delete";
        public const string Reference = "__reference";
        public const string Dereference = "__dereference";

        // type name
        public override string Name
        {
            get
            {
                if (IsArray)
                    return Type.Name + "[]";
                else
                    return "var:" + Type.Name;
            }
        }

        // functions
        public override Func GetFunc(string key)
        {
            var ret = base.GetFunc(key);
            if (ret == null) ret = Type.GetFunc(key);
            return ret;
        }

        // conditions
        public override CondPair GetCond(string key)
        {
            var ret = base.GetCond(key);
            if (ret == null) ret = Type.GetCond(key);
            return ret;
        }

        // cast
        public override TypeBase Cast(TypeBase type)
        {
            if (IsArray && type.Name == "var:object")
                return type;
            else if (type is TypeVar)
                return type;
            else if (type is TypeString && IsArray && Type is TypeChar)
                return type;
            else if (type is TypePointer && IsArray)
            {
                if (Type is TypeIntBase)
                {
                    if (Type == type.Type) return type;
                }
                else if (Type.Cast(type.Type) != null)
                    return type;
            }
            if (!(type is TypeReference))
                return null;
            else if (Type is TypeReference && type.Type is TypeReference)
            {
                var c = Type.Cast(type.Type);
                if (c == null) return null;
                return Types.ToVarType(c);
            }
            else
                return base.Cast(type);
        }

        // set value
        public override void AddSetCodes(OpModule codes, Addr32 ad)
        {
            if (UseGC)
            {
                var flag = !ad.IsAddress && ad.Register == Var.DestRegister;
                if (flag) codes.Add(I386.Push(ad.Register));
                codes.AddRange(new[]
                {
                    I386.Push(Reg32.EAX),
                    I386.Mov(Reg32.EAX, ad),
                });
                AddDereferenceCodes(codes);
                codes.Add(I386.Mov(Reg32.EAX, new Addr32(Reg32.ESP)));
                AddReferenceCodes(codes);
                codes.Add(I386.Pop(Reg32.EAX));
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
                        Type = Types.ToVarType(ts);
                }
                return base.Type;
            }
        }

        // type constructor
        public override bool NeedsCtor { get { return UseGC; } }
        public override void AddConstructor(OpModule codes)
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
        public override void AddDestructor(OpModule codes)
        {
            if (!NeedsDtor) return;
            codes.AddRange(new[]
            {
                I386.Mov(Reg32.EAX, new Addr32(Reg32.ESP)),
                I386.Mov(Reg32.EAX, new Addr32(Reg32.EAX)),
            });
            AddDereferenceCodes(codes);
        }

        public static void AddReferenceCodes(OpModule codes)
        {
#if INLINE_REFCOUNT
            var label = new OpCode();
            codes.AddRange(new[]
            {
                I386.Test(Reg32.EAX, Reg32.EAX),
                I386.Jcc(Cc.Z, label.Address),
                I386.Inc(new Addr32(Reg32.EAX, -12)),
                label,
            });
#else
            codes.AddRange(new[]
            {
                I386.Push(Reg32.EAX),
                GetCall("var", Reference),
                I386.Pop(Reg32.EAX),
            });
#endif
        }

        public static void AddDereferenceCodes(OpModule codes)
        {
#if INLINE_REFCOUNT
            var label = new OpCode();
            codes.AddRange(new[]
            {
                I386.Test(Reg32.EAX, Reg32.EAX),
                I386.Jcc(Cc.Z, label.Address),
                I386.Dec(new Addr32(Reg32.EAX, -12)),
                I386.Jcc(Cc.NZ, label.Address),
                I386.Push(Reg32.EAX),
                codes.GetCall("var", Delete),
                I386.Add(Reg32.ESP, 4),
                label,
            });
#else
            codes.AddRange(new[]
            {
                I386.Push(Reg32.EAX),
                GetCall("var", Dereference),
                I386.Pop(Reg32.EAX),
            });
#endif
        }

        public virtual bool UseGC
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

        public TypeReference(TypeBase type)
            : this(type, false)
        {
        }

        public TypeReference(TypeBase type, bool isArray)
        {
            TypeIntBase.AddComparers(funcs, conds);
            Type = type;
            this.isArray = isArray;
        }
    }
}
