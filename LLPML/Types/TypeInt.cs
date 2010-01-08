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

        protected TypeInt()
        {
            AddOperators(funcs);
            AddComparers(funcs, conds);

            funcs["shift-left"] = (codes, dest, arg) => Shift("sal", codes, dest, arg);
            funcs["shift-right"] = (codes, dest, arg) => Shift("sar", codes, dest, arg);

            funcs["mul"] = (codes, dest, arg) =>
            {
                arg.AddCodes(codes, "mov", null);
                codes.AddRange(new OpCode[]
                {
                    I386.Imul(dest),
                    I386.Mov(dest, Reg32.EAX)
                });
            };
            funcs["div"] = (codes, dest, arg) =>
            {
                arg.AddCodes(codes, "mov", null);
                codes.AddRange(new OpCode[]
                {
                    I386.Xchg(Reg32.EAX, dest),
                    I386.Cdq(),
                    I386.Idiv(dest),
                    I386.Mov(dest, Reg32.EAX)
                });
            };
            funcs["mod"] = (codes, dest, arg) =>
            {
                arg.AddCodes(codes, "mov", null);
                codes.AddRange(new OpCode[]
                {
                    I386.Xchg(Reg32.EAX, dest),
                    I386.Cdq(),
                    I386.Idiv(dest),
                    I386.Mov(dest, Reg32.EDX)
                });
            };

            AddComparers(conds);
        }

        public static void AddComparers(Dictionary<string, CondPair> conds)
        {
            conds["greater"] = new CondPair(Cc.G, Cc.NG);
            conds["greater-equal"] = new CondPair(Cc.GE, Cc.NGE);
            conds["less"] = new CondPair(Cc.L, Cc.NL);
            conds["less-equal"] = new CondPair(Cc.LE, Cc.NLE);
        }
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
        public override void AddGetCodes(OpCodes codes, string op, Addr32 dest, Addr32 src)
        {
            codes.AddCodesSW(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpCodes codes, Addr32 ad)
        {
            codes.Add(I386.MovW(ad, Reg16.AX));
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
        public override void AddGetCodes(OpCodes codes, string op, Addr32 dest, Addr32 src)
        {
            codes.AddCodesSB(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpCodes codes, Addr32 ad)
        {
            codes.Add(I386.MovB(ad, Reg8.AL));
        }
    }
}
