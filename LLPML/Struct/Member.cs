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

        public string TargetType { get; set; }

        private bool isRoot = true;
        public Member Child { get; protected set; }

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
            this.root = parent.root;
            isRoot = false;
            SrcInfo = new Parsing.SrcInfo(root.Source, xr);
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
                    Append(new Member(this, xr));
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

        private Addr32 GetStructAddress(OpCodes codes)
        {
            var ad = target.GetAddress(codes);
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

        public override Addr32 GetAddress(OpCodes codes)
        {
            var ret = new Addr32(GetStructAddress(codes));
            var st = GetTargetStruct();
            ret.Add(GetOffset(st));
            return ret;
        }

        public Pointer.Declare GetPointer()
        {
            var st = GetTargetStruct();
            if (st == null)
                throw Abort("undefined member: {0}", name);
            return st.GetMember(name);
        }

        public Define GetTargetStruct()
        {
            if (target != null) return target.GetStruct();
            return parent.GetStruct(TargetType);
        }

        public override Define GetStruct()
        {
            var st = GetTargetStruct();
            if (st == null) return null;
            return st.GetStruct(st.GetMember(name));
        }

        public override bool IsArray
        {
            get
            {
                var p = GetPointer();
                if (p is Var.Declare)
                    return (p as Var.Declare).IsArray;
                return p.Count > 0;
            }
        }

        public override TypeBase Type
        {
            get
            {
                if (Child == null)
                    return GetPointer().Type;
                return Child.Type;
            }
        }

        public override int TypeSize
        {
            get
            {
                if (Child == null)
                    return GetPointer().TypeSize;
                return Child.TypeSize;
            }
        }

        public override string TypeName
        {
            get
            {
                if (Child == null)
                {
                    var p = GetPointer();
                    if (p is Var.Declare) return p.TypeName;
                    var st = GetStruct();
                    if (st == null) return null;
                    return st.Name;
                }
                return Child.TypeName;
            }
        }

        public override int Size
        {
            get
            {
                if (Child == null)
                {
                    var p = GetPointer();
                    if (p is Var.Declare) return p.Length;
                    return Var.DefaultSize;
                }
                return Child.Size;
            }
        }

        public void Append(Struct.Member mem)
        {
            if (Child == null)
            {
                Child = mem;
                Child.target = this;
            }
            else
                Child.Append(mem);
        }

        private bool CheckFunction(string prefix)
        {
            var st = GetTargetStruct();
            int ret = st.GetOffset(name);
            if (ret >= 0) return false;
            var f = st.GetFunction(prefix + name);
            return f != null;
        }

        private IIntValue ConvertTarget()
        {
            if (this.target is Var)
                return this.target as Var;
            else if (this.target is Pointer)
                return this.target as Pointer;
            return null;
        }

        public bool IsFunction
        {
            get
            {
                if (Child == null)
                    return CheckFunction("");
                return Child.IsFunction;
            }
        }

        public bool IsSetter
        {
            get
            {
                if (Child == null)
                    return CheckFunction("set_");
                return Child.IsSetter;
            }
        }

        public bool IsGetter
        {
            get
            {
                if (Child == null)
                    return CheckFunction("get_");
                return Child.IsGetter;
            }
        }

        public void AddSetterCodes(OpCodes codes, IIntValue arg)
        {
            var setter = new Call(parent, "set_" + name, ConvertTarget(), arg);
            setter.AddCodes(codes);
        }

        public override void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            if (IsGetter)
            {
                var getter = new Call(parent, "get_" + name, ConvertTarget());
                getter.AddCodes(codes, op, dest);
            }
            else if (IsFunction)
            {
                var fp = new Function.Ptr(GetTargetStruct(), name);
                fp.AddCodes(codes, op, dest);
            }
            else
                base.AddCodes(codes, op, dest);
        }
    }
}
