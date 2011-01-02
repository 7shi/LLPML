using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeOf : NodeBase
    {
        public NodeBase Target { get; private set; }

        public TypeOf(BlockBase parent, NodeBase target) : base(parent) { Target = target; }
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

        public override TypeBase Type { get { return TypeType.Instance; } }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            AddCodes(this, Parent, Target, codes, op, dest);
        }

        public static TypeBase GetType(BlockBase parent, NodeBase target)
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

        public static void AddCodes(NodeBase caller, BlockBase parent, NodeBase target, OpModule codes, string op, Addr32 dest)
        {
            if (target is TypeOf) target = (target as TypeOf).Target;

            var v = target as Variant;
            if (v != null && parent.GetFunction(v.Name) == null)
            {
                var fpname = (target as Variant).Name;
                var fpt = Types.GetType(parent, fpname);
                if (fpt == null || !fpt.Check())
                    throw caller.Abort("undefined type: {0}", fpname);
                codes.AddCodesV(op, dest, codes.GetTypeObject(fpt));
                return;
            }

            var tt = target.Type;
            var tr = tt as TypeReference;
            var tts = tt.Type as TypeStruct;
            if (tr != null && (tr.IsArray || (tts != null && tts.IsClass)))
            {
                target.AddCodes(codes, "mov", null);
                var label = new OpCode();
                codes.Add(I386.Test(Reg32.EAX, Reg32.EAX));
                codes.Add(I386.Jcc(Cc.Z, label.Address));
                codes.Add(I386.MovRA(Reg32.EAX, Addr32.NewRO(Reg32.EAX, -16)));
                codes.Add(label);
                codes.AddCodes(op, dest);
            }
            else
                codes.AddCodesV(op, dest, codes.GetTypeObject(tt));
        }
    }
}
