using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class VarInt : VarBase
    {
        public class ShiftLeft : Add
        {
            public ShiftLeft() { }
            public ShiftLeft(Block parent, VarInt dest) : base(parent, dest) { }
            public ShiftLeft(Block parent, VarInt dest, IntValue[] values) : base(parent, dest, values) { }
            public ShiftLeft(Block parent, XmlTextReader xr) : base(parent, xr) { }

            protected virtual string Shift { get { return "sal"; } }

            protected override void Calculate(List<OpCode> codes, Module m, Addr32 ad, IIntValue v)
            {
                if (v is IntValue)
                {
                    int c = (v as IntValue).Value;
                    if (c < 0)
                    {
                        codes.Add(I386.Mov(ad, (uint)0));
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
                        I386.Cmp(Reg32.EAX, (uint)0),
                        I386.Jcc(Cc.E, last.Address),
                        I386.Jcc(Cc.G, l1.Address),
                        I386.Mov(ad, (uint)0),
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
