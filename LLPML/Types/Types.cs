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
                case "var":
                case "int":
                    return TypeInt.Instance;
                case "short":
                    return TypeShort.Instance;
                case "sbyte":
                    return TypeSByte.Instance;
                case "uint":
                    return TypeUInt.Instance;
                case "ushort":
                    return TypeUShort.Instance;
                case "byte":
                    return TypeByte.Instance;
                case "char":
                    return TypeChar.Instance;
                default:
                    return null;
            }
        }
    }
}
