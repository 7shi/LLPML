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
        public class Declare : Pointer.Declare
        {
            private IIntValue value;
            protected string type;
            public string Type { get { return type; } }
            public override int Length { get { return sizeof(int); } }

            public Declare() { }

            public Declare(BlockBase parent, string name)
                : base(parent, name)
            {
            }

            public Declare(BlockBase parent, string name, IIntValue value)
                : this(parent, name)
            {
                this.value = value;
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
                        if (v.Length > 1 || value != null)
                            throw Abort(xr, "multiple values");
                        value = v[0];
                    }
                });

                AddToParent();
            }

            protected override void AddToParent()
            {
                if (!parent.AddVar(this))
                    throw Abort("multiple definitions: " + name);
                base.AddToParent();
            }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                if (value != null)
                {
                    value.AddCodes(codes, m, "mov", address);
                }
            }

            public Struct.Define GetStruct()
            {
                if (type == null) return null;

                Struct.Define st = parent.GetStruct(type);
                if (st != null) return st;
                throw Abort("undefined struct: " + type);
            }
        }
    }
}
