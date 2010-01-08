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
        public override void AddConstructor(OpCodes codes)
        {
            if (Count == 0) return;

            var loop = new OpCode();
            codes.AddRange(new[]
            {
                I386.Push((uint)Count),
                I386.Push(new Addr32(Reg32.ESP, 4)),
                loop,
            });
            Type.AddConstructor(codes);
            codes.AddRange(new[]
            {
                I386.Add(new Addr32(Reg32.ESP), (uint)Type.Size),
                I386.Dec(new Addr32(Reg32.ESP, 4)),
                I386.Jcc(Cc.NZ, loop.Address),
                I386.Add(Reg32.ESP, 8),
            });
        }

        // type destructor
        public override bool NeedsDtor { get { return Type.NeedsDtor; } }
        public override void AddDestructor(OpCodes codes)
        {
            if (Count == 0) return;

            var loop = new OpCode();
            codes.AddRange(new[]
            {
                I386.Push((uint)Count),
                I386.Push(new Addr32(Reg32.ESP, 4)),
                I386.Add(new Addr32(Reg32.ESP), (uint)Size),
                loop,
                I386.Sub(new Addr32(Reg32.ESP), (uint)Type.Size),
            });
            Type.AddDestructor(codes);
            codes.AddRange(new[]
            {
                I386.Dec(new Addr32(Reg32.ESP, 4)),
                I386.Jcc(Cc.NZ, loop.Address),
                I386.Add(Reg32.ESP, 8),
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
