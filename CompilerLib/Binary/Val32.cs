using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.Binary
{
    public class Val32
    {
        private uint value;

        private Val32 ref1, ref2;
        public Val32 Reference
        {
            get
            {
                if (ref1 != null && ref2 == null)
                    return ref1;
                else
                    return null;
            }

            set
            {
                ref1 = value;
                ref2 = null;
            }
        }

        public uint Value
        {
            get
            {
                if (ref1 == null) return value;
                if (ref2 == null) return ref1.Value;
                return ref1.Value + ref2.Value;
            }

            set
            {
                if (ref1 != null && ref2 == null)
                    ref1.Value = value;
                else
                {
                    this.value = value;
                    isInitialized = true;
                }
            }
        }

        private bool isInitialized = false;
        public bool IsInitialized
        {
            get
            {
                if (ref1 != null && ref2 == null)
                    return ref1.IsInitialized;
                else
                    return isInitialized;
            }
        }

        private bool isNeedForRelocation = false;
        public bool IsNeedForRelocation
        {
            get
            {
                if (ref1 != null && ref2 == null)
                    return ref1.IsNeedForRelocation;
                else
                    return isNeedForRelocation;
            }
        }

        //public static implicit operator Val32(uint v)
        //{
        //    return new Val32(v);
        //}

        public static Val32 New(uint v)
        {
            var ret = new Val32();
            ret.Value = v;
            return ret;
        }

        public static Val32 NewI(int v)
        {
            return New((uint)v);
        }

        public static Val32 NewB(uint v, bool reloc)
        {
            var ret = New(v);
            ret.isNeedForRelocation = reloc;
            return ret;
        }

        public static Val32 New2(Val32 a, Val32 b)
        {
            var ret = new Val32();
            ret.isInitialized = a.isInitialized;
            ret.isNeedForRelocation = a.isNeedForRelocation;
            ret.ref1 = a;
            ret.ref2 = b;
            return ret;
        }
    }
}
