using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeInt : TypeIntBase
    {
        // type name
        public override string Name { get { return "int"; } }

        // singleton
        private static TypeInt instance = new TypeInt();
        public static TypeInt Instance { get { return instance; } }

        // cast
        public override TypeBase Cast(TypeBase type)
        {
            if (type is TypeInt || type is TypeUShort || type is TypeByte)
                return this;
            return null;
        }

        protected TypeInt()
        {
            conds["greater"] = CondPair.New(Cc.G, Cc.NG);
            conds["greater-equal"] = CondPair.New(Cc.GE, Cc.NGE);
            conds["less"] = CondPair.New(Cc.L, Cc.NL);
            conds["less-equal"] = CondPair.New(Cc.LE, Cc.NLE);
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
                    Shift("sal", codes, dest);
                    break;
                case "shift-right":
                    Shift("sar", codes, dest);
                    break;
                case "mul":
                    codes.Add(I386.ImulA(dest));
                    codes.Add(I386.MovAR(dest, Reg32.EAX));
                    break;
                case "div":
                    codes.Add(I386.XchgRA(Reg32.EAX, dest));
                    codes.Add(I386.Cdq());
                    codes.Add(I386.IdivA(dest));
                    codes.Add(I386.MovAR(dest, Reg32.EAX));
                    break;
                case "mod":
                    codes.Add(I386.XchgRA(Reg32.EAX, dest));
                    codes.Add(I386.Cdq());
                    codes.Add(I386.IdivA(dest));
                    codes.Add(I386.MovAR(dest, Reg32.EDX));
                    break;
                default:
                    base.AddOpCodes(op, codes, dest);
                    break;
            }
        }
    }

    public class TypeVar : TypeInt
    {
        // type name
        public override string Name { get { return "var"; } }

        // singleton
        private static TypeVar instance = new TypeVar();
        public static new TypeVar Instance { get { return instance; } }
        protected TypeVar() { }

        // cast
        public override TypeBase Cast(TypeBase type)
        {
            return this;
        }
    }

    public class TypeBool : TypeInt
    {
        // type name
        public override string Name { get { return "bool"; } }

        // singleton
        private static TypeBool instance = new TypeBool();
        public static new TypeBool Instance { get { return instance; } }
        protected TypeBool() { }
    }

    public class TypeShort : TypeInt
    {
        // type name
        public override string Name { get { return "short"; } }

        // type size
        public override int Size { get { return 2; } }

        // singleton
        private static TypeShort instance = new TypeShort();
        public static new TypeShort Instance { get { return instance; } }
        protected TypeShort() { }

        // get value
        public override void AddGetCodes(OpModule codes, string op, Addr32 dest, Addr32 src)
        {
            codes.AddCodesSWA(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpModule codes, Addr32 dest)
        {
            codes.Add(I386.MovWAR(dest, Reg16.AX));
        }

        // cast
        public override TypeBase Cast(TypeBase type)
        {
            if (type is TypeShort || type is TypeSByte || type is TypeByte)
                return this;
            else if (type is TypeInt)
                return TypeInt.Instance;
            else if (type is TypeUInt)
                return TypeUInt.Instance;
            return null;
        }
    }

    public class TypeSByte : TypeInt
    {
        // type name
        public override string Name { get { return "sbyte"; } }

        // type size
        public override int Size { get { return 1; } }

        // singleton
        private static TypeSByte instance = new TypeSByte();
        public static new TypeSByte Instance { get { return instance; } }
        protected TypeSByte() { }

        // get value
        public override void AddGetCodes(OpModule codes, string op, Addr32 dest, Addr32 src)
        {
            codes.AddCodesSBA(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpModule codes, Addr32 dest)
        {
            codes.Add(I386.MovBAR(dest, Reg8.AL));
        }

        // cast
        public override TypeBase Cast(TypeBase type)
        {
            if (type is TypeByte)
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
