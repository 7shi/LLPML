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
            private IIntValue value;
            protected string type;
            protected Var thisptr;

            public string Type { get { return type; } }
            public override int Length { get { return Var.Size; } }

            private void Init()
            {
                if (parent is Struct2.Define && !(this is Arg))
                    thisptr = new Struct2.This(parent);
            }

            public Declare() { }

            public Declare(BlockBase parent, string name)
                : base(parent, name)
            {
                Init();
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

                Init();
                AddToParent();
            }

            protected override void AddToParent()
            {
                if (!parent.AddVar(this))
                    throw Abort("multiple definitions: " + name);
            }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                if (value != null)
                {
                    if (HasThis)
                    {
                        value.AddCodes(codes, m, "mov", null);
                        codes.Add(I386.Mov(GetAddress(codes, m), Reg32.EAX));
                    }
                    else
                    {
                        value.AddCodes(codes, m, "mov", address);
                    }
                }
            }

            public Struct.Define GetStruct()
            {
                if (type == null) return null;

                Struct.Define st = parent.GetStruct(type);
                if (st != null) return st;
                throw Abort("undefined struct: " + type);
            }

            public Struct2.Define GetStruct2()
            {
                if (type == null) return null;

                Struct2.Define st = parent.GetStruct2(type);
                if (st != null) return st;
                throw Abort("undefined struct: " + type);
            }

            public bool HasThis
            {
                get
                {
                    return thisptr != null;
                }
            }

            public Addr32 GetAddress(List<OpCode> codes, Module m)
            {
                codes.Add(I386.Mov(Reg32.EDX, thisptr.GetAddress(codes, m)));
                return address;
            }
        }
    }
}
