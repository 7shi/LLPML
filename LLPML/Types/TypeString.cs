using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeString : TypeReference
    {
        // singleton
        private static TypeString instance = new TypeString();
        public static TypeString Instance { get { return instance; } }
        protected TypeString() : base(null, false) { }

        // functions
        public override Func GetFunc(string key)
        {
            var ret = Type.GetFunc(key);
            if (ret != null) return ret;

            if (key == "equal" || key == "not-equal")
                return TypeInt.Instance.GetFunc(key);
            return null;
        }

        // conditions
        public override CondPair GetCond(string key)
        {
            var ret = Type.GetCond(key);
            if (ret != null) return ret;

            if (key == "equal" || key == "not-equal")
                return TypeInt.Instance.GetCond(key);
            return null;
        }

        // recursive type
        private static Root root;
        public static Root Root
        {
            get { return root; }
            set
            {
                type = null;
                root = value;
            }
        }
        private static TypeBase type;
        public override TypeBase Type
        {
            get
            {
                if (type != null) return type;
                return type = Types.GetType(Root, "string");
            }
        }

        public override bool UseGC { get { return true; } }
    }

    public class TypeConstString : TypeString
    {
        // singleton
        private static TypeConstString instance = new TypeConstString();
        public static new TypeConstString Instance { get { return instance; } }
        private TypeConstString() : base() { }

        public override bool UseGC { get { return false; } }
    }
}
