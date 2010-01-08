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

        protected TypeUInt()
        {
            funcs["shift-left" ] = (codes, dest, arg) => Shift("shl", codes, dest, arg);
            funcs["shift-right"] = (codes, dest, arg) => Shift("shr", codes, dest, arg);

            funcs["mul"] = (codes, dest, arg) =>
            {
                arg.AddCodes(codes, "mov", null);
                codes.AddRange(new OpCode[]
                {
                    I386.Mul(dest),
                    I386.Mov(dest, Reg32.EAX)
                });
            };
            funcs["div"] = (codes, dest, arg) =>
            {
                arg.AddCodes(codes, "mov", null);
                codes.AddRange(new OpCode[]
                {
                    I386.Xchg(Reg32.EAX, dest),
                    I386.Xor(Reg32.EDX, Reg32.EDX),
                    I386.Div(dest),
                    I386.Mov(dest, Reg32.EAX)
                });
            };
            funcs["mod"] = (codes, dest, arg) =>
            {
                arg.AddCodes(codes, "mov", null);
                codes.AddRange(new OpCode[]
                {
                    I386.Xchg(Reg32.EAX, dest),
                    I386.Xor(Reg32.EDX, Reg32.EDX),
                    I386.Div(dest),
                    I386.Mov(dest, Reg32.EDX)
                });
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
        public override void AddGetCodes(OpCodes codes, string op, Addr32 dest, Addr32 src)
        {
            codes.AddCodesUW(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpCodes codes, Addr32 ad)
        {
            codes.Add(I386.MovW(ad, Reg16.AX));
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
        public override void AddGetCodes(OpCodes codes, string op, Addr32 dest, Addr32 src)
        {
            codes.AddCodesUB(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpCodes codes, Addr32 ad)
        {
            codes.Add(I386.MovB(ad, Reg8.AL));
        }
    }
}
