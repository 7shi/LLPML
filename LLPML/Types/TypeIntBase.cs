using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class TypeVarBase : TypeBase
    {
        // get value
        public override void AddGetCodes(OpModule codes, string op, Addr32 dest, Addr32 src)
        {
            codes.AddCodes(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpModule codes, Addr32 ad)
        {
            codes.Add(I386.Mov(ad, Reg32.EAX));
        }
    }

    public abstract class TypeIntBase : TypeVarBase
    {
        public static void AddOperators(Dictionary<string, Func> funcs)
        {
            funcs["inc"] = funcs["post-inc"] = (codes, dest) => codes.Add(I386.Inc(dest));
            funcs["dec"] = funcs["post-dec"] = (codes, dest) => codes.Add(I386.Dec(dest));

            funcs["add"] = (codes, dest) => codes.Add(I386.Add(dest, Reg32.EAX));
            funcs["sub"] = (codes, dest) => codes.Add(I386.Sub(dest, Reg32.EAX));
            funcs["and"] = (codes, dest) => codes.Add(I386.And(dest, Reg32.EAX));
            funcs["or"] = (codes, dest) => codes.Add(I386.Or(dest, Reg32.EAX));
            funcs["xor"] = (codes, dest) => codes.Add(I386.Xor(dest, Reg32.EAX));

            funcs["not"] = (codes, dest) =>
            {
                codes.AddRange(new[]
                {
                    I386.Test(Reg32.EAX, Reg32.EAX),
                    I386.Mov(Reg32.EAX, (Val32)0),
                    I386.Setcc(Cc.Z, Reg8.AL)
                });
            };
            funcs["neg"] = (codes, dest) => codes.Add(I386.Neg(Reg32.EAX));
            funcs["rev"] = (codes, dest) => codes.Add(I386.Not(Reg32.EAX));
        }

        public static void AddComparers(Dictionary<string, Func> funcs, Dictionary<string, CondPair> conds)
        {
            funcs["equal"]
                = funcs["not-equal"]
                = funcs["greater"]
                = funcs["greater-equal"]
                = funcs["less"]
                = funcs["less-equal"]
                = (codes, dest) => codes.Add(I386.Cmp(dest, Reg32.EAX));
            conds["equal"] = new CondPair(Cc.E, Cc.NE);
            conds["not-equal"] = new CondPair(Cc.NE, Cc.E);
        }

        public static void Shift(string shift, OpModule codes, Addr32 dest)
        {
            var l1 = new OpCode();
            var l2 = new OpCode();
            var last = new OpCode();
            codes.AddRange(new[]
            {
                I386.Cmp(Reg32.EAX, (Val32)0),
                I386.Jcc(Cc.E, last.Address),
                I386.Jcc(Cc.G, l1.Address),
                I386.Mov(dest, (Val32)0),
                I386.Jmp(last.Address),
                l1,
                I386.Cmp(Reg32.EAX, 255),
                I386.Jcc(Cc.LE, l2.Address),
                I386.Mov(Reg32.EAX, 255),
                l2,
                I386.Mov(Reg32.ECX, Reg32.EAX),
                I386.Shift(shift, dest, Reg8.CL),
                last
            });
        }
    }
}
