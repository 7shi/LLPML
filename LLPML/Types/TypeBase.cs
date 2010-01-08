using System;
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
            codes.Add(I386.Lea(Reg32.EAX, src));
            codes.AddCodes(op, dest);
        }

        // set value
        public virtual void AddSetCodes(OpCodes codes, Addr32 ad)
        {
            throw new Exception("can not set value!");
        }

        // check array
        public virtual bool IsArray { get { return false; } }

        // check value
        public virtual bool IsValue { get { return true; } }

        // recursive type
        public TypeBase Type { get; protected set; }

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
