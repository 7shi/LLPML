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
        public override bool CheckFunc(string op)
        {
            return base.CheckFunc(op) || Type.CheckFunc(op);
        }
        public override void AddOpCodes(string op, OpModule codes, Addr32 dest)
        {
            if (base.CheckFunc(op))
                base.AddOpCodes(op, codes, dest);
            else
                Type.AddOpCodes(op, codes, dest);
        }

        // conditions
        public override CondPair GetCond(string op)
        {
            var ret = base.GetCond(op);
            if (ret == null) ret = Type.GetCond(op);
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
                codes.Add(I386.Push(Reg32.EAX));
                codes.Add(I386.MovRA(Reg32.EAX, ad));
                AddDereferenceCodes(codes);
                codes.Add(I386.MovRA(Reg32.EAX, Addr32.New(Reg32.ESP)));
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
            codes.Add(I386.MovRA(Reg32.EAX, Addr32.New(Reg32.ESP)));
            codes.Add(I386.MovA(Addr32.New(Reg32.EAX), Val32.New(0)));
        }

        // type destructor
        public override bool NeedsDtor { get { return UseGC; } }
        public override void AddDestructor(OpModule codes)
        {
            if (!NeedsDtor) return;
            codes.Add(I386.MovRA(Reg32.EAX, Addr32.New(Reg32.ESP)));
            codes.Add(I386.MovRA(Reg32.EAX, Addr32.New(Reg32.EAX)));
            AddDereferenceCodes(codes);
        }

        public static void AddReferenceCodes(OpModule codes)
        {
            codes.Add(I386.Push(Reg32.EAX));
            codes.Add(codes.GetCall("var", Reference));
            codes.Add(I386.Pop(Reg32.EAX));
        }

        public static void AddDereferenceCodes(OpModule codes)
        {
            codes.Add(I386.Push(Reg32.EAX));
            codes.Add(codes.GetCall("var", Dereference));
            codes.Add(I386.Pop(Reg32.EAX));
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

        public static TypeReference New(TypeBase type, bool isArray)
        {
            var ret = new TypeReference();
            ret.Type = type;
            ret.isArray = isArray;
            return ret;
        }
    }
}
