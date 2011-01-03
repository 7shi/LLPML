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
            conds["greater"] = CondPair.New(Cc.A, Cc.NA);
            conds["greater-equal"] = CondPair.New(Cc.AE, Cc.NAE);
            conds["less"] = CondPair.New(Cc.B, Cc.NB);
            conds["less-equal"] = CondPair.New(Cc.BE, Cc.NBE);
        }

        public override bool CheckFunc(string op)
        {
            switch (op)
            {
                case "shift-left":
                case "shift-right":
                case "mul":
                case "div":
                case "mod":
                    return true;
                default:
                    return base.CheckFunc(op);
            }
        }

        public override void AddOpCodes(string op, OpModule codes, Addr32 dest)
        {
            switch (op)
            {
                case "shift-left":
                    Shift("shl", codes, dest);
                    break;
                case "shift-right":
                    Shift("shr", codes, dest);
                    break;
                case "mul":
                    codes.Add(I386.MulA(dest));
                    codes.Add(I386.MovAR(dest, Reg32.EAX));
                    break;
                case "div":
                    codes.Add(I386.XchgRA(Reg32.EAX, dest));
                    codes.Add(I386.Xor(Reg32.EDX, Reg32.EDX));
                    codes.Add(I386.DivA(dest));
                    codes.Add(I386.MovAR(dest, Reg32.EAX));
                    break;
                case "mod":
                    codes.Add(I386.XchgRA(Reg32.EAX, dest));
                    codes.Add(I386.Xor(Reg32.EDX, Reg32.EDX));
                    codes.Add(I386.DivA(dest));
                    codes.Add(I386.MovAR(dest, Reg32.EDX));
                    break;
                default:
                    base.AddOpCodes(op, codes, dest);
                    break;
            }
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
            codes.AddCodesUWA(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpModule codes, Addr32 dest)
        {
            codes.Add(I386.MovWAR(dest, Reg16.AX));
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
            codes.AddCodesUBA(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpModule codes, Addr32 dest)
        {
            codes.Add(I386.MovBAR(dest, Reg8.AL));
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
