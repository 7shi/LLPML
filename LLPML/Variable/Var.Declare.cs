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
            public bool IsArray { get; set; }
            public bool IsValue { get { return length > 0; } }

            private int length = Var.DefaultSize;
            public override int Length { get { return length; } }

            public override string Type
            {
                set
                {
                    if (value != null && value.EndsWith("[]"))
                    {
                        base.Type = value.Substring(0, value.Length - 2).TrimEnd();
                        length = Var.DefaultSize;
                        IsArray = true;
                    }
                    else
                    {
                        base.Type = value;
                        length = SizeOf.GetValueSize(value);
                        if (length == 0) length = Var.DefaultSize;
                        IsArray = false;
                    }
                }
            }

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
                this.Type = type;
            }

            public Declare(BlockBase parent, XmlTextReader xr)
                : base(parent, xr)
            {
            }

            public override void Read(XmlTextReader xr)
            {
                RequiresName(xr);
                Type = xr["type"];

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
                if (Type == null) return null;

                Struct.Define st = parent.GetStruct(Type);
                if (st != null) return st;
                throw Abort("undefined struct: " + Type);
            }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                if (Value == null) return;

                Value.AddCodes(codes, m, "mov", null);
                if (HasThis)
                    Let.AddCodes(length, codes, m, GetAddress(codes, m, null));
                else
                    Let.AddCodes(length, codes, m, address);
            }
        }
    }
}
