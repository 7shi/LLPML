using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class New : NodeBase, IIntValue
    {
        public const string Function = "__operator_new";
        public TypeBase Type { get; protected set; }
        public bool IsArray { get; protected set; }
        public IIntValue Length { get; protected set; }

        public New(BlockBase parent, string type)
            : base(parent)
        {
            Type = Types.GetVarType(parent, type);
        }

        public New(BlockBase parent, string type, IIntValue length)
            : base(parent)
        {
            IsArray = true;
            Length = length;
            Type = new TypeIterator(Types.GetType(parent, type));
        }

        public New(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);

            var type = xr["type"];
            var length = xr["length"];
            if (length == null)
                Type = Types.GetVarType(parent, type);
            else
            {
                IsArray = true;
                Length = new IntValue(int.Parse(length));
                Type = new TypeIterator(Types.GetType(parent, type));
            }
        }

        public override void AddCodes(OpCodes codes)
        {
            AddCodes(codes, "mov", null);
        }

        public void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            var tt = Type.Type;
            var f = parent.GetFunction(Function);
            if (f == null)
                throw Abort("new: undefined function: {0}", Function);
            int size = tt.Size;
            if (!IsArray)
            {
                codes.AddRange(new[]
                {
                    I386.Push((uint)size),
                    I386.Call(f.First),
                });
                if (f.CallType == CallType.CDecl)
                    codes.Add(I386.Add(Reg32.ESP, 4));
                codes.Add(I386.Push(Reg32.EAX));
                if (tt.NeedsCtor)
                    tt.AddConstructor(codes);
            }
            else if (Length is IntValue)
            {
                int len = (Length as IntValue).Value;
                codes.AddRange(new[]
                {
                    I386.Push((uint)(size * len)),
                    I386.Call(f.First),
                });
                if (f.CallType == CallType.CDecl)
                    codes.Add(I386.Add(Reg32.ESP, 4));
                if (len == 0 || !tt.NeedsCtor)
                {
                    codes.AddCodes(op, dest);
                    return;
                }
                var label = new OpCode();
                codes.AddRange(new[]
                {
                    I386.Push(Reg32.EAX),
                    I386.Push((uint)len),
                    I386.Push(Reg32.EAX),
                    label,
                });
                tt.AddConstructor(codes);
                codes.AddRange(new[]
                {
                    I386.Add(new Addr32(Reg32.ESP), (uint)size),
                    I386.Dec(new Addr32(Reg32.ESP, 4)),
                    I386.Jcc(Cc.NZ, label.Address),
                    I386.Add(Reg32.ESP, 8),
                });
            }
            else
            {
                Length.AddCodes(codes, "push", null);
                codes.AddRange(new[]
                {
                    I386.Mov(Reg32.EAX, (uint)size),
                    I386.Mul(new Addr32(Reg32.ESP)),
                    I386.Push(Reg32.EAX),
                    I386.Call(f.First),
                });
                if (!tt.NeedsCtor)
                {
                    if (f.CallType == CallType.CDecl)
                        codes.Add(I386.Add(Reg32.ESP, 8));
                    else
                        codes.Add(I386.Add(Reg32.ESP, 4));
                    codes.AddCodes(op, dest);
                    return;
                }
                if (f.CallType == CallType.CDecl)
                    codes.Add(I386.Add(Reg32.ESP, 4));
                var label1 = new OpCode();
                var label2 = new OpCode();
                codes.AddRange(new[]
                {
                    I386.Pop(Reg32.EDX),
                    I386.Push(Reg32.EAX),
                    I386.Test(Reg32.EDX, Reg32.EDX),
                    I386.Jcc(Cc.Z, label2.Address),
                    I386.Push(Reg32.EDX),
                    I386.Push(Reg32.EAX),
                    label1,
                });
                tt.AddConstructor(codes);
                codes.AddRange(new[]
                {
                    I386.Add(new Addr32(Reg32.ESP), (uint)size),
                    I386.Dec(new Addr32(Reg32.ESP, 4)),
                    I386.Jcc(Cc.NZ, label1.Address),
                    I386.Add(Reg32.ESP, 8),
                    label2,
                });
            }
            if (op == "push" && dest == null) return;
            codes.Add(I386.Pop(Reg32.EAX));
            codes.AddCodes(op, dest);
        }
    }
}
