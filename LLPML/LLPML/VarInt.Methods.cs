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
        public class NoOperand : NodeBase
        {
            protected string name;

            public NoOperand() { }
            public NoOperand(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void Read(XmlTextReader xr)
            {
                if (!xr.IsEmptyElement)
                    throw Abort(xr, "<" + xr.Name + "> can not have any children");

                name = xr["name"];
            }
        }

        public class Inc : NoOperand
        {
            public Inc(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                codes.Add(I386.Inc(parent.GetVarInt(name).Address));
            }
        }

        public class Dec : NoOperand
        {
            public Dec(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                codes.Add(I386.Dec(parent.GetVarInt(name).Address));
            }
        }

        public class IntOperand : NodeBase
        {
            protected string name;
            protected object operand;

            public IntOperand() { }
            public IntOperand(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void Read(XmlTextReader xr)
            {
                name = xr["name"];
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
                                operand = new VarInt(parent, xr);
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
                Addr32 dest = parent.GetVarInt(name).Address;
                if (operand is int)
                {
                    codes.Add(I386.Add(dest, (uint)(int)operand));
                }
                else if (operand is VarInt)
                {
                    codes.Add(I386.Mov(Reg32.EAX, (operand as VarInt).GetAddress(codes, m)));
                    codes.Add(I386.Add(dest, Reg32.EAX));
                }
            }
        }

        public class Sub : IntOperand
        {
            public Sub(Block parent, XmlTextReader xr) : base(parent, xr) { }

            public override void AddCodes(List<OpCode> codes, Module m)
            {
                Addr32 dest = parent.GetVarInt(name).Address;
                if (operand is int)
                {
                    codes.Add(I386.Sub(dest, (uint)(int)operand));
                }
                else if (operand is VarInt)
                {
                    codes.Add(I386.Mov(Reg32.EAX, (operand as VarInt).GetAddress(codes, m)));
                    codes.Add(I386.Sub(dest, Reg32.EAX));
                }
            }
        }
    }
}
