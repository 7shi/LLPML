using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class VarInt : NodeBase
    {
        string name;
        public string Name { get { return name; } }

        Call call;
        int? value = null;

        public VarInt() { }
        public VarInt(Block parent, string name, int value) : base(parent)
        {
            this.name = name;
            this.value = value;
            parent.AddVarInt(name);
        }
        public VarInt(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            name = xr["name"];
            value = null;
            call = null;
            Parse(xr, delegate
            {
                if (xr.NodeType == XmlNodeType.Text)
                {
                    value = int.Parse(xr.Value);
                }
                else if (xr.NodeType == XmlNodeType.Element)
                {
                    switch (xr.Name)
                    {
                        case "call":
                            call = new Call(parent, xr);
                            break;
                        default:
                            throw Abort(xr);
                    }
                }
            });
            if (value != null || call != null) parent.AddVarInt(name);
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            if (value != null)
            {
                codes.Add(I386.Mov(Address, (uint)value));
            }
            else if (call != null)
            {
                call.AddCodes(codes, m);
                codes.Add(I386.Mov(Address, Reg32.EAX));
            }
        }

        public Addr32 Address { get { return parent.GetVarInt(name).addr; } }

        public class NoOperand : NodeBase
        {
            protected string target;

            public NoOperand() { }
            public NoOperand(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void Read(XmlTextReader xr)
            {
                target = xr["target"];
                Parse(xr, null);
            }
        }

        public class Inc : NoOperand
        {
            public Inc(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                codes.Add(I386.Inc(parent.GetVarInt(target).addr));
            }
        }

        public class Dec : NoOperand
        {
            public Dec(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                codes.Add(I386.Dec(parent.GetVarInt(target).addr));
            }
        }

        public class IntOperand : NodeBase
        {
            protected string target;
            protected object operand;

            public IntOperand() { }
            public IntOperand(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void Read(XmlTextReader xr)
            {
                target = xr["target"];
                Parse(xr, delegate
                {
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        switch (xr.Name)
                        {
                            case "int":
                                if (operand != null) throw Abort(xr);
                                operand = parent.ReadInt(xr);
                                break;
                            case "var-int":
                                if (operand != null) throw Abort(xr);
                                operand = parent.ReadVarInt(xr);
                                break;
                            default:
                                throw Abort(xr);
                        }
                    }
                });
                if (operand == null) throw Abort(xr);
            }
        }

        public class Add : IntOperand
        {
            public Add(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                Addr32 v = parent.GetVarInt(target).addr;
                if (operand is int)
                {
                    codes.Add(I386.Add(v, (uint)(int)operand));
                }
                else if (operand is Block.LocalVarInt)
                {
                    codes.Add(I386.Mov(Reg32.EAX, (operand as Block.LocalVarInt).addr));
                    codes.Add(I386.Add(v, Reg32.EAX));
                }
            }
        }

        public class Sub : IntOperand
        {
            public Sub(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                Addr32 v = parent.GetVarInt(target).addr;
                if (operand is int)
                {
                    codes.Add(I386.Sub(v, (uint)(int)operand));
                }
                else if (operand is Block.LocalVarInt)
                {
                    codes.Add(I386.Mov(Reg32.EAX, (operand as Block.LocalVarInt).addr));
                    codes.Add(I386.Sub(v, Reg32.EAX));
                }
            }
        }
    }
}
