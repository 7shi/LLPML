using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.Binary
{
    public class Val32
    {
        private uint value;
        public Val32 ref1, ref2;

        public uint Value
        {
            get
            {
                if (ref1 == null) return value;
                return ref1.Value + ref2.Value;
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

        public Val32() { }
        public Val32(uint v) { Value = v; }
        public Val32(uint v, bool reloc) : this(v) { IsNeedForRelocation = reloc; }

        public Val32(Val32 a, Val32 b)
        {
            isInitialized = a.isInitialized;
            IsNeedForRelocation = a.IsNeedForRelocation;
            ref1 = a;
            ref2 = b;
        }

        public static implicit operator Val32(uint v)
        {
            return new Val32(v);
        }
    }
}
