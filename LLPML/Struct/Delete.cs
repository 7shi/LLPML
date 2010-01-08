using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Delete : NodeBase
    {
        public const string Function = "__operator_delete";
        public IIntValue Target { get; protected set; }

        public Delete(BlockBase parent, IIntValue target)
            : base(parent)
        {
            Target = target;
        }

        public Delete(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                var vs = IntValue.Read(parent, xr);
                if (vs == null) return;
                foreach (IIntValue v in vs)
                {
                    if (Target == null)
                        Target = v;
                    else
                        throw Abort("delete: too many operands");
                }
            });
            if (Target == null)
                throw Abort(xr, "no target specified");
        }

        public override void AddCodes(OpCodes codes)
        {
            var f = parent.GetFunction(Function);
            if (f == null)
                throw Abort("delete: undefined function: {0}", Function);
            Target.AddCodes(codes, "push", null);
            var t = Target.Type;
            if (t is TypeReference && t.Type is TypeStruct)
            {
                var st = (t.Type as TypeStruct).GetStruct();
                if (st == null)
                    throw Abort("delete: undefined struct: {0}", t.Type.Name);
                var dtor = st.GetFunction(Define.Destructor);
                if (dtor.CallType != CallType.CDecl)
                    throw Abort("delete: {0} must be __cdecl", dtor.FullName);
                codes.Add(I386.Call(dtor.First));
            }
            codes.Add(I386.Call(f.First));
            if (f.CallType == CallType.CDecl)
                codes.Add(I386.Add(Reg32.ESP, 4));
        }
    }
}
