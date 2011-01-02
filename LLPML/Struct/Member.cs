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
        private NodeBase target;
        public NodeBase Target
        {
            get
            {
                if (target == null && !string.IsNullOrEmpty(TargetType))
                {
                    var t = Variant.GetTarget(Parent, TargetType);
                    if (t != null)
                    {
                        target = t;
                        TargetType = null;
                    }
                }
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
            : base(parent.Parent)
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

                var vs = IntValue.Read(Parent, xr);
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
                target = new Var(Parent, "this");
        }

        protected Addr32 GetAddressInternal(OpModule codes)
        {
            var st = GetTargetStruct();
            if (st == null)
                throw Abort("can not find member: {0}", name);

            var mem = st.GetMember(name);
            if (mem != null && mem.IsStatic)
                return Addr32.NewAd(mem.Address);

            Addr32 ret = null;
            TypeBase t = null;
            var target = Target;
            if (target is Member)
            {
                var tsm = target as Member;
                ret = tsm.GetAddressInternal(codes);
                if (ret != null)
                    t = tsm.TypeInternal;
                else
                {
                    tsm.AddCodesInternal(codes, "mov", null);
                    codes.Add(I386.Mov(Var.DestRegister, Reg32.EAX));
                    ret = Addr32.New(Var.DestRegister);
                }
            }
            else if (target is Var)
            {
                var tv = Var.Get(target);
                ret = tv.GetAddress(codes);
                if (!(tv is Index)) t = tv.Type;
            }
            else if (target != null)
            {
                target.AddCodes(codes, "mov", null);
                codes.Add(I386.Mov(Var.DestRegister, Reg32.EAX));
                ret = Addr32.New(Var.DestRegister);
            }
            else
            {
                var g = Parent.GetFunction("get_" + TargetType);
                if (g != null)
                {
                    new Call(Parent, g.Name).AddCodes(codes, "mov", null);
                    if (mem != null)
                    {
                        codes.Add(I386.AddR(Reg32.EAX, Val32.NewI(st.GetOffset(name))));
                        codes.Add(I386.Mov(Var.DestRegister, Reg32.EAX));
                        return Addr32.New(Var.DestRegister);
                    }
                    if (IsFunctionInternal)
                    {
                        // TODO: Delegate
                    }
                    else if (IsGetterInternal)
                    {
                        var gg = GetFunction("get_");
                        codes.Add(I386.Push(Reg32.EAX));
                        codes.Add(I386.CallD(gg.First));
                        if (gg.CallType == CallType.CDecl)
                            codes.Add(I386.AddR(Reg32.ESP, Val32.New(4)));
                        return null;
                    }
                    throw Abort("undefined member: {0}.{1}", st.FullName, name);
                }
            }

            if (t != null && t.IsValue)
            {
                codes.Add(I386.MovRA(Var.DestRegister, ret));
                ret = Addr32.New(Var.DestRegister);
            }
            if (mem != null)
            {
                if (ret == null)
                    throw Abort("not static: {0}.{1}", st.FullName, name);
                ret.Add(st.GetOffset(name));
                return ret;
            }

            if (IsFunctionInternal)
            {
                // TODO: Delegate
            }
            else if (IsGetterInternal)
            {
                GetCall("get_").AddCodes(codes, "mov", null);
                return null;
            }

            throw Abort("undefined member: {0}.{1}", st.FullName, name);
        }

        public override Addr32 GetAddress(OpModule codes)
        {
            if (Child != null)
                return Child.GetAddress(codes);
            else
                return GetAddressInternal(codes);
        }

        public Define GetTargetStruct()
        {
            var target = Target;
            if (target is Member)
                return (target as Member).GetStructInternal();
            else if (target != null)
                return Types.GetStruct(target.Type);

            var g = Parent.GetFunction("get_" + TargetType);
            if (g != null)
                return Types.GetStruct(g.ReturnType.Type);
            return Parent.GetStruct(TargetType);
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
            var target = Target;
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
        {
            get { return Child != null ? Child.Type : TypeInternal; }
        }

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

        public NodeBase GetTarget()
        {
            if (Child != null)
                return Child.GetTarget();
            return GetTargetInternal();
        }

        public NodeBase GetTargetInternal()
        {
            var target = Target;
            if (target is Member)
                return (target as Member).Duplicate();
            return target;
        }

        private bool flgDelg;
        private Delegate delg;

        public Delegate GetDelegate()
        {
            if (Child != null) return Child.GetDelegate();

            if (flgDelg) return delg;
            flgDelg = true;

            var st = GetTargetStruct();
            if (st == null) return null;

            var m = st.GetMember(name);
            if (m != null) return null;

            var f = st.GetFunction(name);
            if (f == null || GetIsStatic()) return null;

            delg = new Delegate(Parent, f.CallType, new[] { Target }, f);
            return delg;
        }

        public Function GetFunction()
        {
            if (Child != null) return Child.GetFunction();

            var t = GetTargetStruct();
            if (t == null) return null;
            var ret = t.GetFunction(name);
            var target = Target;
            if (target is Base && (ret.IsVirtual || ret.IsOverride))
                ret = t.GetFunction("override_" + name);
            return ret;
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

                var t = Target;
                if (t == null) return false;

                TypeBase tt;
                if (t is Member)
                    tt = (t as Member).TypeInternal;
                else
                    tt = t.Type;
                if (tt is TypeString) return true;

                var tr = tt as TypeReference;
                return tr != null && tr.IsArray;
            }
        }

        public Member Duplicate()
        {
            var m = new Member(Parent, name);
            var target = Target;
            if (target is Member)
            {
                var t = (target as Member).Duplicate();
                t.Append(m);
                return t;
            }
            m.Target = target;
            return m;
        }

        protected Call GetCall(string prefix, params NodeBase[] args)
        {
            return new Call(Parent, GetFunction(prefix), GetTargetInternal(), args);
        }

        protected void AddSetterCodesInternal(OpModule codes, NodeBase arg)
        {
            GetCall("set_", arg).AddCodes(codes);
        }

        public void AddSetterCodes(OpModule codes, NodeBase arg)
        {
            if (Child != null)
                Child.AddSetterCodes(codes, arg);
            else
                AddSetterCodesInternal(codes, arg);
        }

        protected void AddCodesInternal(OpModule codes, string op, Addr32 dest)
        {
            var st = GetTargetStruct();
            var target = Target;
            if (IsLengthInternal)
            {
                if (target is Member)
                    (target as Member).AddCodesInternal(codes, "mov", null);
                else
                    target.AddCodes(codes, "mov", null);
                codes.Add(I386.MovRA(Reg32.EAX, Addr32.NewRO(Reg32.EAX, -4)));
                codes.AddCodes(op, dest);
                return;
            }
            else if (IsGetterInternal)
            {
                if (target == null)
                {
                    var g = Parent.GetFunction("get_" + TargetType);
                    if (g != null)
                    {
                        new Call(Parent, g.Name).AddCodes(codes, "mov", null);
                        var gg = GetFunction("get_");
                        codes.Add(I386.Push(Reg32.EAX));
                        codes.Add(I386.CallD(gg.First));
                        if (gg.CallType == CallType.CDecl)
                            codes.Add(I386.AddR(Reg32.ESP, Val32.New(4)));
                        codes.AddCodes(op, dest);
                        return;
                    }
                }
                GetCall("get_").AddCodes(codes, op, dest);
                return;
            }
            else if (IsFunctionInternal)
            {
                var delg = GetDelegate();
                if (delg != null)
                    delg.AddCodes(codes, op, dest);
                else
                {
                    var fp = new Variant(GetTargetStruct(), name);
                    fp.AddCodes(codes, op, dest);
                }
                return;
            }
            if (st != null)
            {
                var ci = st.GetInt(name);
                if (ci != null)
                {
                    ci.AddCodes(codes, op, dest);
                    return;
                }
                var cs = st.GetString(name);
                if (cs != null)
                {
                    cs.AddCodes(codes, op, dest);
                    return;
                }
            }
            TypeInternal.AddGetCodes(codes, op, dest, GetAddressInternal(codes));
        }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            if (Child != null)
                Child.AddCodes(codes, op, dest);
            else
                AddCodesInternal(codes, op, dest);
        }

        public string FullName
        {
            get
            {
                string n = null;
                if (target is Member)
                    n = (target as Member).FullName;
                else if (target is NodeBase)
                    n = (target as NodeBase).Name;
                else if (!string.IsNullOrEmpty(TargetType))
                    n = TargetType;
                if (string.IsNullOrEmpty(n)) return name;
                return n + "." + name;
            }
        }
    }
}
