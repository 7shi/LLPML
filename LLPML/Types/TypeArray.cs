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

        public int Count { get; protected set; }

        public TypeArray(TypeBase type, int count)
        {
            Type = type;
            Count = count;
        }
    }
}
