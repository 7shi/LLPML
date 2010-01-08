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
        private Var target;
        public Var Target
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
            :base(parent.parent)
        {
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
                    if (v is Var)
                    {
                        if (!isRoot)
                            throw Abort(xr, "needless instance");
                        else if (target != null)
                            throw Abort(xr, "too many operands");
                        target = v as Var;
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
            if (target is Var || target is Index) return ad;
            codes.Add(I386.Mov(Var.DestRegister, ad));
            return new Addr32(Var.DestRegister);
        }

        private Addr32 GetAddressInternal(OpCodes codes)
        {
            Addr32 ret;
            if (target is Struct.Member)
            {
                var tsm = target as Struct.Member;
                ret = tsm.GetAddressInternal(codes);
                if (tsm.GetMember().Type.IsValue)
                {
                    codes.Add(I386.Mov(Var.DestRegister, ret));
                    ret = new Addr32(Var.DestRegister);
                }
            }
            else
            {
                ret = target.GetAddress(codes);
                if (!(target is Index) && target.Type.IsValue)
                {
                    codes.Add(I386.Mov(Var.DestRegister, ret));
                    ret = new Addr32(Var.DestRegister);
                }
            }
            ret.Add(GetTargetStruct().GetOffset(name));
            return ret;
        }

        public override Addr32 GetAddress(OpCodes codes)
        {
            if (Child != null)
                return Child.GetAddress(codes);
            else
                return GetAddressInternal(codes);
        }

        public Var.Declare GetMember()
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
            return Types.GetStruct(st.GetMember(name).Type);
        }

        public override TypeBase Type
        {
            get
            {
                if (Child == null)
                    return GetMember().Type;
                return Child.Type;
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
            else if (this.target is Var)
                return this.target as Var;
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
