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

        public static TypePointer New(TypeBase type)
        {
            var ret = new TypePointer();
            ret.Type = type;
            ret.conds["greater"] = CondPair.New(Cc.A, Cc.NA);
            ret.conds["greater-equal"] = CondPair.New(Cc.AE, Cc.NAE);
            ret.conds["less"] = CondPair.New(Cc.B, Cc.NB);
            ret.conds["less-equal"] = CondPair.New(Cc.BE, Cc.NBE);
            return ret;
        }

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
                    if (Type.Size == 1)
                        codes.Add(I386.IncA(dest));
                    else
                        codes.Add(I386.AddA(dest, Val32.NewI(Type.Size)));
                    break;
                case "dec":
                case "post-dec":
                    if (Type.Size == 1)
                        codes.Add(I386.DecA(dest));
                    else
                        codes.Add(I386.SubA(dest, Val32.NewI(Type.Size)));
                    break;
                case "add":
                    codes.Add(I386.MovR(Reg32.EDX, Val32.NewI(Type.Size)));
                    codes.Add(I386.Mul(Reg32.EDX));
                    codes.Add(I386.AddAR(dest, Reg32.EAX));
                    break;
                case "sub":
                    codes.Add(I386.MovR(Reg32.EDX, Val32.NewI(Type.Size)));
                    codes.Add(I386.Mul(Reg32.EDX));
                    codes.Add(I386.SubAR(dest, Reg32.EAX));
                    break;
                default:
                    base.AddOpCodes(op, codes, dest);
                    break;
            }
        }

        public override CondPair GetCond(string key)
        {
            return base.GetCond(key);
        }
    }
}
