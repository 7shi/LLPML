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
            funcs["shift-left" ] = (codes, dest, arg) => Shift("sal", codes, dest, arg);
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
}
