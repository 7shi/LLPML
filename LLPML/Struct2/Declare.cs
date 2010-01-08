using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct2
{
    public class Declare : Pointer.Declare
    {
        public override int Length { get { return GetStruct().GetSize(); } }

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
            SetLine(xr);
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
                if (xr.NodeType == XmlNodeType.Element && xr.Name == "struct2-declare")
                {
                    Declare d = new Declare(this, xr);
                    values.Add(d);
                }
                else
                {
                    IIntValue[] v = IntValue.Read(parent, xr);
                    if (v != null) values.AddRange(v);
                }
            });

            if (isRoot) AddToParent();
        }

        public Define GetStruct()
        {
            Define st = parent.GetStruct2(type);
            if (st != null) return st;
            throw Abort("undefined struct: " + type);
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            Define st = GetStruct();
            st.AddInitializer(codes, m, address);
            AddCodes(codes, m, st, new Addr32(address));
            st.AddConstructor(codes, m, address);
        }

        public void AddCodes(List<OpCode> codes, Module m, Define st, Addr32 ad)
        {
            if (values.Count == 0) return;

            Pointer.Declare[] members = st.GetMembers();
            if (members.Length != values.Count)
                throw Abort("can not initialize: " + st.Name);

            for (int i = 0; i < values.Count; i++)
            {
                Pointer.Declare mem = members[i];
                Define memst = st.GetStruct(mem);
                object obj = values[i];
                if (obj is Declare)
                {
                    if (memst == null)
                        throw Abort("value required: " + mem.Name);
                    (obj as Declare).AddCodes(codes, m, memst, ad);
                }
                else if (obj is IIntValue)
                {
                    if (memst != null)
                        throw Abort("struct required: " + mem.Name);
                    (obj as IIntValue).AddCodes(codes, m, "mov", new Addr32(ad));
                    ad.Add(mem.Length);
                }
                else
                {
                    throw Abort("invalid parameter: " + mem.Name);
                }
            }
        }
    }
}
