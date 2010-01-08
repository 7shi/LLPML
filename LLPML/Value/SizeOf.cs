using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class SizeOf : IntValue
    {
        public SizeOf(BlockBase parent, string name) : base(parent, name) { }
        public SizeOf(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);
            RequiresName(xr);
        }

        public override int Value
        {
            get
            {
                int size = 0;
                var vd = parent.GetVar(name);
                if (vd != null)
                    size = vd.Type.Size;
                else
                    size = Types.GetType(parent, name).Size;
                if (size == 0) throw Abort("undefined type: " + name);
                return size;
            }
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
