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
}
