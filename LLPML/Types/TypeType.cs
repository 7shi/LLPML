using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeType : TypeReference
    {
        // singleton
        private static TypeType instance = new TypeType();
        public static TypeType Instance { get { return instance; } }
        protected TypeType() : base(null, false) { }

        // recursive type
        private static TypeBase type;
        public static void Init() { type = null; }
        public override TypeBase Type
        {
            get
            {
                if (type != null) return type;
                return type = Types.GetType("Type");
            }
        }
    }
}
