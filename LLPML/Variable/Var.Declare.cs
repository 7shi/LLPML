using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Var
    {
        public class Declare : Pointer.Declare
        {
            public IIntValue Value { get; set; }

            public string Type { get { return type; } }
            public override int Length { get { return Var.Size; } }

            public Declare() { }

            public Declare(BlockBase parent, string name)
                : base(parent, name)
            {
            }

            public Declare(BlockBase parent, string name, IIntValue value)
                : this(parent, name)
            {
                Value = value;
            }

            public Declare(BlockBase parent, string name, int value)
                : this(parent, name, new IntValue(value))
            {
            }

            public Declare(BlockBase parent, string name, string type)
                : this(parent, name)
            {
                this.type = type;
            }

            public Declare(BlockBase parent, XmlTextReader xr)
                : base(parent, xr)
            {
            }

            public override void Read(XmlTextReader xr)
            {
                RequiresName(xr);
                type = xr["type"];

                Parse(xr, delegate
                {
                    IIntValue[] v = IntValue.Read(parent, xr);
                    if (v != null)
                    {
                        if (v.Length > 1 || Value != null)
                            throw Abort(xr, "multiple values");
                        Value = v[0];
                    }
                });

                AddToParent();
            }

            protected override void AddToParent()
            {
                if (!parent.AddVar(this))
                    throw Abort("multiple definitions: " + name);
            }

            public Struct.Define GetStruct()
            {
                if (type == null) return null;

                Struct.Define st = parent.GetStruct(type);
                if (st != null) return st;
                throw Abort("undefined struct: " + type);
            }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                if (Value == null) return;

                if (HasThis)
                {
                    Value.AddCodes(codes, m, "mov", null);
                    codes.Add(I386.Mov(GetAddress(codes, m, null), Reg32.EAX));
                }
                else
                {
                    Value.AddCodes(codes, m, "mov", address);
                }
            }
        }
    }
}
