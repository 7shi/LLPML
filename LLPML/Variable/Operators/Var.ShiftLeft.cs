using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Var
    {
        public class ShiftLeft : Add
        {
            public ShiftLeft() { }
            public ShiftLeft(BlockBase parent, Var dest) : base(parent, dest) { }
            public ShiftLeft(BlockBase parent, Var dest, params IIntValue[] values) : base(parent, dest, values) { }
            public ShiftLeft(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

            protected virtual string Shift { get { return "sal"; } }

            protected override void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
            {
                if (v is IntValue)
                {
                    int c = (v as IntValue).Value;
                    if (c < 0)
                    {
                        codes.Add(I386.Mov(ad, (Val32)0));
                    }
                    else if (c > 0)
                    {
                        if (c > 255) c = 255;
                        codes.Add(I386.Shift(Shift, ad, (byte)c));
                    }
                }
                else
                {
                    v.AddCodes(codes, m, "mov", null);
                    OpCode l1 = new OpCode();
                    OpCode l2 = new OpCode();
                    OpCode last = new OpCode();
                    codes.AddRange(new OpCode[]
                    {
                        I386.Cmp(Reg32.EAX, (Val32)0),
                        I386.Jcc(Cc.E, last.Address),
                        I386.Jcc(Cc.G, l1.Address),
                        I386.Mov(ad, (Val32)0),
                        I386.Jmp(last.Address),
                        l1,
                        I386.Cmp(Reg32.EAX, 255),
                        I386.Jcc(Cc.LE, l2.Address),
                        I386.Mov(Reg32.EAX, 255),
                        l2,
                        I386.Mov(Reg32.ECX, Reg32.EAX),
                        I386.Shift(Shift, ad, Reg8.CL),
                        last
                    });
                }
            }
        }
    }
}
