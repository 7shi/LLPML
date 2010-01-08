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
        public override void AddGetCodes(OpCodes codes, string op, Addr32 dest, Addr32 src)
        {
            codes.AddCodes(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpCodes codes, Addr32 ad)
        {
            codes.Add(I386.Mov(ad, Reg32.EAX));
        }
    }

    public abstract class TypeIntBase : TypeVarBase
    {
        public static void AddOperators(Dictionary<string, Func> funcs)
        {
            funcs["inc"] = funcs["post-inc"] = (codes, dest, arg) => codes.Add(I386.Inc(dest));
            funcs["dec"] = funcs["post-dec"] = (codes, dest, arg) => codes.Add(I386.Dec(dest));

            funcs["add"] = (codes, dest, arg) => arg.AddCodes(codes, "add", dest);
            funcs["sub"] = (codes, dest, arg) => arg.AddCodes(codes, "sub", dest);
            funcs["and"] = (codes, dest, arg) => arg.AddCodes(codes, "and", dest);
            funcs["or"] = (codes, dest, arg) => arg.AddCodes(codes, "or", dest);
            funcs["xor"] = (codes, dest, arg) => arg.AddCodes(codes, "xor", dest);

            funcs["not"] = (codes, dest, arg) =>
            {
                codes.AddRange(new[]
                {
                    I386.Test(Reg32.EAX, Reg32.EAX),
                    I386.Mov(Reg32.EAX, (Val32)0),
                    I386.Setcc(Cc.Z, Reg8.AL)
                });
            };
            funcs["neg"] = (codes, dest, arg) => codes.Add(I386.Neg(Reg32.EAX));
            funcs["rev"] = (codes, dest, arg) => codes.Add(I386.Not(Reg32.EAX));
        }

        public static void AddComparers(Dictionary<string, Func> funcs, Dictionary<string, CondPair> conds)
        {
            funcs["equal"]
                = funcs["not-equal"]
                = funcs["greater"]
                = funcs["greater-equal"]
                = funcs["less"]
                = funcs["less-equal"]
                = (codes, dest, arg) =>
                  {
                      arg.AddCodes(codes, "mov", null);
                      codes.Add(I386.Cmp(dest, Reg32.EAX));
                  };
            conds["equal"] = new CondPair(Cc.E, Cc.NE);
            conds["not-equal"] = new CondPair(Cc.NE, Cc.E);
        }

        public static void Shift(string shift, OpCodes codes, Addr32 dest, IIntValue arg)
        {
            if (arg is IntValue)
            {
                int c = (arg as IntValue).Value;
                if (c < 0)
                {
                    codes.Add(I386.Mov(dest, (Val32)0));
                }
                else if (c > 0)
                {
                    if (c > 255) c = 255;
                    codes.Add(I386.Shift(shift, dest, (byte)c));
                }
            }
            else
            {
                arg.AddCodes(codes, "mov", null);
                OpCode l1 = new OpCode();
                OpCode l2 = new OpCode();
                OpCode last = new OpCode();
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
}
