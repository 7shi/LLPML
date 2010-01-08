using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Member : Var
    {
        private VarBase target;
        public VarBase Target
        {
            get
            {
                return target;
            }

            set
            {
                target = value;
                isRoot = true;
            }
        }

        private bool isRoot = true;
        public Member Child { get; set; }

        public Member(BlockBase parent, string name)
            : base(parent)
        {
            isRoot = false;
            this.name = name;
        }

        public Member(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public Member(Member parent, XmlTextReader xr)
        {
            this.parent = parent.parent;
            isRoot = false;
            SetLine(xr);
            Read(xr);
        }

        public override void Read(XmlTextReader xr)
        {
            RequiresName(xr);

            Parse(xr, delegate
            {
                if ((!isRoot || target != null) &&
                    xr.NodeType == XmlNodeType.Element && xr.Name == "struct-member")
                {
                    if (Child != null)
                        throw Abort(xr, "multiple members");
                    Child = new Member(this, xr);
                    return;
                }

                var vs = IntValue.Read(parent, xr);
                if (vs == null) return;
                foreach (var v in vs)
                {
                    if (v is VarBase)
                    {
                        if (!isRoot)
                            throw Abort(xr, "needless instance");
                        else if (target != null)
                            throw Abort(xr, "too many operands");
                        target = v as VarBase;
                    }
                    else
                        throw Abort(xr, "invalid element");
                }
            });
            if (isRoot && target == null)
                target = new Var(parent, "this");
        }

        private Addr32 GetStructAddress(List<OpCode> codes, Module m)
        {
            var ad = target.GetAddress(codes, m);
            if (target is Pointer || target is Index) return ad;
            codes.Add(I386.Mov(Reg32.EDX, ad));
            return new Addr32(Reg32.EDX);
        }

        public int GetOffset(Define st)
        {
            int ret = st.GetOffset(name);
            if (ret < 0) throw Abort("undefined member: " + name);
            if (Child == null) return ret;
            return ret + Child.GetOffset(st.GetStruct(st.GetMember(name)));
        }

        public override Addr32 GetAddress(List<OpCode> codes, Module m)
        {
            var ret = new Addr32(GetStructAddress(codes, m));
            var st = target.GetStruct();
            ret.Add(GetOffset(st));
            return ret;
        }

        public Pointer.Declare GetPointer()
        {
            var st = target.GetStruct();
            if (st == null) return null;
            return st.GetMember(name);
        }

        public override Define GetStruct()
        {
            var st = target.GetStruct();
            if (st == null) return null;
            return st.GetStruct(st.GetMember(name));
        }

        public override bool IsArray
        {
            get
            {
                return GetPointer().Count > 0;
            }
        }

        public override string Type
        {
            get
            {
                var st = GetStruct();
                if (st == null) return null;
                return st.Name;
            }
        }

        public override int TypeSize
        {
            get
            {
                return GetPointer().TypeSize;
            }
        }

        public void Append(Struct.Member mem)
        {
            if (Child == null)
                Child = mem;
            else
                Child.Append(mem);
        }
    }
}
