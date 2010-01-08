using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Declare : Var.Declare
    {
        private List<object> values = new List<object>();
        public List<object> Values { get { return values; } }

        private bool isRoot = true;

        public override bool NeedsInit
        {
            get
            {
                if (values != null) return true;
                var st = GetStruct();
                return st != null && st.NeedsInit;
            }
        }

        public override bool NeedsCtor
        {
            get
            {
                var st = GetStruct();
                return st != null && st.NeedsCtor;
            }
        }

        public override bool NeedsDtor
        {
            get
            {
                var st = GetStruct();
                return st != null && st.NeedsDtor;
            }
        }

        public Declare(Declare parent)
        {
            this.parent = parent.parent;
            this.root = parent.root;
            isRoot = false;
        }

        public Declare(BlockBase parent, string name, string type)
            : base(parent, name)
        {
            this.type = Types.GetType(parent, type) as TypeStruct;
            if (this.type == null) throw Abort("type required");
        }

        public Declare(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public Declare(Declare parent, XmlTextReader xr)
            : this(parent)
        {
            SrcInfo = new Parsing.SrcInfo(root.Source, xr);
            Read(xr);
        }

        public override void Read(XmlTextReader xr)
        {
            if (isRoot)
            {
                RequiresName(xr);
                type = Types.GetType(parent, xr["type"]) as TypeStruct;
                if (type == null) throw Abort(xr, "type required");
                if (xr["static"] == "1") IsStatic = true;
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

        public override void AddCodes(OpCodes codes)
        {
            var st = GetStruct();
            if (!st.NeedsInit && values.Count == 0 && !st.NeedsCtor)
                return;
            codes.AddRange(new[]
            {
                I386.Lea(Reg32.EAX, GetAddress(codes, parent)),
                I386.Push(Reg32.EAX)
            });
            var ad = new Addr32(Reg32.ESP);
            st.AddInit(codes, ad);
            AddInitValues(codes, st);
            st.AddConstructor(codes, ad);
            codes.Add(I386.Add(Reg32.ESP, 4));
        }

        private bool AddInitValues(OpCodes codes, Define st)
        {
            if (values.Count == 0) return false;

            Var.Declare[] members = st.GetMembers();
            if (members.Length != values.Count)
                throw Abort("initializers mismatched: " + st.Name);

            var ad = new Addr32(Reg32.ESP);
            codes.Add(I386.Push(ad));
            for (int i = 0; i < values.Count; i++)
            {
                Var.Declare mem = members[i];
                object obj = values[i];
                if (obj is Declare)
                {
                    Define memst = Types.GetStruct(mem.Type);
                    if (!(mem is Declare) || memst == null)
                        throw Abort("struct required: " + mem.Name);
                    (obj as Declare).AddInitValues(codes, memst);
                }
                else if (obj is IIntValue)
                {
                    if (!(mem is Var.Declare))
                        throw Abort("value required: " + mem.Name);
                    (obj as IIntValue).AddCodes(codes, "mov", null);
                    codes.Add(I386.Mov(Var.DestRegister, ad));
                    mem.Type.AddSetCodes(codes, new Addr32(Var.DestRegister));
                }
                else
                    throw Abort("invalid parameter: " + mem.Name);
                codes.Add(I386.Add(ad, (uint)mem.Type.Size));
            }
            codes.Add(I386.Add(Reg32.ESP, 4));
            return true;
        }

        public void CheckField(Define st1, Define st2)
        {
            st2.MakeUp();
            if (st1 == st2)
                throw Abort(
                    "can not define recursive field: {0}",
                    st1.GetFullName(name));
            var b = st2.GetBaseStruct();
            if (b != null) CheckField(st1, b);
        }
    }
}
