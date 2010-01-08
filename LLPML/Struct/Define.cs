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

        public string BaseType { get; set; }

        private Arg thisptr;

        public Define(BlockBase parent, string name)
            : base(parent)
        {
            this.name = name;
            thisptr = new Arg(this, "this", name);
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

        public Pointer.Declare GetMember(string name)
        {
            Pointer.Declare ret = null;
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

        public Pointer.Declare[] GetMembers()
        {
            List<Pointer.Declare> list = new List<Pointer.Declare>();
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

        public void CheckStruct()
        {
            CheckStruct(null);
        }

        public void CheckStruct(string type)
        {
            if (type == null)
            {
                CheckBaseStruct(name);
                type = name;
            }
            else if (type == name)
                throw Abort("can not define recursive type: " + name);
            ForEachMembers((p, pos) =>
                {
                    if (p is Struct.Declare)
                    {
                        var st = GetStruct(p);
                        if (st != null) st.CheckStruct(type);
                    }
                    return false;
                }, null);
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

        private void CallBlock(OpCodes codes, Addr32 ad, Block b)
        {
            codes.AddRange(new OpCode[]
            {
                I386.Lea(Reg32.EAX, new Addr32(ad)),
                I386.Push(Reg32.EAX),
                I386.Call(b.First)
            });
        }

        public void AddInit(OpCodes codes, Addr32 ad)
        {
            CallBlock(codes, ad, this);
            codes.Add(I386.Add(Reg32.ESP, Var.DefaultSize));
        }

        public void AddConstructor(OpCodes codes, Addr32 ad)
        {
            Function f = GetFunction(Constructor);
            if (f == null) return;

            CallBlock(codes, ad, f);
            if (f.CallType == CallType.CDecl)
                codes.Add(I386.Add(Reg32.ESP, Var.DefaultSize));
        }

        public void AddDestructor(OpCodes codes, Addr32 ad)
        {
            Function f = GetFunction(Destructor);
            if (f == null) return;

            CallBlock(codes, ad, f);
            if (f.CallType == CallType.CDecl)
                codes.Add(I386.Add(Reg32.ESP, Var.DefaultSize));
        }

        public void AddBeforeCtor(OpCodes codes, Var th)
        {
            codes.Add(I386.Mov(Reg32.EAX, th.GetAddress(codes)));
            AddInit(codes, new Addr32(Reg32.EAX));

            Define st = GetBaseStruct();
            int pos = 0;
            if (st != null)
            {
                codes.Add(I386.Mov(Reg32.EAX, th.GetAddress(codes)));
                st.AddConstructor(codes, new Addr32(Reg32.EAX));
                pos = st.GetSize();
            }
        }

        public void AddAfterDtor(OpCodes codes, Var th)
        {
            var list = new List<Pointer.Declare>();
            var poslist = new Dictionary<Pointer.Declare, int>();
            ForEachMembers((p, pos) =>
                {
                    list.Add(p);
                    poslist[p] = pos;
                    return false;
                }, null);
            list.Reverse();
            foreach (var p in list)
            {
                Define memst = GetStruct(p);
                if (memst == null) continue;

                codes.Add(I386.Mov(Reg32.EAX, th.GetAddress(codes)));
                memst.AddDestructor(codes, new Addr32(Reg32.EAX, poslist[p]));
            }
            Define st = GetBaseStruct();
            if (st != null)
            {
                codes.Add(I386.Mov(Reg32.EAX, th.GetAddress(codes)));
                st.AddDestructor(codes, new Addr32(Reg32.EAX));
            }
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
                p.Address = new Addr32(Reg32.EDX, offset + pos);
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
            OpCodes codes, IEnumerable<Pointer.Declare> ptrs)
        {
        }
    }
}
