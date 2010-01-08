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
        public const string Constructor = "ctor";
        public const string Destructor = "dtor";

        private string baseType;
        public string BaseType { get { return baseType; } }

        private Arg thisptr;

        public Define(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            RequiresName(xr);

            baseType = xr["base"];
            if (name == baseType)
                throw Abort(xr, "can not define recursive base type: " + name);

            thisptr = new Arg(this, "this", name);
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
            if (baseType == null) return null;
            Define st = parent.GetStruct(baseType);
            if (st != null) return st;
            throw Abort("undefined struct: " + baseType);
        }

        public int GetSize()
        {
            int ret = 0;
            Define st = GetBaseStruct();
            if (st != null) ret = st.GetSize();
            foreach (object obj in members.Values)
            {
                Pointer.Declare p = GetMember(obj);
                if (p != null) ret += p.Length;
            }
            if (ret == 0)
                ret = Var.Size;
            else
                ret = (ret + Var.Size - 1) / Var.Size * Var.Size;
            return ret;
        }

        public int GetOffset(string name)
        {
            int ret = 0;
            Define st = GetBaseStruct();
            if (st != null) ret = st.GetSize();
            foreach (object obj in members.Values)
            {
                Pointer.Declare p = GetMember(obj);
                if (p != null)
                {
                    if (p.Name == name) return ret;
                    ret += p.Length;
                }
            }
            if (st != null) return st.GetOffset(name);
            return -1;
        }

        public Define GetStruct(Pointer.Declare p)
        {
            if (p is Var.Declare)
            {
                Var.Declare v = p as Var.Declare;
                return v.GetStruct();
            }
            else if (p is Struct.Declare)
            {
                Struct.Declare st = p as Struct.Declare;
                return st.GetStruct();
            }
            return null;
        }

        private Pointer.Declare GetMember(object obj)
        {
            if (obj is Arg) return null;
            return obj as Pointer.Declare;
        }

        public Pointer.Declare GetMember(string name)
        {
            foreach (object obj in members.Values)
            {
                Pointer.Declare p = GetMember(obj);
                if (p != null && p.Name == name) return p;
            }
            Define st = GetBaseStruct();
            if (st == null) return null;
            return st.GetMember(name);
        }

        public Pointer.Declare[] GetMembers()
        {
            List<Pointer.Declare> list = new List<Pointer.Declare>();
            Define st = GetBaseStruct();
            if (st != null) list.AddRange(st.GetMembers());
            foreach (object obj in members.Values)
            {
                Pointer.Declare p = GetMember(obj);
                if (p != null) list.Add(p);
            }
            return list.ToArray();
        }

        public string GetMemberName(string name)
        {
            return this.name + "::" + name;
        }

        public void CheckStruct()
        {
            CheckBaseStruct(name);
            foreach (object obj in members.Values)
            {
                Define st = GetStruct(GetMember(obj));
                if (st != null) st.CheckStruct(name);
            }
        }

        public void CheckStruct(string type)
        {
            if (type == name)
                throw Abort("can not define recursive type: " + name);
            foreach (object obj in members.Values)
            {
                Define st = GetStruct(GetMember(obj));
                if (st != null) st.CheckStruct(type);
            }
        }

        public void CheckBaseStruct(string name)
        {
            Define b = GetBaseStruct();
            if (b == null) return;

            if (b.name == name)
                throw Abort("can not define recursive base type: " + name);
            b.CheckBaseStruct(name);
        }

        public override Struct.Define ThisStruct { get { return this; } }

        private void CallBlock(List<OpCode> codes, Module m, Addr32 ad, Block b)
        {
            codes.AddRange(new OpCode[]
            {
                I386.Lea(Reg32.EAX, new Addr32(ad)),
                I386.Push(Reg32.EAX),
                I386.Call(b.First)
            });
        }

        public void AddInit(List<OpCode> codes, Module m, Addr32 ad)
        {
            CallBlock(codes, m, ad, this);
            codes.Add(I386.Add(Reg32.ESP, Var.Size));
        }

        public void AddConstructor(List<OpCode> codes, Module m, Addr32 ad)
        {
            Function f = GetFunction(Constructor);
            if (f == null) return;

            CallBlock(codes, m, ad, f);
            if (f.Type == CallType.CDecl)
                codes.Add(I386.Add(Reg32.ESP, Var.Size));
        }

        public void AddDestructor(List<OpCode> codes, Module m, Addr32 ad)
        {
            Function f = GetFunction(Destructor);
            if (f == null) return;

            CallBlock(codes, m, ad, f);
            if (f.Type == CallType.CDecl)
                codes.Add(I386.Add(Reg32.ESP, Var.Size));
        }

        public void AddBeforeCtor(List<OpCode> codes, Module m, Var th)
        {
            codes.Add(I386.Mov(Reg32.EAX, th.GetAddress(codes, m)));
            AddInit(codes, m, new Addr32(Reg32.EAX));

            Define st = GetBaseStruct();
            int pos = 0;
            if (st != null)
            {
                codes.Add(I386.Mov(Reg32.EAX, th.GetAddress(codes, m)));
                st.AddConstructor(codes, m, new Addr32(Reg32.EAX));
                pos = st.GetSize();
            }
        }

        public void AddAfterDtor(List<OpCode> codes, Module m, Var th)
        {
            Define st = GetBaseStruct();
            int pos = GetSize();
            object[] members = new object[this.members.Count];
            this.members.Values.CopyTo(members, 0);
            Array.Reverse(members);
            foreach (object obj in members)
            {
                Pointer.Declare p = GetMember(obj);
                if (p != null) pos -= p.Length;
                Define memst = GetStruct(p);
                if (memst != null)
                {
                    codes.Add(I386.Mov(Reg32.EAX, th.GetAddress(codes, m)));
                    memst.AddDestructor(codes, m, new Addr32(Reg32.EAX, pos));
                }
            }
            if (st != null)
            {
                codes.Add(I386.Mov(Reg32.EAX, th.GetAddress(codes, m)));
                st.AddDestructor(codes, m, new Addr32(Reg32.EAX));
            }
        }

        public override bool HasStackFrame { get { return false; } }

        protected override void BeforeAddCodes(List<OpCode> codes, Module m)
        {
            Define st = GetBaseStruct();
            int pos = 0;
            if (st != null) pos = st.GetSize();
            thisptr.Address = new Addr32(Reg32.EBP, 8);
            foreach (object obj in members.Values)
            {
                Pointer.Declare p = GetMember(obj);
                if (p != null)
                {
                    p.Address = new Addr32(Reg32.EDX, pos);
                    pos += p.Length;
                }
            }
            int lv = Level + 1;
            codes.Add(I386.Enter((ushort)(lv * 4), (byte)lv));
            if (st != null) st.AddInit(codes, m, thisptr.Address);
        }

        protected override void AfterAddCodes(List<OpCode> codes, Module m)
        {
            AddExitCodes(codes, m);
        }

        public override void AddExitCodes(List<OpCode> codes, Module m)
        {
            codes.Add(I386.Leave());
            codes.Add(I386.Ret());
        }

        public override void AddDestructors(
            List<OpCode> codes, Module m, IEnumerable<Pointer.Declare> ptrs)
        {
        }
    }
}
