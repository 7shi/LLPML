using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Var : VarBase, IIntValue
    {
        public class Add : Operator, IIntValue
        {
            public Add() { }
            public Add(BlockBase parent, Var dest) : base(parent, dest) { }
            public Add(BlockBase parent, Var dest, IntValue[] values) : base(parent, dest, values) { }
            public Add(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

            protected virtual void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
            {
                v.AddCodes(codes, m, "add", ad);
            }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                Addr32 ad = dest.GetAddress(codes, m);
                foreach (IIntValue v in values)
                {
                    Calculate(codes, m, ad, v);
                }
            }

            void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
            {
                AddCodes(codes, m);
                codes.Add(I386.Mov(Reg32.EAX, this.dest.GetAddress(codes, m)));
                IntValue.AddCodes(codes, op, dest);
            }
        }
    }
}
