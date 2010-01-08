using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Declare : Pointer.Declare
    {
        public override int Length
        {
            get
            {
                Struct.Define st = parent.GetStruct(type);
                if (st == null) throw new Exception("undefined struct: " + type);
                return st.GetSize();
            }
        }

        private string type;
        private List<IIntValue> values = new List<IIntValue>();

        public Declare() { }

        public Declare(Block parent, string name, string type)
            : base(parent, name)
        {
            this.type = type;
        }

        public Declare(Block parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            RequiresName(xr);

            type = xr["type"];
            if (type == null) throw Abort(xr, "type required");

            Parse(xr, delegate
            {
                IIntValue v = IntValue.Read(parent, xr, true);
                if (v != null) values.Add(v);
            });

            parent.AddPointer(this);
        }

        public Struct.Define GetStruct()
        {
            Struct.Define st = parent.GetStruct(type);
            if (st == null) throw new Exception("undefined struct: " + type);
            return st;
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            Struct.Define st = GetStruct();
            if (values.Count == 0) return;
            if (st.GetSize() != values.Count * 4)
                throw new Exception("can not initialize");

            Addr32 ad = new Addr32(address);
            foreach (IIntValue v in values)
            {
                v.AddCodes(codes, m, "mov", new Addr32(ad));
                ad.Add(4);
            }
        }
    }
}
