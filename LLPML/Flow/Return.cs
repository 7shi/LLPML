using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Return : BreakBase
    {
        public bool IsLast = false;

        private NodeBase value;
        public NodeBase Value
        {
            get { return value; }
            set
            {
                this.value = value;
                Parent.GetFunction().AddTypeInfo(value);
            }
        }

        public static Return New(BlockBase parent, NodeBase value)
        {
            var ret = new Return();
            ret.init(parent);
            if (value != null) ret.Value = value;
            return ret;
        }

        public override void AddCodes(OpModule codes)
        {
            ///if (castFailed != null) throw Abort(castFailed);
            var f = Parent.GetFunction();
            if (value != null)
            {
                value.AddCodesV(codes, "mov", null);
                var retval = f.GetRetVal(Parent);
                var dest = retval.GetAddress(codes);
                if (!OpModule.NeedsDtor(value))
                {
                    codes.Add(I386.MovAR(dest, Reg32.EAX));
                    var tr = value.Type as TypeReference;
                    if (tr != null && tr.UseGC)
                        TypeReference.AddReferenceCodes(codes);
                }
                else
                {
                    codes.Add(I386.Push(Reg32.EAX));
                    var rt = f.ReturnType;
                    if (rt == null)
                        codes.Add(I386.MovAR(dest, Reg32.EAX));
                    else
                    {
                        if (rt is TypeReference)
                            codes.Add(I386.MovA(dest, Val32.New(0)));
                        rt.AddSetCodes(codes, dest);
                    }
                    codes.AddDtorCodes(value.Type);
                }
            }
            var b = Parent;
            var ptrs = UsingPointers;
            for (; ; ptrs = b.UsingPointers, b = b.Parent)
            {
                b.AddDestructors(codes, ptrs);
                if (b == f) break;
                b.AddExitCodes(codes);
            }
            if (!IsLast) codes.Add(I386.JmpD(b.Destruct));
        }
    }
}
