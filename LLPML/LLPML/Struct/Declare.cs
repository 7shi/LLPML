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
        public string Type { get { return type; } }

        private List<object> values = new List<object>();
        private bool isRoot = true;

        public Declare() { }

        public Declare(BlockBase parent, string name, string type)
            : base(parent, name)
        {
            this.type = type;
        }

        public Declare(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public Declare(Declare parent, XmlTextReader xr)
        {
            this.parent = parent.parent;
            isRoot = false;
            Read(xr);
        }

        public override void Read(XmlTextReader xr)
        {
            if (isRoot)
            {
                RequiresName(xr);
                type = xr["type"];
                if (type == null) throw Abort(xr, "type required");
            }

            Parse(xr, delegate
            {
                if (xr.NodeType == XmlNodeType.Element && xr.Name == "struct-declare")
                {
                    Declare d = new Declare(this, xr);
                    values.Add(d);
                }
                else
                {
                    IIntValue v = IntValue.Read(parent, xr, false);
                    if (v != null) values.Add(v);
                }
            });

            if (isRoot) parent.AddPointer(this);
        }

        public Struct.Define GetStruct()
        {
            Struct.Define st = parent.GetStruct(type);
            if (st != null) return st;
            throw new Exception("undefined struct: " + type);
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            AddCodes(codes, m, GetStruct(), new Addr32(address));
        }

        public void AddCodes(List<OpCode> codes, Module m, Struct.Define st, Addr32 ad)
        {
            Addr32 ad2 = new Addr32(ad);
            if (values.Count > 0)
            {
                if (st.Members.Count != values.Count)
                    throw new Exception("can not initialize: " + st.Name);

                for (int i = 0; i < values.Count; i++)
                {
                    Define.Member mem = st.Members[i];
                    Define memst = mem.GetStruct();
                    object obj = values[i];
                    if (obj is Declare)
                    {
                        if (memst == null)
                            throw new Exception("value required: " + mem.Name);
                        (obj as Declare).AddCodes(codes, m, memst, ad);
                    }
                    else if (obj is IIntValue)
                    {
                        if (memst != null)
                            throw new Exception("struct required: " + mem.Name);
                        (obj as IIntValue).AddCodes(codes, m, "mov", new Addr32(ad));
                        ad.Add(mem.GetSize());
                    }
                    else
                    {
                        throw new Exception("invalid parameter: " + mem.Name);
                    }
                }
            }
            st.AddConstructor(codes, m, ad2);
        }
    }
}
