using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.LLPML.Struct;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Index : Var
    {
        private NodeBase target;
        private NodeBase order;

        public static Index New(BlockBase parent, NodeBase target, NodeBase order)
        {
            var ret = new Index();
            ret.Parent = parent;
            ret.target = target;
            ret.order = order;
            return ret;
        }

        public override Addr32 GetAddress(OpModule codes)
        {
            var target = Var.Get(this.target);
            if (target == null)
                throw Abort("index: source is not array");
            var t = target.Type;
            int ts;
            if (t is TypeString)
                ts = TypeChar.Instance.Size;
            else if (!t.IsArray)
                throw Abort("{0} is not array", target.Name);
            else
                ts = t.Type.Size;
            var oi = order as IntValue;
            if (oi != null)
            {
                if (oi.Value < 0)
                    throw Abort("{0}[{1}]: over flow: < 0",
                        target.Name, oi.Value);
                if (t is TypeArray)
                {
                    var tc = (t as TypeArray).Count;
                    if (oi.Value >= tc)
                        throw Abort("{0}[{1}]: over flow: >= {2}",
                            target.Name, oi.Value, tc);
                }
                var ret = target.GetAddress(codes);
                if (ret == null)
                {
                    codes.Add(I386.Mov(Var.DestRegister, Reg32.EAX));
                    ret = Addr32.New(Var.DestRegister);
                }
                else if (t.IsValue)
                {
                    codes.Add(I386.MovRA(Var.DestRegister, ret));
                    ret = Addr32.New(Var.DestRegister);
                }
                ret.Add(ts * oi.Value);
                return ret;
            }

            order.AddCodesV(codes, "mov", null);
            codes.Add(I386.MovR(Reg32.EDX, Val32.NewI(ts)));
            codes.Add(I386.Imul(Reg32.EDX));
            codes.Add(I386.Push(Reg32.EAX));
            target.AddCodesV(codes, "mov", null);
            codes.Add(I386.Pop(Var.DestRegister));
            codes.Add(I386.Add(Var.DestRegister, Reg32.EAX));
            return Addr32.New(Var.DestRegister);
        }

        public override TypeBase Type
        {
            get
            {
                var t = target.Type;
                if (t is TypeString)
                    return TypeConstChar.Instance;
                else if (t != null)
                    return t.Type;
                if (target is Member)
                    throw Abort("index: undefined member: {0}",
                        (target as Member).FullName);
                else if (target is NodeBase)
                    throw Abort("index: undefined symbol: {0}",
                        (target as NodeBase).Name);
                throw Abort("index: undefined symbol");
            }
        }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            Type.AddGetCodes(codes, op, dest, GetAddress(codes));
        }
    }
}
