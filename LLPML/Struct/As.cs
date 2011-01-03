using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class As : Operator
    {
        public override string Tag { get { return "__type_as"; } }

        public static As New(BlockBase parent, NodeBase arg1, NodeBase arg2)
        {
            return Init2(new As(), parent, arg1, arg2) as As;
        }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            var f = Parent.GetFunction(Tag);
            if (f == null) throw Abort("is: can not find: {0}", Tag);

            TypeOf.AddCodes(this, Parent, values[1] as NodeBase, codes, "push", null);
            (values[0] as NodeBase).AddCodesV(codes, "push", null);
            codes.Add(I386.CallD(f.First));
            codes.Add(I386.AddR(Reg32.ESP, Val32.New(8)));
            codes.AddCodes(op, dest);
        }

        public override TypeBase Type
        {
            get
            {
                var ret = TypeOf.GetType(Parent, values[1] as NodeBase);
                if (ret is TypeStruct && (ret as TypeStruct).IsClass)
                    return Types.ToVarType(ret);
                else
                    return ret;
            }
        }

        public override IntValue GetConst()
        {
            return null;
        }
    }
}
