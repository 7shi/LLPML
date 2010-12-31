using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Index : Var
    {
        private IIntValue target;
        private IIntValue order;

        public Index(BlockBase parent, IIntValue target, IIntValue order)
            : base(parent)
        {
            this.target = target;
            this.order = order;
        }

        public Index(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                var vs = IntValue.Read(Parent, xr);
                if (vs == null) return;
                foreach (var v in vs)
                {
                    if (target == null)
                        target = v;
                    else if (order == null)
                        order = v;
                    else
                        throw Abort(xr, "too many operands");
                }
            });
            if (target == null)
                throw Abort(xr, "array required");
            else if (order == null)
                throw Abort(xr, "order required");
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
                    codes.Add(I386.Mov(Var.DestRegister, ret));
                    ret = Addr32.New(Var.DestRegister);
                }
                ret.Add(ts * oi.Value);
                return ret;
            }

            order.AddCodes(codes, "mov", null);
            codes.Add(I386.Mov(Reg32.EDX, Val32.NewI(ts)));
            codes.Add(I386.Imul(Reg32.EDX));
            codes.Add(I386.Push(Reg32.EAX));
            target.AddCodes(codes, "mov", null);
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
                if (target is Struct.Member)
                    throw Abort("index: undefined member: {0}",
                        (target as Struct.Member).FullName);
                else if (target is NodeBase)
                    throw Abort("index: undefined symbol: {0}",
                        (target as NodeBase).Name);
                throw Abort("index: undefined symbol");
            }
        }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            Type.AddGetCodes(codes, op, dest, GetAddress(codes));
        }
    }
}
