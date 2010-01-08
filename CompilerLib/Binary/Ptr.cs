using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.Binary
{
    public class Ptr<T>
    {
        private T value;
        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                isInitialized = true;
            }
        }

        private bool isInitialized = false;
        public bool IsInitialized { get { return isInitialized; } }

        public bool IsNeedForRelocation = false;

        public Ptr() { }
        public Ptr(T v) { Value = v; }
        public Ptr(T v, bool reloc) : this(v) { IsNeedForRelocation = reloc; }

        public static implicit operator Ptr<T>(T v)
        {
            return new Ptr<T>(v);
        }
    }
}
