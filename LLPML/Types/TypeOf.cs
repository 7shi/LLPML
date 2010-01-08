using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeOf : NodeBase, IIntValue
    {
        public IIntValue Target { get; private set; }

        public TypeOf(BlockBase parent, IIntValue target) : base(parent) { Target = target; }
        public TypeOf(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                var vs = IntValue.Read(Parent, xr);
                if (vs == null) return;
                foreach (var v in vs)
                {
                    if (Target != null)
                        throw Abort(xr, "too many operands");
                    Target = v;
                }
            });
            if (Target == null)
                throw Abort(xr, "target required");
        }

        public TypeBase Type { get { return TypeType.Instance; } }

        public void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            AddCodes(this, Parent, Target, codes, op, dest);
        }

        public static TypeBase GetType(BlockBase parent, IIntValue target)
        {
            var v = target as Variant;
            if (v != null)
            {
                var vt = v.GetVariantType();
                if (vt != null) return vt;
                return Types.GetType(parent, v.Name);
            }
            return target.Type;
        }

        public static void AddCodes(NodeBase caller, BlockBase parent, IIntValue target, OpModule codes, string op, Addr32 dest)
        {
            if (target is TypeOf) target = (target as TypeOf).Target;

            var v = target as Variant;
            if (v != null && parent.GetFunction(v.Name) == null)
            {
                var fpname = (target as Variant).Name;
                var fpt = Types.GetType(parent, fpname);
                if (fpt == null || !fpt.Check())
                    throw caller.Abort("undefined type: {0}", fpname);
                codes.AddCodes(op, dest, codes.GetTypeObject(fpt));
                return;
            }

            var tt = target.Type;
            var tr = tt as TypeReference;
            var tts = tt.Type as TypeStruct;
            if (tr != null && (tr.IsArray || (tts != null && tts.IsClass)))
            {
                target.AddCodes(codes, "mov", null);
                var label = new OpCode();
                codes.AddRange(new[]
                {
                    I386.Test(Reg32.EAX, Reg32.EAX),
                    I386.Jcc(Cc.Z, label.Address),
                    I386.Mov(Reg32.EAX, new Addr32(Reg32.EAX, -16)),
                    label,
                });
                codes.AddCodes(op, dest);
            }
            else
                codes.AddCodes(op, dest, codes.GetTypeObject(tt));
        }
    }
}
