using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Is : Operator
    {
        public override string Tag { get { return "__type_is"; } }

        public static Is New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new Is(), parent, arg1, arg2) as Is;
        }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            var f = Parent.GetFunction(Tag);
            if (f == null) throw Abort("is: can not find: {0}", Tag);

            TypeOf.AddCodes(this, Parent, values[1] as NodeBase, codes, "push", null);
            TypeOf.AddCodes(this, Parent, values[0] as NodeBase, codes, "push", null);
            codes.Add(I386.CallD(f.First));
            codes.Add(I386.AddR(Reg32.ESP, Val32.New(8)));
            codes.AddCodes(op, dest);
        }

        public override TypeBase Type
        {
            get { return TypeBool.Instance; }
        }

        public override IntValue GetConst()
        {
            return null;
        }
    }
}
