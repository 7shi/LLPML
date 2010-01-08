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
        private VarBase target;
        private IIntValue order;

        public Index(BlockBase parent, VarBase ptr, IIntValue order)
            : base(parent)
        {
            this.target = ptr;
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
                var vs = IntValue.Read(parent, xr);
                if (vs == null) return;
                foreach (var v in vs)
                {
                    if (target == null)
                    {
                        target = v as VarBase;
                        if (target == null)
                            throw Abort("array required");
                    }
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

        public override Addr32 GetAddress(List<OpCode> codes, Module m)
        {
            var ts = target.TypeSize;
            if (ts == 0
                || (target is Var && !(target as Var).IsArray))
                throw Abort("{0} is not array", target.Name);
            var oi = order as IntValue;
            if (oi != null)
            {
                if (oi.Value < 0)
                    throw Abort("{0}[{1}]: over flow: < 0",
                        target.Name, oi.Value);
                var ret = target.GetAddress(codes, m);
                Pointer.Declare pd = null;
                if (target is Pointer)
                    pd = (target as Pointer).Reference;
                else if (target is Struct.Member)
                    pd = (target as Struct.Member).GetPointer();
                if (pd != null)
                {
                    if (oi.Value >= pd.Count)
                        throw Abort("{0}[{1}]: over flow: >= {2}",
                            target.Name, oi.Value, pd.Count);
                }
                else
                {
                    codes.Add(I386.Mov(Reg32.EDX, ret));
                    ret = new Addr32(Reg32.EDX);
                }
                ret.Add(ts * oi.Value);
                return ret;
            }
            codes.Add(I386.Push(Reg32.EAX));
            order.AddCodes(codes, m, "mov", null);
            codes.AddRange(new OpCode[]
                {
                    I386.Mov(Reg32.EDX, (uint)ts),
                    I386.Imul(Reg32.EDX),
                    I386.Push(Reg32.EAX),
                });
            if (target is Pointer)
                (target as Pointer).GetValue(codes, m);
            else
                (target as IIntValue).AddCodes(codes, m, "mov", null);
            codes.AddRange(new OpCode[]
                {
                    I386.Pop(Reg32.EDX),
                    I386.Add(Reg32.EDX, Reg32.EAX),
                });
            codes.Add(I386.Pop(Reg32.EAX));
            return new Addr32(Reg32.EDX);
        }

        public override bool IsArray { get { return false; } }
        public override string Type { get { return target.Type; } }

        public override Struct.Define GetStruct()
        {
            return target.GetStruct();
        }

        public override int Size { get { return target.TypeSize; } }
    }
}
