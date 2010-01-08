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
        public override int Length { get { return GetStruct().GetSize(); } }

        public string Type { get { return type; } }

        private List<object> values = new List<object>();
        public List<object> Values { get { return values; } }

        private bool isRoot = true;

        public Declare(Declare parent)
        {
            this.parent = parent.parent;
            isRoot = false;
        }

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
            : this(parent)
        {
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
                if (xr.NodeType == XmlNodeType.Element && xr.Name == "struct-declare")
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
            Define st = parent.GetStruct(type);
            if (st != null) return st;
            throw Abort("undefined struct: " + type);
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            Define st = GetStruct();
            Addr32 ad = GetAddress(codes, m, parent);
            if (AddInitValues(codes, m, st, ad))
                ad = GetAddress(codes, m, parent);
            st.AddConstructor(codes, m, ad);
        }

        private bool AddInitValues(List<OpCode> codes, Module m, Define st, Addr32 ad)
        {
            if (values.Count == 0) return false;

            Pointer.Declare[] members = st.GetMembers();
            if (members.Length != values.Count)
                throw Abort("initializers mismatched: " + st.Name);

            for (int i = 0; i < values.Count; i++)
            {
                Pointer.Declare mem = members[i];
                object obj = values[i];
                if (obj is Declare)
                {
                    Define memst = st.GetStruct(mem);
                    if (!(mem is Declare) || memst == null)
                        throw Abort("struct required: " + mem.Name);
                    (obj as Declare).AddInitValues(codes, m, memst, ad);
                }
                else if (obj is IIntValue)
                {
                    if (!(mem is Var.Declare))
                        throw Abort("value required: " + mem.Name);
                    (obj as IIntValue).AddCodes(codes, m, "mov", new Addr32(ad));
                    ad.Add(mem.Length);
                }
                else
                {
                    throw Abort("invalid parameter: " + mem.Name);
                }
            }
            return true;
        }
    }
}
