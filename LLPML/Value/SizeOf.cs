using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class SizeOf : NodeBase, IIntValue
    {
        public SizeOf(BlockBase parent, string name) : base(parent, name) { }
        public SizeOf(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequiresName(xr);
        }

        public TypeBase Type { get { return TypeInt.Instance; } }

        public void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            int size = 0;
            var pd = parent.GetPointer(name);
            if (pd != null)
                size = pd.Length;
            else
                size = GetTypeSize(parent, name);
            if (size == 0) throw Abort("undefined type: " + name);
            codes.AddCodes(op, dest, (uint)size);
        }

        public static int GetTypeSize(BlockBase parent, string type)
        {
            if (type.StartsWith("var:")) return Var.DefaultSize;

            var sz = GetValueSize(type);
            if (sz > 0) return sz;

            var st = parent.GetStruct(type);
            if (st != null) return st.GetSize();

            return Var.DefaultSize;
        }

        public static int GetValueSize(string type)
        {
            switch (type)
            {
                case "byte":
                case "sbyte":
                    return 1;
                case "char":
                case "short":
                case "ushort":
                    return 2;
                case "int":
                case "uint":
                    return 4;
                //case "long":
                //case "ulong":
                //    return 8;
                case "var":
                    return Var.DefaultSize;
            }
            return 0;
        }
    }
}
