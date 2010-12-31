using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypePointer : TypeVarBase
    {
        // type name
        public override string Name { get { return "var:" + Type.Name + "*"; } }

        // type size
        //public override int Size { get { return Type.Size; } }

        // check array
        public override bool IsArray { get { return true; } }

        // cast
        public override TypeBase Cast(TypeBase type)
        {
            if (type is TypeVar) return type;
            if (!(type is TypePointer || type is TypeReference)) return null;
            return base.Cast(type);
        }

        public TypePointer(TypeBase type)
        {
            Type = type;

            funcs["inc"] = funcs["post-inc"] = (codes, dest) =>
            {
                if (Type.Size == 1)
                    codes.Add(I386.IncA(dest));
                else
                    codes.Add(I386.AddA(dest, Val32.NewI(Type.Size)));
            };
            funcs["dec"] = funcs["post-dec"] = (codes, dest) =>
            {
                if (Type.Size == 1)
                    codes.Add(I386.DecA(dest));
                else
                    codes.Add(I386.SubA(dest, Val32.NewI(Type.Size)));
            };
            funcs["add"] = (codes, dest) =>
            {
                codes.Add(I386.MovR(Reg32.EDX, Val32.NewI(Type.Size)));
                codes.Add(I386.Mul(Reg32.EDX));
                codes.Add(I386.AddAR(dest, Reg32.EAX));
            };
            funcs["sub"] = (codes, dest) =>
            {
                codes.Add(I386.MovR(Reg32.EDX, Val32.NewI(Type.Size)));
                codes.Add(I386.Mul(Reg32.EDX));
                codes.Add(I386.SubAR(dest, Reg32.EAX));
            };

            // partial inheritance
            TypeIntBase.AddComparers(funcs, conds);
            TypeInt.AddComparers(conds);
        }
    }
}
