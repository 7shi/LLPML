using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Inc : Operand1
    {
        public Inc() { }
        public Inc(Block parent, string name) : base(parent, name) { }
        public Inc(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            codes.Add(I386.Inc(dest.GetAddress(codes, m)));
        }
    }

    public class Dec : Operand1
    {
        public Dec() { }
        public Dec(Block parent, string name) : base(parent, name) { }
        public Dec(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            codes.Add(I386.Dec(dest.GetAddress(codes, m)));
        }
    }

    public class Add : Operand2
    {
        public Add() { }
        public Add(Block parent, string name) : base(parent, name) { }
        public Add(Block parent, string name, int value) : base(parent, name, value) { }
        public Add(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            value.AddCodes(codes, m, "add", dest.GetAddress(codes, m));
        }
    }

    public class Sub : Operand2
    {
        public Sub() { }
        public Sub(Block parent, string name) : base(parent, name) { }
        public Sub(Block parent, string name, int value) : base(parent, name, value) { }
        public Sub(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            value.AddCodes(codes, m, "sub", dest.GetAddress(codes, m));
        }
    }
}
