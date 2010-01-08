using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeIterator : TypeBase
    {
        // type name
        public override string Name { get { return Type.Name + "[]"; } }

        // check array
        public override bool IsArray { get { return true; } }

        // cast
        public override TypeBase Cast(TypeBase type)
        {
            if (type is TypeVar) return type;
            if (!(type is TypeIterator || type is TypeReference)) return null;
            return base.Cast(type);
        }

        public TypeIterator(TypeBase type)
        {
            Type = type;

            funcs["inc"] = funcs["post-inc"] = (codes, dest, arg) =>
            {
                if (Size == 1)
                    codes.Add(I386.Inc(dest));
                else
                    codes.Add(I386.Add(dest, (uint)Size));
            };
            funcs["dec"] = funcs["post-dec"] = (codes, dest, arg) =>
            {
                if (Size == 1)
                    codes.Add(I386.Dec(dest));
                else
                    codes.Add(I386.Sub(dest, (uint)Size));
            };
            funcs["add"] = (codes, dest, arg) =>
            {
                if (arg is IntValue)
                {
                    var v = (arg as IntValue).Value * Size;
                    if (v == 1)
                        codes.Add(I386.Inc(dest));
                    else
                        codes.Add(I386.Add(dest, (uint)v));
                }
                else
                {
                    arg.AddCodes(codes, "mov", null);
                    codes.AddRange(new OpCode[]
                    {
                        I386.Mov(Reg32.EDX, (uint)Size),
                        I386.Mul(Reg32.EDX),
                        I386.Add(dest, Reg32.EAX)
                    });
                }
            };
            funcs["sub"] = (codes, dest, arg) =>
            {
                if (arg is IntValue)
                {
                    var v = (arg as IntValue).Value * Size;
                    if (v == 1)
                        codes.Add(I386.Dec(dest));
                    else
                        codes.Add(I386.Sub(dest, (uint)v));
                }
                else
                {
                    arg.AddCodes(codes, "mov", null);
                    codes.AddRange(new OpCode[]
                    {
                        I386.Mov(Reg32.EDX, (uint)Size),
                        I386.Mul(Reg32.EDX),
                        I386.Sub(dest, Reg32.EAX)
                    });
                }
            };

            // partial inheritance
            TypeIntBase.AddComparers(funcs, conds);
            TypeInt.AddComparers(conds);
        }

        // get value
        public override void AddGetCodes(OpCodes codes, string op, Addr32 dest, Addr32 src)
        {
            codes.AddCodes(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpCodes codes, Addr32 dest)
        {
            codes.Add(I386.Mov(dest, Reg32.EAX));
        }
    }
}
