using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.Binary
{
    public class Ref<T>
    {
        private bool isInitialized = true;
        public bool IsInitialized { get { return isInitialized; } }

        public T Value;
        public bool IsNeedForRelocation = false;

        public Ref()
        {
            isInitialized = false;
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

        public void Set(Ref<T> src)
        {
            isInitialized = src.isInitialized;
            Value = src.Value;
            IsNeedForRelocation = src.IsNeedForRelocation;
        }
    }
}
