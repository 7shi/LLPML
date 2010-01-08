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

        void IIntValue.AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest)
        {
            int size = 0;
            var pd = parent.GetPointer(name);
            if (pd != null)
                size = pd.Length;
            else
                size = GetTypeSize(parent, name);
            if (size == 0) throw Abort("undefined type: " + name);
            IntValue.AddCodes(codes, op, dest, (uint)size);
        }

        public static int GetTypeSize(BlockBase parent, string type)
        {
            if (type.StartsWith("var:")) return Var.Size;

            switch (type)
            {
                case "byte":
                    return 1;
                case "char":
                case "short":
                    return 2;
                case "int":
                    return 4;
                case "long":
                    return 8;
                case "var":
                    return Var.Size;
            }

            var st = parent.GetStruct(type);
            if (st != null) return st.GetSize();

            return 0;
        }
    }
}
