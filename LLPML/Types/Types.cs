using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.LLPML
{
    public class Types
    {
        public static TypeBase GetType(BlockBase parent, string type)
        {
            if (type == null) return null;
            if (type.StartsWith("var:"))
                return new TypeReference(GetType(parent, type.Substring(4)));
            else if (type.EndsWith("[]"))
            {
                var t = type.Substring(0, type.Length - 2).TrimEnd();
                return new TypeIterator(GetType(parent, t));
            }
            else if (type.EndsWith("]"))
            {
                var p = type.IndexOf('[');
                var t = GetType(parent, type.Substring(0, p));
                var n = type.Substring(p + 1, type.Length - p - 2);
                return new TypeArray(t, int.Parse(n));
            }
            var ret = Types.GetValueType(type);
            if (ret != null) return ret;
            return new TypeStruct(parent, type);
        }

        public static TypeBase GetValueType(string type)
        {
            switch (type)
            {
                case "var":
                    return TypeVar.Instance;
                case "bool":
                    return TypeBool.Instance;
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

        public static TypeBase ConvertVarType(TypeBase t)
        {
            if (t == null)
                return TypeVar.Instance;
            if (t is TypeStruct)
                return new TypeReference(t);
            else if (t is TypeArray)
                return new TypeIterator(t.Type);
            else
                return t;
        }

        public static TypeBase GetVarType(BlockBase parent, string type)
        {
            return ConvertVarType(GetType(parent, type));
        }

        public static Struct.Define GetStruct(TypeBase t)
        {
            if (t == null)
                return null;
            if (t is TypeStruct)
                return (t as TypeStruct).GetStruct();
            else
                return GetStruct(t.Type);
        }

        public static TypeBase Cast(TypeBase t1, TypeBase t2)
        {
            if (t1 == null) return t2;
            if (t2 == null) return t1;

            var c1 = t1.Cast(t2);
            if (c1 != null) return c1;

            return t2.Cast(t1);
        }
    }
}
