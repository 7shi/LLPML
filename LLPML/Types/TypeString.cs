using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeString : TypeReference
    {
        public const string Equal = "__string_equal";
        public const string Add = "__string_add";
        public const string Sub = "__string_sub";
        public const string Mul = "__string_mul";

        // singleton
        private static TypeString instance = new TypeString();
        public static TypeString Instance { get { return instance; } }

        protected TypeString()
            : base(null, false)
        {
            funcs["equal"] = funcs["not-equal"] = (codes, dest) =>
            {
                codes.Add(I386.PushA(dest));
                codes.Add(I386.XchgRA(Reg32.EAX, Addr32.New(Reg32.ESP)));
                codes.Add(I386.Push(Reg32.EAX));
                codes.Add(codes.GetCall("string", Equal));
                codes.Add(I386.AddR(Reg32.ESP, Val32.New(8)));
                codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
            };
            conds["equal"] = CondPair.New(Cc.NZ, Cc.Z);
            conds["not-equal"] = CondPair.New(Cc.Z, Cc.NZ);

            funcs["add"] = (codes, dest) => AddFunc(codes, dest, Add);
            funcs["add-char"] = (codes, dest) => AddFunc(codes, dest, Add + "_char");
            funcs["add-int"] = (codes, dest) => AddFunc(codes, dest, Add + "_int");
            funcs["sub"] = (codes, dest) => AddFunc(codes, dest, Sub);
            funcs["sub-char"] = (codes, dest) => AddFunc(codes, dest, Sub + "_char");
            funcs["sub-int"] = (codes, dest) => AddFunc(codes, dest, Sub + "_int");
            funcs["mul"] = (codes, dest) => AddFunc(codes, dest, Add);
            funcs["mul-int"] = (codes, dest) => AddFunc(codes, dest, Mul + "_int");
        }

        private void AddFunc(OpModule codes, Addr32 dest, string func)
        {
            codes.Add(I386.PushA(dest));
            codes.Add(I386.XchgRA(Reg32.EAX, Addr32.New(Reg32.ESP)));
            codes.Add(I386.Push(Reg32.EAX));
            codes.Add(codes.GetCall("string", func));
            codes.Add(I386.AddR(Reg32.ESP, Val32.New(8)));
            codes.Add(I386.XchgRA(Reg32.EAX, dest));
            codes.Add(I386.Push(Reg32.EAX));
            codes.AddDtorCodes(TypeString.Instance);
        }

        // cast
        public override TypeBase Cast(TypeBase type)
        {
            if (type is TypePointer && type.Type is TypeChar)
                return type;
            return base.Cast(type);
        }

        // recursive type
        private static TypeBase type;
        public static void Init() { type = null; }
        public override TypeBase Type
        {
            get
            {
                if (type != null) return type;
                return type = Types.GetType("string");
            }
        }

        public override bool UseGC { get { return true; } }
    }

    public class TypeConstString : TypeString
    {
        // type name
        public override string Name
        {
            get { return "var:(const string)"; }
        }

        // singleton
        private static TypeConstString instance = new TypeConstString();
        public static new TypeConstString Instance { get { return instance; } }
        private TypeConstString() : base() { }
    }

    public class TypeConstChar : TypeChar
    {
        // type name
        public override string Name { get { return "(const char)"; } }

        // singleton
        private static TypeConstChar instance = new TypeConstChar();
        public static new TypeConstChar Instance { get { return instance; } }
        protected TypeConstChar() { }
    }
}
