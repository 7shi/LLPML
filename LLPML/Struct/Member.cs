using System;
using System.Collections.Generic;
using System.Text;
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
            }
        }

        public string TargetType { get; set; }

        public Member Child { get; protected set; }

        public static Member New(BlockBase parent, string name)
        {
            var ret = new Member();
            ret.Parent = parent;
            ret.name = name;
            return ret;
        }

        protected Addr32 GetAddressInternal(OpModule codes)
        {
            var st = GetTargetStruct();
            if (st == null)
                throw Abort("can not find member: {0}", name);

            var mem = st.GetMemberDecl(name);
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
                target.AddCodesV(codes, "mov", null);
                codes.Add(I386.Mov(Var.DestRegister, Reg32.EAX));
                ret = Addr32.New(Var.DestRegister);
            }
            else
            {
                var g = Parent.GetFunction("get_" + TargetType);
                if (g != null)
                {
                    Call.NewName(Parent, g.Name).AddCodesV(codes, "mov", null);
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
                        var gg = GetFunctionPrefix("get_");
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
                GetCall("get_", null).AddCodesV(codes, "mov", null);
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
            if (Child != null)
                return Child.GetStruct();
            else
                return GetStructInternal();
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

                var m = st.GetMemberDecl(name);
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
            get
            {
                if (Child != null) return Child.Type;
                return TypeInternal;
            }
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

            var m = st.GetMemberDecl(name);
            if (m != null) return null;

            var f = st.GetFunction(name);
            if (f == null || GetIsStatic()) return null;

            var args = new NodeBase[1];
            args[0] = Target;
            delg = Delegate.New(Parent, f.CallType, args, f);
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

        protected Function GetFunctionPrefix(string prefix)
        {
            var st = GetTargetStruct();
            if (st == null) return null;
            int of = st.GetOffset(name);
            if (of >= 0) return null;
            return st.GetFunction(prefix + name);
        }

        protected bool CheckFunction(string prefix)
        {
            return GetFunctionPrefix(prefix) != null;
        }

        protected bool IsFunctionInternal { get { return CheckFunction(""); } }
        public bool IsFunction
        {
            get
            {
                if (Child != null) return Child.IsFunction;
                return IsFunctionInternal;
            }
        }

        protected bool IsSetterInternal { get { return CheckFunction("set_"); } }
        public bool IsSetter
        {
            get
            {
                if (Child != null) return Child.IsSetter;
                return IsSetterInternal;
            }
        }

        protected bool IsGetterInternal { get { return CheckFunction("get_"); } }
        public bool IsGetter
        {
            get
            {
                if (Child != null) return Child.IsGetter;
                return IsGetterInternal;
            }
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
            var m = Member.New(Parent, name);
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

        protected Call GetCall(string prefix, NodeBase[] args)
        {
            return Call.NewV(Parent, GetFunctionPrefix(prefix), GetTargetInternal(), args);
        }

        protected void AddSetterCodesInternal(OpModule codes, NodeBase arg)
        {
            var args = new NodeBase[1];
            args[0] = arg;
            GetCall("set_", args).AddCodes(codes);
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
                    target.AddCodesV(codes, "mov", null);
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
                        Call.NewName(Parent, g.Name).AddCodesV(codes, "mov", null);
                        var gg = GetFunctionPrefix("get_");
                        codes.Add(I386.Push(Reg32.EAX));
                        codes.Add(I386.CallD(gg.First));
                        if (gg.CallType == CallType.CDecl)
                            codes.Add(I386.AddR(Reg32.ESP, Val32.New(4)));
                        codes.AddCodes(op, dest);
                        return;
                    }
                }
                GetCall("get_", null).AddCodesV(codes, op, dest);
                return;
            }
            else if (IsFunctionInternal)
            {
                var delg = GetDelegate();
                if (delg != null)
                    delg.AddCodesV(codes, op, dest);
                else
                {
                    var fp = Variant.NewName(GetTargetStruct(), name);
                    fp.AddCodesV(codes, op, dest);
                }
                return;
            }
            if (st != null)
            {
                var ci = st.GetInt(name);
                if (ci != null)
                {
                    ci.AddCodesV(codes, op, dest);
                    return;
                }
                var cs = st.GetString(name);
                if (cs != null)
                {
                    cs.AddCodesV(codes, op, dest);
                    return;
                }
            }
            TypeInternal.AddGetCodes(codes, op, dest, GetAddressInternal(codes));
        }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            if (Child != null)
                Child.AddCodesV(codes, op, dest);
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
