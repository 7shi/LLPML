using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeArray : TypeBase
    {
        // type name
        public override string Name { get { return type; } }

        private string type;
        private int size;
        public override int Size { get { return size; } }

        public TypeArray(string type, int size)
        {
            this.type = type;
            this.size = size;

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
            TypeUInt.AddComparers(conds);
        }
    }
}
