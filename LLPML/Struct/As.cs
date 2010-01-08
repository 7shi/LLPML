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

        public override int Min { get { return 2; } }
        public override int Max { get { return 2; } }

        public As(BlockBase parent, IIntValue v1, IIntValue v2) : base(parent, v1, v2) { }
        public As(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            var f = Parent.GetFunction(Tag);
            if (f == null) throw Abort("is: can not find: {0}", Tag);

            TypeOf.AddCodes(this, Parent, values[1], codes, "push", null);
            values[0].AddCodes(codes, "push", null);
            codes.AddRange(new[]
            {
                I386.Call(f.First),
                I386.Add(Reg32.ESP, 8),
            });
            codes.AddCodes(op, dest);
        }

        public override TypeBase Type
        {
            get
            {
                var ret = TypeOf.GetType(Parent, values[1]);
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
