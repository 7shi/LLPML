using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.LLPML
{
    public class Types
    {
        public static TypeBase GetType(string type)
        {
            switch (type)
            {
                case "int":
                case "short":
                case "sbyte":
                    return TypeInt.Instance;
                case "uint":
                case "ushort":
                case "byte":
                    return TypeUInt.Instance;
                case null:
                    return TypeInt.Instance;
                default:
                    return null;
            }
        }
    }
}
