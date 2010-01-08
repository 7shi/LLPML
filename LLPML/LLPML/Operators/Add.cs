using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Add : Operands, IIntValue
    {
        public Add() { }
        public Add(Block parent, string name) : base(parent, name) { }
        public Add(Block parent, string name, IntValue[] values) : base(parent, name, values) { }
        public Add(Block parent, XmlTextReader xr) : base(parent, xr) { }

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
                    v.AddCodes(codes, m, "add", ad);
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
