using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Define : Block
    {
        public const string Initializer = "izer";
        public const string Constructor = "ctor";
        public const string Destructor = "dtor";

        public string BaseType { get; set; }

        protected TypeBase type;

        public override TypeBase Type
        {
            get
            {
                if (type == null)
                    type = TypeStruct.New(this, name);
                return type;
            }
        }

        public bool IsClass { get; set; }

        private Arg thisptr;
        private bool isAnonymous;

        public Define(BlockBase parent, string name)
            : base(parent)
        {
            if (string.IsNullOrEmpty(name))
            {
                isAnonymous = true;
                name = Parent.GetAnonymousName();
            }
            this.name = name;
            thisptr = new Arg(this, "this", Types.ToVarType(Type));
        }

        public Define(BlockBase parent, string name, string baseType)
            : this(parent, name)
        {
            BaseType = baseType;
        }

        public override object GetMember(string name)
        {
            if (members.ContainsKey(name))
                return members.Get(name);
            var st = GetBaseStruct();
            if (st == null) return null;
            return st.GetMember(name);
        }

        public override object GetMemberRecursive(string name)
        {
            var ret = GetMember(name);
            if (ret != null || Parent == null) return ret;
            ret = Parent.GetMemberRecursive(name);
            if (ret == null || !(ret is VarDeclare || ret is Function)) return ret;
            if ((ret as NodeBase).Parent is Root) return ret;
            return null;
        }

        public Define GetBaseStruct()
        {
            if (BaseType == null) return null;
            Define st = Parent.GetStruct(BaseType);
            if (!root.IsCompiling || st != null) return st;
            throw Abort("undefined struct: " + BaseType);
        }

        protected int GetSizeInternal()
        {
            int ret = 0;
            ForEachMembers(null, size => ret = size);
            Define st = GetBaseStruct();
            if (st != null) ret += st.GetSizeInternal();
            return ret;
        }

        public int GetSize()
        {
            var ret = GetSizeInternal();
            if (ret == 0)
                ret = Var.DefaultSize;
            return ret;
        }

        public int GetOffset(string name)
        {
            int ret = -1;
            ForEachMembers((p, pos) =>
            {
                if (p.Name != name) return false;
                ret = pos;
                return true;
            }, null);
            Define st = GetBaseStruct();
            if (st == null) return ret;

            if (ret < 0) return st.GetOffset(name);
            return ret + st.GetSizeInternal();
        }

        public VarDeclare GetMemberDecl(string name)
        {
            VarDeclare ret = null;
            ForEachMembers((p, pos) =>
            {
                if (p.Name != name) return false;
                ret = p;
                return true;
            }, null);
            if (ret != null) return ret;
            Define st = GetBaseStruct();
            if (st == null) return null;
            return st.GetMemberDecl(name);
        }

        public VarDeclare[] GetMemberDecls()
        {
            List<VarDeclare> list = new List<VarDeclare>();
            Define st = GetBaseStruct();
            if (st != null) list.AddRange(st.GetMemberDecls());
            ForEachMembers((p, pos) =>
            {
                list.Add(p);
                return false;
            }, null);
            return list.ToArray();
        }

        public override Define ThisStruct { get { return this; } }

        private void CallBlock(OpModule codes, Addr32 ad, Block b, CallType ct)
        {
            if (ad != null)
                codes.Add(I386.PushA(ad));
            else
                codes.Add(I386.Push(Reg32.EAX));
            codes.Add(I386.CallD(b.First));
            if (ct == CallType.CDecl)
                codes.Add(I386.AddR(Reg32.ESP, Val32.New(4)));
        }

        public void AddInit(OpModule codes, Addr32 ad)
        {
            if (!NeedsInit) return;

            //AddDebug(codes, "AddInit: " + name);
            CallBlock(codes, ad, this, CallType.CDecl);
        }

        public void AddConstructor(OpModule codes, Addr32 ad)
        {
            if (!NeedsCtor) return;

            //AddDebug(codes, "AddConstructor: " + name);
            var f = GetFunction(Constructor);
            CallBlock(codes, ad, f, f.CallType);
        }

        private void AddDestructor(OpModule codes, Addr32 ad)
        {
            if (!NeedsDtor) return;

            //AddDebug(codes, "AddDestructor: " + name);
            var dtor = GetFunction(Destructor);
            if (dtor.IsVirtual)
                dtor = GetFunction("override_" + Destructor);
            CallBlock(codes, ad, dtor, dtor.CallType);
        }

        public void AddBeforeCtor(OpModule codes)
        {
            Define st = GetBaseStruct();
            if (st != null)
                st.AddConstructor(codes, Addr32.NewRO(Reg32.EBP, 8));
        }

        public void AddAfterDtor(OpModule codes)
        {
            var list = new List<VarDeclare>();
            var poslist = new Dictionary<VarDeclare, int>();
            int offset = 0;
            var st = GetBaseStruct();
            if (st != null) offset = st.GetSizeInternal();
            ForEachMembers((p, pos) =>
            {
                if (!p.IsStatic && p.NeedsDtor)
                {
                    list.Add(p);
                    poslist[p] = offset + pos;
                }
                return false;
            }, null);
            list.Reverse();
            var ad = Addr32.NewRO(Reg32.EBP, 8);
            foreach (var p in list)
            {
                codes.Add(I386.MovRA(Var.DestRegister, ad));
                p.Type.AddDestructor(codes, Addr32.NewRO(Var.DestRegister, poslist[p]));
            }
            if (st != null)
                st.AddDestructor(codes, ad);
        }

        public override bool HasStackFrame { get { return false; } }

        protected override void BeforeAddCodes(OpModule codes)
        {
            thisptr.Address = Addr32.NewRO(Reg32.EBP, 8);
            int lv = Level + 1;
            codes.Add(I386.Enter((ushort)(lv * 4), (byte)lv));
            var st = GetBaseStruct();
            if (st != null && !IsEmpty) st.AddInit(codes, thisptr.Address);
        }

        protected override void AfterAddCodes(OpModule codes)
        {
            AddExitCodes(codes);
        }

        public override void AddExitCodes(OpModule codes)
        {
            codes.Add(I386.Leave());
            codes.Add(I386.Ret());
        }

        public override void AddDestructors(
            OpModule codes, IEnumerable<VarDeclare> ptrs)
        {
        }

        public bool NeedsInit
        {
            get
            {
                var st = GetBaseStruct();
                if (st != null && st.NeedsInit) return true;

                foreach (var s in sentences)
                {
                    if (s is VarDeclare)
                    {
                        var d = s as VarDeclare;
                        if (!d.NeedsInit && !d.NeedsCtor)
                            continue;
                    }
                    return true;
                }
                return false;
            }
        }

        public bool NeedsCtor
        {
            get
            {
                var st = GetBaseStruct();
                if (st != null && st.NeedsCtor) return true;
                return GetFunction(Constructor).Sentences.Count > 0;
            }
        }

        public bool NeedsDtor
        {
            get
            {
                var st = GetBaseStruct();
                if (st != null && st.NeedsDtor) return true;
                var dtor = GetFunction(Destructor);
                if (dtor.IsVirtual || dtor.Sentences.Count > 0)
                    return true;
                foreach (object obj in members.Values)
                {
                    var vd = obj as VarDeclare;
                    if (vd != null && vd.NeedsDtor)
                        return true;
                }
                return false;
            }
        }

        private bool doneCheckStruct = false;

        public void CheckStruct()
        {
            if (doneCheckStruct) return;
            doneCheckStruct = true;

            if (IsClass && BaseType == null && FullName != "object")
                BaseType = "object";
            var list = new List<Define>();
            CheckStruct(list);
            CheckField();
        }

        private void CheckStruct(List<Define> list)
        {
            MakeUp();
            if (list.Contains(this))
                throw Abort("can not define recursive type: {0}", list[0].name);
            list.Add(this);
            var b = GetBaseStruct();
            if (b == null) return;

            if (IsClass && !b.IsClass)
                throw Abort("class: can not inherit from struct: {0} <= {1}", FullName, b.FullName);
            else if (!IsClass && b.IsClass)
                throw Abort("struct: can not inherit from class: {0} <= {1}", FullName, b.FullName);
            b.CheckStruct(list);
        }

        public bool IsEmpty { get; private set; }
        private bool doneCheckField = false;

        public void CheckField()
        {
            if (doneCheckField) return;
            doneCheckField = true;

            IsEmpty = isAnonymous && members.Count <= 1;
            foreach (var obj in members.Values)
            {
                var field = obj as Declare;
                if (field == null) continue;

                var t = field.Type;
                if (!(t is TypeStruct)) continue;

                var st = (t as TypeStruct).GetStruct();
                field.CheckField(this, st);
            }

            var bst = GetBaseStruct();
            int offset = 0;
            if (bst != null) offset = bst.GetSizeInternal();
            ForEachMembers((p, pos) =>
            {
                if (!p.IsStatic)
                    p.Address = Addr32.NewRO(Var.DestRegister, offset + pos);
                return false;
            }, null);
        }

        protected override void MakeUpInternal()
        {
            CheckStruct();

            var f1 = base.GetMember(Initializer) as Function;
            if (f1 == null) AddFunction(new Function(this, Initializer, false));

            var f2 = base.GetMember(Constructor) as Function;
            if (f2 == null) AddFunction(new Function(this, Constructor, false));

            var f3 = base.GetMember(Destructor) as Function;
            if (f3 == null) AddFunction(new Function(this, Destructor, false));
        }

        public bool CanUpCast(Define st)
        {
            MakeUp();
            if (st == this) return true;
            var b = GetBaseStruct();
            if (b == null) return false;
            return b.CanUpCast(st);
        }
    }
}
