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
        protected TypeVarBase()
        {
            conds["equal"] = CondPair.New(Cc.E, Cc.NE);
            conds["not-equal"] = CondPair.New(Cc.NE, Cc.E);
        }

        // get value
        public override void AddGetCodes(OpModule codes, string op, Addr32 dest, Addr32 src)
        {
            codes.AddCodesA(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpModule codes, Addr32 ad)
        {
            codes.Add(I386.MovAR(ad, Reg32.EAX));
        }

        public override bool CheckFunc(string op)
        {
            switch (op)
            {
                case "equal":
                case "not-equal":
                case "greater":
                case "greater-equal":
                case "less":
                case "less-equal":
                    return true;
                default:
                    return base.CheckFunc(op);
            }
        }

        public override void AddOpCodes(string op, OpModule codes, Addr32 dest)
        {
            switch (op)
            {
                case "equal":
                case "not-equal":
                case "greater":
                case "greater-equal":
                case "less":
                case "less-equal":
                    codes.Add(I386.CmpAR(dest, Reg32.EAX));
                    break;
                default:
                    base.AddOpCodes(op, codes, dest);
                    break;
            }
        }
    }

    public abstract class TypeIntBase : TypeVarBase
    {

        public override bool CheckFunc(string op)
        {
            switch (op)
            {
                case "inc":
                case "post-inc":
                case "dec":
                case "post-dec":
                case "add":
                case "sub":
                case "and":
                case "or":
                case "xor":
                case "not":
                case "neg":
                case "rev":
                    return true;
                default:
                    return base.CheckFunc(op);
            }
        }

        public override void AddOpCodes(string op, OpModule codes, Addr32 dest)
        {
            switch (op)
            {
                case "inc":
                case "post-inc":
                    codes.Add(I386.IncA(dest));
                    break;
                case "dec":
                case "post-dec":
                    codes.Add(I386.DecA(dest));
                    break;
                case "add":
                    codes.Add(I386.AddAR(dest, Reg32.EAX));
                    break;
                case "sub":
                    codes.Add(I386.SubAR(dest, Reg32.EAX));
                    break;
                case "and":
                    codes.Add(I386.AndAR(dest, Reg32.EAX));
                    break;
                case "or":
                    codes.Add(I386.OrAR(dest, Reg32.EAX));
                    break;
                case "xor":
                    codes.Add(I386.XorAR(dest, Reg32.EAX));
                    break;
                case "not":
                    codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
                    codes.Add(I386.MovR(Reg32.EAX, Val32.New(0)));
                    codes.Add(I386.Setcc(Cc.Z, Reg8.AL));
                    break;
                case "neg":
                    codes.Add(I386.Neg(Reg32.EAX));
                    break;
                case "rev":
                    codes.Add(I386.Not(Reg32.EAX));
                    break;
                default:
                    base.AddOpCodes(op, codes, dest);
                    break;
            }
        }

        protected static void Shift(string shift, OpModule codes, Addr32 dest)
        {
            var l1 = new OpCode();
            var l2 = new OpCode();
            var last = new OpCode();
            codes.Add(I386.CmpR(Reg32.EAX, Val32.New(0)));
            codes.Add(I386.Jcc(Cc.E, last.Address));
            codes.Add(I386.Jcc(Cc.G, l1.Address));
            codes.Add(I386.MovA(dest, Val32.New(0)));
            codes.Add(I386.JmpD(last.Address));
            codes.Add(l1);
            codes.Add(I386.CmpR(Reg32.EAX, Val32.New(255)));
            codes.Add(I386.Jcc(Cc.LE, l2.Address));
            codes.Add(I386.MovR(Reg32.EAX, Val32.New(255)));
            codes.Add(l2);
            codes.Add(I386.Mov(Reg32.ECX, Reg32.EAX));
            codes.Add(I386.ShiftAR(shift, dest, Reg8.CL));
            codes.Add(last);
        }
    }
}
