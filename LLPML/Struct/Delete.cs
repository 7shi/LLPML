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
        public const string GetSize = "__get_heap_size";

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
            var f1 = parent.GetFunction(Function);
            if (f1 == null)
                throw Abort("delete: undefined function: {0}", Function);
            Target.AddCodes(codes, "push", null);
            var t = Target.Type;
            if (t is TypeReference)
            {
                if (t.Type.NeedsDtor)
                    t.Type.AddDestructor(codes);
            }
            else if (t is TypeIterator)
            {
                if (t.Type.NeedsDtor)
                {
                    var f2 = parent.GetFunction(GetSize);
                    if (f2 == null)
                        throw Abort("delete: undefined function: {0}", GetSize);
                    if (f2.CallType != CallType.CDecl)
                        codes.Add(I386.Push(new Addr32(Reg32.ESP)));
                    var label1 = new OpCode();
                    var label2 = new OpCode();
                    var size = t.Type.Size;
                    codes.AddRange(new[]
                    {
                        I386.Call(f2.First),
                        I386.Test(Reg32.EAX, Reg32.EAX),
                        I386.Jcc(Cc.Z, label2.Address),
                        I386.Push(Reg32.EAX),
                        I386.Push(new Addr32(Reg32.ESP, 4)),
                        I386.Add(new Addr32(Reg32.ESP), Reg32.EAX),
                        label1,
                        I386.Sub(new Addr32(Reg32.ESP), (uint)size),
                    });
                    t.Type.AddDestructor(codes);
                    codes.AddRange(new[]
                    {
                        I386.Sub(new Addr32(Reg32.ESP, 4), (uint)size),
                        I386.Jcc(Cc.G, label1.Address),
                        I386.Add(Reg32.ESP, 8),
                        label2,
                    });
                }
            }
            else
                throw Abort("delete: can not delete: {0}", t.Name);
            codes.Add(I386.Call(f1.First));
            if (f1.CallType == CallType.CDecl)
                codes.Add(I386.Add(Reg32.ESP, 4));
        }
    }
}
