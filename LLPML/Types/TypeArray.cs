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
            if (!(type is TypePointer)) return null;
            return base.Cast(type);
        }

        // type constructor
        public override bool NeedsCtor { get { return Type.NeedsCtor; } }
        public override void AddConstructor(OpModule codes)
        {
            var count = Count;
            if (count == 0) return;

            var loop = new OpCode();
            codes.AddRange(new[]
            {
                I386.Push((uint)count),
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
        public override void AddDestructor(OpModule codes)
        {
            var count = Count;
            if (count == 0) return;

            var loop = new OpCode();
            codes.AddRange(new[]
            {
                I386.Push((uint)count),
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

        private IIntValue count;
        public int Count
        {
            get
            {
                var v = IntValue.GetValue(count);
                if (v == null)
                {
                    var nb = count as NodeBase;
                    if (nb != null)
                        throw nb.Abort("配列のサイズが定数ではありません。");
                    else
                        throw new Exception("配列のサイズが定数ではありません。");
                }
                return v.Value;
            }
        }

        public TypeArray(TypeBase type, IIntValue count)
        {
            Type = type;
            this.count = count;
        }

        public TypeArray(TypeBase type, int count)
            : this(type, new IntValue(count))
        {
        }
    }
}
