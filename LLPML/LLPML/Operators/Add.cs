using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Add : Operator, IIntValue
    {
        public Add() { }
        public Add(Block parent) : base(parent) { }
        public Add(Block parent, IntValue[] values) : base(parent, values) { }
        public Add(Block parent, XmlTextReader xr) : base(parent, xr) { }

        protected virtual void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
        {
            v.AddCodes(codes, m, "add", ad);
        }

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            bool first = true;
            Addr32 ad = new Addr32(Reg32.ESP);
            foreach (IIntValue v in values)
            {
                if (first)
                {
                    v.AddCodes(codes, m, "push", null);
                    first = false;
                }
                else
                {
                    Calculate(codes, m, ad, v);
                }
            }
            if (op != "push")
            {
                codes.Add(I386.Pop(Reg32.EAX));
                IntValue.AddCodes(codes, op, dest);
            }
        }
    }
}
