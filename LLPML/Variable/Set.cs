using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.LLPML.Struct;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Set : VarOperator
    {
        public override string Tag { get { return "set"; } }

        public static Set New(BlockBase parent, NodeBase dest, NodeBase arg)
        {
            return Init1(new Set(), parent, dest, arg) as Set;
        }

        public override void AddCodes(OpModule codes)
        {
            var dest = Var.Get(this.dest);
            if (dest == null)
            {
                if (this.dest is Variant)
                {
                    var setter = (this.dest as Variant).GetSetter();
                    if (setter != null)
                    {
                        new Call(Parent, setter, This.New(Parent), values[0])
                            .AddCodes(codes);
                        return;
                    }
                }
                throw Abort("set: destination is not variable");
            }
            var dt = dest.Type;
            if (dt is TypeConstChar)
                throw Abort("set: can not change constants");
            if (dest is Member)
            {
                var mem = dest as Member;
                if (mem.IsSetter)
                {
                    mem.AddSetterCodes(codes, values[0]);
                    return;
                }
            }

            var v = values[0];
            v.AddCodesV(codes, "push", null);
            var ad = dest.GetAddress(codes);
            if (!OpModule.NeedsDtor(v))
            {
                if (ad == null)
                {
                    ad = Addr32.New(Reg32.ESP);
                    codes.Add(I386.XchgRA(Reg32.EAX, ad));
                    dt.AddSetCodes(codes, ad);
                    codes.Add(I386.AddR(Reg32.ESP, Val32.New(4)));
                }
                else
                {
                    codes.Add(I386.Pop(Reg32.EAX));
                    dt.AddSetCodes(codes, ad);
                }
            }
            else
            {
                codes.Add(I386.MovRA(Reg32.EAX, Addr32.New(Reg32.ESP)));
                dt.AddSetCodes(codes, ad);
                codes.AddDtorCodes(v.Type);
            }
        }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            AddCodes(codes);
            codes.AddCodes(op, dest);
        }
    }
}
