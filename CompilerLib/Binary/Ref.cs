using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.Binary
{
    public class Ref<T>
    {
        public T Value;
        public bool IsNeedForRelocation = false;

        public Ref()
        {
        }

        public Ref(T v)
        {
            Value = v;
        }

        public Ref(T v, bool reloc)
        {
            Value = v;
            IsNeedForRelocation = reloc;
        }

        public static implicit operator Ref<T>(T v)
        {
            return new Ref<T>(v);
        }
    }
}
