using System;
using System.Collections.Generic;
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
            codes.Add(I386.PushD(Val32.NewI(count)));
            codes.Add(I386.PushA(Addr32.NewRO(Reg32.ESP, 4)));
            codes.Add(loop);
            Type.AddConstructor(codes);
            codes.Add(I386.AddA(Addr32.New(Reg32.ESP), Val32.NewI(Type.Size)));
            codes.Add(I386.DecA(Addr32.NewRO(Reg32.ESP, 4)));
            codes.Add(I386.Jcc(Cc.NZ, loop.Address));
            codes.Add(I386.AddR(Reg32.ESP, Val32.New(8)));
        }

        // type destructor
        public override bool NeedsDtor { get { return Type.NeedsDtor; } }
        public override void AddDestructor(OpModule codes)
        {
            var count = Count;
            if (count == 0) return;

            var loop = new OpCode();
            codes.Add(I386.PushD(Val32.NewI(count)));
            codes.Add(I386.PushA(Addr32.NewRO(Reg32.ESP, 4)));
            codes.Add(I386.AddA(Addr32.New(Reg32.ESP), Val32.NewI(Size)));
            codes.Add(loop);
            codes.Add(I386.SubA(Addr32.New(Reg32.ESP), Val32.NewI(Type.Size)));
            Type.AddDestructor(codes);
            codes.Add(I386.DecA(Addr32.NewRO(Reg32.ESP, 4)));
            codes.Add(I386.Jcc(Cc.NZ, loop.Address));
            codes.Add(I386.AddR(Reg32.ESP, Val32.New(8)));
        }

        private NodeBase count;
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

        public TypeArray(TypeBase type, NodeBase count)
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
