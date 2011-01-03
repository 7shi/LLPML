using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class TypeBase
    {
        public BlockBase Parent { get; protected set; }

        // type name
        public abstract string Name { get; }

        // type size
        public virtual int Size { get { return Var.DefaultSize; } }

        // functions
        public virtual bool CheckFunc(string op) { return false; }
        public virtual void AddOpCodes(string op, OpModule codes, Addr32 dest) { }

        // conditions
        protected Hashtable conds = new Hashtable();
        public virtual CondPair GetCond(string op)
        {
            if (!conds.ContainsKey(op))
                return null;
            return conds[op] as CondPair;
        }

        // get value
        public virtual void AddGetCodes(OpModule codes, string op, Addr32 dest, Addr32 src)
        {
            if (src != null) codes.Add(I386.Lea(Reg32.EAX, src));
            codes.AddCodes(op, dest);
        }

        // set value
        public virtual void AddSetCodes(OpModule codes, Addr32 dest)
        {
            throw new Exception("can not set value!");
        }

        // check array
        public virtual bool IsArray { get { return false; } }

        // check value
        public virtual bool IsValue { get { return true; } }

        // recursive type
        public virtual TypeBase Type { get; protected set; }

        // cast
        public virtual TypeBase Cast(TypeBase type)
        {
            var tr1 = this as TypeReference;
            var tr2 = type as TypeReference;
            if (tr1 == null && tr2 != null && tr2.UseGC)
                return null;

            var t1 = Type;
            var t2 = type.Type;
            if (t1 == t2) return type;

            var ts1 = t1 as TypeStruct;
            var ts2 = t2 as TypeStruct;
            if (ts1 == null || ts2 == null) return null;

            var st1 = ts1.GetStruct();
            var st2 = ts2.GetStruct();
            if (st1 == null || st2 == null) return null;
            if (st1 == st2 || st1.CanUpCast(st2)) return type;

            return null;
        }

        // type constructor
        public virtual bool NeedsCtor { get { return false; } }
        public virtual void AddConstructor(OpModule codes) { }

        public void AddConstructor(OpModule codes, Addr32 ad)
        {
            if (ad != null)
                codes.Add(I386.Lea(Reg32.EAX, ad));
            codes.Add(I386.Push(Reg32.EAX));
            AddConstructor(codes);
            codes.Add(I386.AddR(Reg32.ESP, Val32.New(4)));
        }

        // type destructor
        public virtual bool NeedsDtor { get { return false; } }
        public virtual void AddDestructor(OpModule codes) { }

        public void AddDestructor(OpModule codes, Addr32 ad)
        {
            if (ad != null)
                codes.Add(I386.Lea(Reg32.EAX, ad));
            codes.Add(I386.Push(Reg32.EAX));
            AddDestructor(codes);
            codes.Add(I386.AddR(Reg32.ESP, Val32.New(4)));
        }

        // type check
        public virtual bool Check()
        {
            if (Type != null) return Type.Check();
            return true;
        }
    }
}
