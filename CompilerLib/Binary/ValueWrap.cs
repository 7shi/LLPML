using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.Binary
{
    public class ValueWrap
    {
        private uint value;
        public ValueWrap addition;

        public uint Value
        {
            get
            {
                if (addition == null) return value;
                return value + addition.Value;
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

        public ValueWrap() { }
        public ValueWrap(uint v) { Value = v; }
        public ValueWrap(uint v, ValueWrap add) : this(v) { addition = add; }
        public ValueWrap(uint v, bool reloc) : this(v) { IsNeedForRelocation = reloc; }

        public static implicit operator ValueWrap(uint v)
        {
            return new ValueWrap(v);
        }
    }
}
