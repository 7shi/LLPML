using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeArray : TypeBase
    {
        // type name
        public override string Name { get { return Type.Name + "[" + Count + "]"; } }

        // type size
        public override int Size { get { return Type.Size * Count; } }

        // check array
        public override bool IsArray { get { return true; } }

        // check value
        public override bool IsValue { get { return false; } }

        // cast
        public override TypeBase Cast(TypeBase type)
        {
            if (type is TypeVar) return type;
            if (!(type is TypeIterator)) return null;
            return base.Cast(type);
        }

        // type constructor
        public override bool NeedsCtor { get { return Type.NeedsCtor; } }
        public override void AddConstructor(OpCodes codes, Addr32 ad)
        {
            codes.Add(I386.Push(Reg32.EDI));
            if (ad == null)
                codes.Add(I386.Mov(Reg32.EDI, Reg32.EAX));
            else
                codes.Add(I386.Lea(Reg32.EDI, ad));
            var loop = new OpCode();
            codes.AddRange(new[]
            {
                I386.Mov(Reg32.ECX, (uint)Count),
                loop,
                I386.Push(Reg32.ECX),
            });
            Type.AddConstructor(codes, new Addr32(Reg32.EDI));
            codes.AddRange(new[]
            {
                I386.Add(Reg32.EDI, (uint)Type.Size),
                I386.Pop(Reg32.ECX),
                I386.Loop(loop.Address),
                I386.Pop(Reg32.EDI),
            });
        }

        // type destructor
        public override bool NeedsDtor { get { return Type.NeedsDtor; } }
        public override void AddDestructor(OpCodes codes, Addr32 ad)
        {
            codes.Add(I386.Push(Reg32.EDI));
            if (ad == null)
                codes.Add(I386.Mov(Reg32.EDI, Reg32.EAX));
            else
                codes.Add(I386.Lea(Reg32.EDI, ad));
            var loop = new OpCode();
            codes.AddRange(new[]
            {
                I386.Add(Reg32.EDI, (uint)Size),
                I386.Mov(Reg32.ECX, (uint)Count),
                loop,
                I386.Push(Reg32.ECX),
                I386.Sub(Reg32.EDI, (uint)Type.Size),
            });
            Type.AddDestructor(codes, new Addr32(Reg32.EDI));
            codes.AddRange(new[]
            {
                I386.Pop(Reg32.ECX),
                I386.Loop(loop.Address),
                I386.Pop(Reg32.EDI),
            });
        }

        public int Count { get; protected set; }

        public TypeArray(TypeBase type, int count)
        {
            Type = type;
            Count = count;
        }
    }
}
