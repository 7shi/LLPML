using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class New : NodeBase, IIntValue
    {
        public const string Function = "__operator_new";
        public TypeBase Type { get; protected set; }

        public New(BlockBase parent, string type)
            : base(parent)
        {
            Type = new TypeReference(new TypeStruct(parent, type));
        }

        public New(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            Type = new TypeReference(new TypeStruct(parent, xr["type"]));
        }

        public override void AddCodes(OpCodes codes)
        {
            AddCodes(codes, "mov", null);
        }

        public void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            var tt = Type.Type;
            var st = (tt as TypeStruct).GetStruct();
            if (st == null)
                throw Abort("new: undefined struct: {0}", tt.Name);
            var f1 = parent.GetFunction(Function);
            if (f1 == null)
                throw Abort("new: undefined function: {0}", Function);
            else if (f1.CallType != CallType.CDecl)
                throw Abort("new: {0} must be __cdecl", Function);
            var f2 = st.GetFunction(Define.Initializer);
            var f3 = st.GetFunction(Define.Constructor);
            if (f3.CallType != CallType.CDecl)
                throw Abort("new: {0} must be __cdecl", f3.FullName);
            codes.AddRange(new[]
            {
                I386.Push((uint)st.GetSize()),
                I386.Call(f1.First),
                I386.Add(Reg32.ESP, 4),
                I386.Push(Reg32.EAX),
                I386.Call(f2.First),
                I386.Call(f3.First),
            });
            if (op == "push" && dest == null) return;
            codes.Add(I386.Pop(Reg32.EAX));
            codes.AddCodes(op, dest);
        }
    }
}
