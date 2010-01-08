using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
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

        public TypeBase Type
        {
            get
            {
                if (type == null)
                    type = new TypeStruct(this, name);
                return type;
            }
        }

        private Arg thisptr;

        public Define(BlockBase parent, string name)
            : base(parent)
        {
            this.name = name;
            thisptr = new Arg(this, "this", new TypeReference(Type));
        }

        public Define(BlockBase parent, string name, string baseType)
            : this(parent, name)
        {
            BaseType = baseType;
        }

        public Define(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            RequiresName(xr);

            BaseType = xr["base"];
            if (name == BaseType)
                throw Abort(xr, "can not define recursive base type: " + name);

            thisptr = new Arg(this, "this", new TypeReference(Type));
            base.Read(xr);

            if (!parent.AddStruct(this))
                throw Abort("multiple definitions: " + name);
        }

        public override T GetMember<T>(string name)
        {
            object obj;
            if (members.TryGetValue(name, out obj)) return obj as T;
            Define st = GetBaseStruct();
            if (st == null) return null;
            return st.GetMember<T>(name);
        }

        public Define GetBaseStruct()
        {
            if (BaseType == null) return null;
            Define st = parent.GetStruct(BaseType);
            if (st != null) return st;
            throw Abort("undefined struct: " + BaseType);
        }

        public int GetSize()
        {
            int ret = 0;
            ForEachMembers(null, size => ret = size);
            Define st = GetBaseStruct();
            if (st != null) ret += st.GetSize();
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
            return ret + st.GetSize();
        }

        public Var.Declare GetMember(string name)
        {
            Var.Declare ret = null;
            ForEachMembers((p, pos) =>
            {
                if (p.Name != name) return false;
                ret = p;
                return true;
            }, null);
            if (ret != null) return ret;
            Define st = GetBaseStruct();
            if (st == null) return null;
            return st.GetMember(name);
        }

        public Var.Declare[] GetMembers()
        {
            List<Var.Declare> list = new List<Var.Declare>();
            Define st = GetBaseStruct();
            if (st != null) list.AddRange(st.GetMembers());
            ForEachMembers((p, pos) =>
            {
                list.Add(p);
                return false;
            }, null);
            return list.ToArray();
        }

        public string GetMemberName(string name)
        {
            return this.name + Separator + name;
        }

        public override Struct.Define ThisStruct { get { return this; } }

        private void CallBlock(OpCodes codes, Addr32 ad, Block b, CallType ct)
        {
            if (ad != null)
                codes.Add(I386.Push(ad));
            else
                codes.Add(I386.Push(Reg32.EAX));
            codes.Add(I386.Call(b.First));
            if (ct == CallType.CDecl)
                codes.Add(I386.Add(Reg32.ESP, 4));
        }

        public void AddInit(OpCodes codes, Addr32 ad)
        {
            if (!NeedsInit) return;

            //AddDebug(codes, "AddInit: " + name);
            CallBlock(codes, ad, this, CallType.CDecl);
        }

        public void AddConstructor(OpCodes codes, Addr32 ad)
        {
            if (!NeedsCtor) return;

            //AddDebug(codes, "AddConstructor: " + name);
            var f = GetFunction(Constructor);
            CallBlock(codes, ad, f, f.CallType);
        }

        public void AddDestructor(OpCodes codes, Addr32 ad)
        {
            if (!NeedsDtor) return;

            //AddDebug(codes, "AddDestructor: " + name);
            var f = GetFunction(Destructor);
            CallBlock(codes, ad, f, f.CallType);
        }

        public void AddBeforeCtor(OpCodes codes)
        {
            Define st = GetBaseStruct();
            if (st != null)
                st.AddConstructor(codes, new Addr32(Reg32.EBP, 8));
        }

        public void AddAfterDtor(OpCodes codes)
        {
            var list = new List<Var.Declare>();
            var poslist = new Dictionary<Var.Declare, int>();
            int offset = 0;
            var st = GetBaseStruct();
            if (st != null) offset = st.GetSize();
            ForEachMembers((p, pos) =>
            {
                if (p.NeedsDtor)
                {
                    list.Add(p);
                    poslist[p] = offset + pos;
                }
                return false;
            }, null);
            list.Reverse();
            var ad = new Addr32(Reg32.EBP, 8);
            foreach (var p in list)
            {
                codes.Add(I386.Mov(Var.DestRegister, ad));
                p.Type.AddDestructor(codes, new Addr32(Var.DestRegister, poslist[p]));
            }
            if (st != null)
                st.AddDestructor(codes, ad);
        }

        public override bool HasStackFrame { get { return false; } }

        protected override void BeforeAddCodes(OpCodes codes)
        {
            Define st = GetBaseStruct();
            int offset = 0;
            if (st != null) offset = st.GetSize();
            thisptr.Address = new Addr32(Reg32.EBP, 8);
            ForEachMembers((p, pos) =>
            {
                p.Address = new Addr32(Var.DestRegister, offset + pos);
                return false;
            }, null);
            int lv = Level + 1;
            codes.Add(I386.Enter((ushort)(lv * 4), (byte)lv));
            if (st != null) st.AddInit(codes, thisptr.Address);
        }

        protected override void AfterAddCodes(OpCodes codes)
        {
            AddExitCodes(codes);
        }

        public override void AddExitCodes(OpCodes codes)
        {
            codes.Add(I386.Leave());
            codes.Add(I386.Ret());
        }

        public override void AddDestructors(
            OpCodes codes, IEnumerable<Var.Declare> ptrs)
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
                    if (s is Var.Declare)
                    {
                        var d = s as Var.Declare;
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
                if (GetFunction(Destructor).Sentences.Count > 0)
                    return true;
                foreach (object obj in members.Values)
                {
                    var vd = obj as Var.Declare;
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

            var list = new List<Define>();
            CheckStruct(list);
            CheckField();
        }

        private void CheckStruct(List<Define> list)
        {
            MakeUp();
            if (list.Contains(this))
                throw Abort("can not define recursive type: " + list[0].name);
            list.Add(this);
            var b = GetBaseStruct();
            if (b != null) b.CheckStruct(list);
        }

        private bool doneCheckField = false;

        public void CheckField()
        {
            if (doneCheckField) return;
            doneCheckField = true;

            foreach (var obj in members.Values)
            {
                var field = obj as Declare;
                if (field == null) continue;

                var t = field.Type;
                if (!(t is TypeStruct)) continue;

                var st = (t as TypeStruct).GetStruct();
                field.CheckField(this, st);
            }
        }

        protected override void MakeUpInternal()
        {
            CheckStruct();
            string[] funcs = { Initializer, Constructor, Destructor };
            foreach (var func in funcs)
            {
                var f = base.GetMember<Function>(func);
                if (f == null) AddFunction(new Function(this, func));
            }
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
