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

        // singleton
        private static TypeString instance = new TypeString();
        public static TypeString Instance { get { return instance; } }

        protected TypeString()
            : base(null, false)
        {
            funcs["equal"] = funcs["not-equal"] = (codes, dest) =>
            {
                codes.AddRange(new[]
                {
                    I386.Push(dest),
                    I386.Xchg(Reg32.EAX, new Addr32(Reg32.ESP)),
                    I386.Push(Reg32.EAX),
                    codes.GetCall("string", Equal),
                    I386.Add(Reg32.ESP, 8),
                    I386.Test(Reg32.EAX, Reg32.EAX),
                });
            };
            conds["equal"] = new CondPair(Cc.NZ, Cc.Z);
            conds["not-equal"] = new CondPair(Cc.Z, Cc.NZ);

            funcs["add"] = (codes, dest) =>
            {
                codes.AddRange(new[]
                {
                    I386.Push(dest),
                    I386.Xchg(Reg32.EAX, new Addr32(Reg32.ESP)),
                    I386.Push(Reg32.EAX),
                    codes.GetCall("string", Add),
                    I386.Add(Reg32.ESP, 8),
                    I386.Xchg(Reg32.EAX, dest),
                    I386.Push(Reg32.EAX),
                });
                codes.AddDtorCodes(TypeString.Instance);
            };
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
