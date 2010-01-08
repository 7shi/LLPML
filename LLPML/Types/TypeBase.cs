﻿using System;
using System.Collections.Generic;
using System.Text;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class TypeBase
    {
        // type name
        public abstract string Name { get; }

        // type size
        public virtual int Size { get { return Var.DefaultSize; } }

        // functions
        public delegate void Func(OpCodes codes, Addr32 dest, IIntValue arg);
        protected Dictionary<string, Func> funcs = new Dictionary<string, Func>();
        public virtual Func GetFunc(string key)
        {
            Func f;
            if (!funcs.TryGetValue(key, out f)) return null;
            return f;
        }

        // conditions
        protected Dictionary<string, CondPair> conds = new Dictionary<string, CondPair>();
        public virtual CondPair GetCond(string key)
        {
            CondPair c;
            if (!conds.TryGetValue(key, out c)) return null;
            return c;
        }

        // get value
        public virtual void AddGetCodes(OpCodes codes, string op, Addr32 dest, Addr32 src)
        {
            if (src != null) codes.Add(I386.Lea(Reg32.EAX, src));
            codes.AddCodes(op, dest);
        }

        // set value
        public virtual void AddSetCodes(OpCodes codes, Addr32 dest)
        {
            throw new Exception("can not set value!");
        }

        // check array
        public virtual bool IsArray { get { return false; } }

        // check value
        public virtual bool IsValue { get { return true; } }

        // recursive type
        public TypeBase Type { get; protected set; }

        // cast
        public virtual TypeBase Cast(TypeBase type)
        {
            var t1 = Type;
            var t2 = type.Type;
            if (t1 == t2) return type;

            var st1 = t1 as TypeStruct;
            var st2 = t2 as TypeStruct;
            if (st1 == null || st2 == null) return null;

            if (st1.GetStruct().CanUpCast(st2.GetStruct()))
                return type;

            return null;
        }

        // type constructor
        public virtual bool NeedsCtor { get { return false; } }
        public virtual void AddConstructor(OpCodes codes, Addr32 ad) { }

        // type destructor
        public virtual bool NeedsDtor { get { return false; } }
        public virtual void AddDestructor(OpCodes codes, Addr32 ad) { }

        // operator name
        public static string GetFuncName(string op)
        {
            switch (op)
            {
                case "++": return "operator_inc";
                case "--": return "operator_dec";
                case "+" : return "operator_add";
                case "-" : return "operator_sub";
                case "&" : return "operator_and";
                case "|" : return "operator_or";
                case "^" : return "operator_xor";
                case "<<": return "operator_left";
                case ">>": return "operator_right";
                case "*" : return "operator_mul";
                case "/" : return "operator_div";
                case "%" : return "operator_mod";
                case "!" : return "operator_not";
                case "~" : return "operator_rev";
                case "==": return "operator_equal";
                case "!=": return "operator_not_equal";
                case ">" : return "operator_greater";
                case ">=": return "operator_greater_equal";
                case "<" : return "operator_less";
                case "<=": return "operator_less_equal";
            }
            // -X => operator_neg
            return null;
        }
    }
}
