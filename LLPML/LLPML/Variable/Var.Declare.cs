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

            public Declare(Block parent, string name)
                : base(parent, name)
            {
                parent.AddVar(this);
            }

            public Declare(Block parent, string name, IIntValue value)
                : this(parent, name)
            {
                this.value = value;
            }

            public Declare(Block parent, string name, int value)
                : this(parent, name, new IntValue(value))
            {
            }

            public Declare(Block parent, XmlTextReader xr)
                : base(parent, xr)
            {
            }

            public override void Read(XmlTextReader xr)
            {
                RequiresName(xr);
                type = xr["type"];

                Parse(xr, delegate
                {
                    IIntValue v = IntValue.Read(parent, xr, true);
                    if (v != null)
                    {
                        if (value != null) throw Abort(xr, "multiple values");
                        value = v;
                    }
                });

                parent.AddVar(this);
                parent.AddPointer(this);
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
                throw new Exception("undefined struct: " + type);
            }
        }
    }
}
