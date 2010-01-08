using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct2
{
    public class Define : Block
    {
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

            if (!parent.AddStruct2(this))
                throw Abort("multiple definitions: " + name);
        }

        public Define GetBaseStruct()
        {
            if (baseType == null) return null;
            Define st = parent.GetStruct2(baseType);
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
                if (v.Name == name) return v.GetStruct2();
            }
            else if (p is Struct2.Declare)
            {
                Struct2.Declare st = p as Struct2.Declare;
                if (st.Name == name) return st.GetStruct();
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
            if (st != null) return st.GetMember(name);
            return null;
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

        public override Struct2.Define ThisStruct { get { return this; } }

        public void AddInitializer(List<OpCode> codes, Module m, Addr32 ad)
        {
            CallBlock(codes, m, ad, this);
            codes.Add(I386.Add(Reg32.ESP, Var.Size));
        }

        public void AddConstructor(List<OpCode> codes, Module m, Addr32 ad)
        {
            Function f = GetFunction("this");
            if (f == null) return;

            CallBlock(codes, m, ad, f);
            if (f.Type == CallType.CDecl)
                codes.Add(I386.Add(Reg32.ESP, Var.Size));
        }

        public void AddPreCtor(List<OpCode> codes, Module m, Addr32 ad)
        {
            Define st = GetBaseStruct();
            Addr32 ad2 = new Addr32(ad);
            if (st != null)
            {
                st.AddConstructor(codes, m, ad);
                ad2.Add(st.GetSize());
            }
            foreach (object obj in members.Values)
            {
                Pointer.Declare p = GetMember(obj);
                Define memst = GetStruct(p);
                if (memst != null) memst.AddConstructor(codes, m, ad2);
                if (p != null) ad2.Add(p.Length);
            }
        }

        private void CallBlock(List<OpCode> codes, Module m, Addr32 ad, Block b)
        {
            codes.AddRange(new OpCode[]
            {
                I386.Lea(Reg32.EAX, new Addr32(ad)),
                I386.Push(Reg32.EAX),
                I386.Call(b.First)
            });
        }

        public void AddDestructor(List<OpCode> codes, Module m, Addr32 ad)
        {
            Function dtor = GetFunction("~this");
            if (dtor != null)
            {
                codes.AddRange(new OpCode[]
                {
                    I386.Lea(Reg32.EAX, new Addr32(ad)),
                    I386.Push(Reg32.EAX),
                    I386.Call(dtor.First)
                });
                if (dtor.Type == CallType.CDecl)
                {
                    codes.Add(I386.Add(Reg32.ESP, 4));
                }
            }

            Define st = GetBaseStruct();
            Addr32 ad2 = new Addr32(ad);
            if (st != null) ad2.Add(st.GetSize());
            foreach (object obj in members.Values)
            {
                Pointer.Declare p = GetMember(obj);
                Define memst = GetStruct(p);
                if (memst != null) memst.AddDestructor(codes, m, ad2);
                if (p != null) ad2.Add(p.Length);
            }
            if (st != null) st.AddDestructor(codes, m, ad);
        }

        public override bool HasStackFrame { get { return false; } }

        protected override void BeforeAddCodes(List<OpCode> codes, Module m)
        {
            int offset = 0;
            thisptr.Address = new Addr32(Reg32.EBP, 8);
            foreach (object obj in members.Values)
            {
                Pointer.Declare p = GetMember(obj);
                if (p != null)
                {
                    p.Address = new Addr32(Reg32.EDX, offset);
                    offset += p.Length;
                }
            }
            int lv = Level + 1;
            codes.Add(I386.Enter((ushort)(lv * 4), (byte)lv));
            Define st = GetBaseStruct();
            if (st != null) st.AddInitializer(codes, m, thisptr.Address);
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
    }
}
