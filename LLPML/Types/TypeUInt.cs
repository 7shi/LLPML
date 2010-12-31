using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeUInt : TypeIntBase
    {
        // type name
        public override string Name { get { return "uint"; } }

        // singleton
        private static TypeUInt instance = new TypeUInt();
        public static TypeUInt Instance { get { return instance; } }

        // cast
        public override TypeBase Cast(TypeBase type)
        {
            if (type is TypeUInt || type is TypeShort || type is TypeSByte)
                return this;
            return null;
        }

        protected TypeUInt()
        {
            AddOperators(funcs);
            AddComparers(funcs, conds);

            funcs["shift-left"] = (codes, dest) => Shift("shl", codes, dest);
            funcs["shift-right"] = (codes, dest) => Shift("shr", codes, dest);

            funcs["mul"] = (codes, dest) =>
            {
                codes.Add(I386.Mul(dest));
                codes.Add(I386.Mov(dest, Reg32.EAX));
            };
            funcs["div"] = (codes, dest) =>
            {
                codes.Add(I386.Xchg(Reg32.EAX, dest));
                codes.Add(I386.Xor(Reg32.EDX, Reg32.EDX));
                codes.Add(I386.Div(dest));
                codes.Add(I386.Mov(dest, Reg32.EAX));
            };
            funcs["mod"] = (codes, dest) =>
            {
                codes.Add(I386.Xchg(Reg32.EAX, dest));
                codes.Add(I386.Xor(Reg32.EDX, Reg32.EDX));
                codes.Add(I386.Div(dest));
                codes.Add(I386.Mov(dest, Reg32.EDX));
            };

            AddComparers(conds);
        }

        public static void AddComparers(Dictionary<string, CondPair> conds)
        {
            conds["greater"] = new CondPair(Cc.A, Cc.NA);
            conds["greater-equal"] = new CondPair(Cc.AE, Cc.NAE);
            conds["less"] = new CondPair(Cc.B, Cc.NB);
            conds["less-equal"] = new CondPair(Cc.BE, Cc.NBE);
        }
    }

    public class TypeUShort : TypeUInt
    {
        // type name
        public override string Name { get { return "ushort"; } }

        // type size
        public override int Size { get { return 2; } }

        // singleton
        private static TypeUShort instance = new TypeUShort();
        public static new TypeUShort Instance { get { return instance; } }
        protected TypeUShort() { }

        // get value
        public override void AddGetCodes(OpModule codes, string op, Addr32 dest, Addr32 src)
        {
            codes.AddCodesUW(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpModule codes, Addr32 dest)
        {
            codes.Add(I386.MovW(dest, Reg16.AX));
        }

        // cast
        public override TypeBase Cast(TypeBase type)
        {
            if (type is TypeUShort || type is TypeSByte || type is TypeByte)
                return this;
            else if (type is TypeInt)
                return TypeInt.Instance;
            else if (type is TypeUInt)
                return TypeUInt.Instance;
            return null;
        }
    }

    public class TypeChar : TypeUShort
    {
        // type name
        public override string Name { get { return "char"; } }

        // singleton
        private static TypeChar instance = new TypeChar();
        public static new TypeChar Instance { get { return instance; } }
        protected TypeChar() { }
    }

    public class TypeByte : TypeUInt
    {
        // type name
        public override string Name { get { return "byte"; } }

        // type size
        public override int Size { get { return 1; } }

        // singleton
        private static TypeByte instance = new TypeByte();
        public static new TypeByte Instance { get { return instance; } }
        protected TypeByte() { }

        // get value
        public override void AddGetCodes(OpModule codes, string op, Addr32 dest, Addr32 src)
        {
            codes.AddCodesUB(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpModule codes, Addr32 dest)
        {
            codes.Add(I386.MovB(dest, Reg8.AL));
        }

        // cast
        public override TypeBase Cast(TypeBase type)
        {
            if (type is TypeSByte)
                return this;
            else if (type is TypeInt)
                return TypeInt.Instance;
            else if (type is TypeUInt)
                return TypeUInt.Instance;
            else if (type is TypeShort)
                return TypeShort.Instance;
            else if (type is TypeUShort)
                return TypeUShort.Instance;
            return null;
        }
    }
}
