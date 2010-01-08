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

            private TypeBase type;
            public override TypeBase Type { get { return type; } }

            public override string TypeName
            {
                set
                {
                    if (value != null && value.EndsWith("[]"))
                    {
                        var t = value.Substring(0, value.Length - 2).TrimEnd();
                        base.TypeName = t;
                        length = Var.DefaultSize;
                        IsArray = true;
                        type = new TypePointer(t, SizeOf.GetTypeSize(parent, t));
                    }
                    else
                    {
                        base.TypeName = value;
                        length = SizeOf.GetValueSize(value);
                        if (length == 0) length = Var.DefaultSize;
                        IsArray = false;
                        type = Types.GetType(value);
                        if (type == null)
                            type = new TypePointer(value, length);
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
                this.TypeName = type;
            }

            public Declare(BlockBase parent, XmlTextReader xr)
                : base(parent, xr)
            {
            }

            public override void Read(XmlTextReader xr)
            {
                RequiresName(xr);
                TypeName = xr["type"];

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
                if (TypeName == null) return null;

                Struct.Define st = parent.GetStruct(TypeName);
                if (st != null) return st;
                throw Abort("undefined struct: " + TypeName);
            }

            public override void AddCodes(OpCodes codes)
            {
                if (Value == null) return;

                Value.AddCodes(codes, "mov", null);
                if (HasThis)
                    Set.AddCodes(length, codes, GetAddress(codes, null));
                else
                    Set.AddCodes(length, codes, address);
            }
        }
    }
}
