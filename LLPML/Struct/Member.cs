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
        private IIntValue target;
        public IIntValue Target
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
            : base(parent.parent)
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

        protected Addr32 GetAddressInternal(OpCodes codes)
        {
            var st = GetTargetStruct();
            if (st == null)
                throw Abort("can not find member: {0}", name);
            var mem = st.GetMember(name);
            if (mem != null && mem.IsStatic)
                return new Addr32(mem.Address);

            Addr32 ret = null;
            TypeBase t = null;
            if (target is Member)
            {
                var tsm = target as Member;
                ret = tsm.GetAddressInternal(codes);
                t = tsm.TypeInternal;
            }
            else if (target is Var)
            {
                var tv = target as Var;
                ret = tv.GetAddress(codes);
                if (!(tv is Index)) t = tv.Type;
            }
            else
            {
                target.AddCodes(codes, "mov", null);
                codes.Add(I386.Mov(Var.DestRegister, Reg32.EAX));
                ret = new Addr32(Var.DestRegister);
            }

            if (t != null && t.IsValue)
            {
                codes.Add(I386.Mov(Var.DestRegister, ret));
                ret = new Addr32(Var.DestRegister);
            }
            if (mem != null)
            {
                ret.Add(st.GetOffset(name));
                return ret;
            }

            if (IsFunctionInternal)
                GetCall("").AddCodes(codes, "mov", null);
            else
                GetCall("get_").AddCodes(codes, "mov", null);
            return null;
        }

        public override Addr32 GetAddress(OpCodes codes)
        {
            if (Child != null)
                return Child.GetAddress(codes);
            else
                return GetAddressInternal(codes);
        }

        public Define GetTargetStruct()
        {
            if (target is Member)
                return (target as Member).GetStructInternal();
            else if (target != null)
                return Types.GetStruct(target.Type);
            else
                return parent.GetStruct(TargetType);
        }

        protected Define GetStructInternal()
        {
            var t = TypeInternal;
            if (t != null) return Types.GetStruct(t);
            return null;
        }

        public override Define GetStruct()
        {
            return Child != null ? Child.GetStruct() : GetStructInternal();
        }

        protected bool GetIsStatic()
        {
            if (target == null) return true;
            if (target is Member)
                return (target as Member).GetIsStatic();
            return false;
        }

        protected TypeBase TypeInternal
        {
            get
            {
                if (IsLengthInternal) return TypeVar.Instance;

                var st = GetTargetStruct();
                if (st == null) return null;

                var m = st.GetMember(name);
                if (m != null) return m.Type;

                var f = st.GetFunction(name);
                if (f != null)
                {
                    var delg = GetDelegate();
                    if (delg != null) return delg.Type;
                    return f.Type;
                }

                var g = st.GetFunction("get_" + name);
                if (g != null) return g.ReturnType;

                return null;
            }
        }
        public override TypeBase Type
        { get { return Child != null ? Child.Type : TypeInternal; } }

        public void Append(Member mem)
        {
            if (Child == null)
            {
                Child = mem;
                Child.target = this;
            }
            else
                Child.Append(mem);
        }

        public string GetName()
        {
            if (Child != null)
                return Child.GetName();
            return name;
        }

        public IIntValue GetTarget()
        {
            if (Child != null)
                return Child.GetTarget();
            return GetTargetInternal();
        }

        public IIntValue GetTargetInternal()
        {
            if (target is Member)
                return (target as Member).Duplicate();
            return target;
        }

        private bool flgDelg;
        private Delegate delg;

        private Delegate GetDelegate()
        {
            if (flgDelg) return delg;
            flgDelg = true;

            var st = GetTargetStruct();
            if (st == null) return null;

            var m = st.GetMember(name);
            if (m != null) return null;

            var f = st.GetFunction(name);
            if (f == null || GetIsStatic()) return null;

            delg = new Delegate(parent, f.CallType, new[] { target }, f);
            return delg;
        }

        public Function GetFunction()
        {
            if (Child == null)
            {
                var t = GetTargetStruct();
                if (t == null) return null;
                var ret = t.GetFunction(name);
                if (target is Base && ret.IsVirtual)
                    ret = t.GetFunction("override_" + name);
                return ret;
            }
            return Child.GetFunction();
        }

        protected Function GetFunction(string prefix)
        {
            var st = GetTargetStruct();
            if (st == null) return null;
            int of = st.GetOffset(name);
            if (of >= 0) return null;
            return st.GetFunction(prefix + name);
        }

        protected bool CheckFunction(string prefix)
        {
            return GetFunction(prefix) != null;
        }

        protected bool IsFunctionInternal { get { return CheckFunction(""); } }
        public bool IsFunction
        {
            get { return Child != null ? Child.IsFunction : IsFunctionInternal; }
        }

        protected bool IsSetterInternal { get { return CheckFunction("set_"); } }
        public bool IsSetter
        {
            get { return Child != null ? Child.IsSetter : IsSetterInternal; }
        }

        protected bool IsGetterInternal { get { return CheckFunction("get_"); } }
        public bool IsGetter
        {
            get { return Child != null ? Child.IsGetter : IsGetterInternal; }
        }

        protected bool IsLengthInternal
        {
            get
            {
                if (name != "Length") return false;

                var tr = target.Type as TypeReference;
                return tr != null && tr.IsArray;
            }
        }

        public Member Duplicate()
        {
            var m = new Member(parent, name);
            if (target is Member)
            {
                var t = (target as Member).Duplicate();
                t.Append(m);
                return t;
            }
            m.Target = target;
            return m;
        }

        protected Call GetCall(string prefix, params IIntValue[] args)
        {
            return new Call(parent, GetFunction(prefix), GetTargetInternal(), args);
        }

        protected void AddSetterCodesInternal(OpCodes codes, IIntValue arg)
        {
            GetCall("set_", arg).AddCodes(codes);
        }

        public void AddSetterCodes(OpCodes codes, IIntValue arg)
        {
            if (Child != null)
                Child.AddSetterCodes(codes, arg);
            else
                AddSetterCodesInternal(codes, arg);
        }

        protected void AddCodesInternal(OpCodes codes, string op, Addr32 dest)
        {
            if (IsLengthInternal)
            {
                target.AddCodes(codes, "mov", null);
                codes.Add(I386.Mov(Reg32.EAX, new Addr32(Reg32.EAX, -4)));
                codes.AddCodes(op, dest);
            }
            else if (IsGetterInternal)
                GetCall("get_").AddCodes(codes, op, dest);
            else if (IsFunctionInternal)
            {
                var delg = GetDelegate();
                if (delg != null)
                    delg.AddCodes(codes, op, dest);
                else
                {
                    var fp = new Function.Ptr(GetTargetStruct(), name);
                    fp.AddCodes(codes, op, dest);
                }
            }
            else
                TypeInternal.AddGetCodes(codes, op, dest, GetAddressInternal(codes));
        }

        public override void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            if (Child != null)
                Child.AddCodes(codes, op, dest);
            else
                AddCodesInternal(codes, op, dest);
        }
    }
}
