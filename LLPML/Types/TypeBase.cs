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

        // functions
        public delegate void Func(OpCodes codes, Addr32 dest, IIntValue arg);
        protected Dictionary<string, Func> funcs = new Dictionary<string, Func>();
        public bool TryGetFunc(string key, out Func f) { return funcs.TryGetValue(key, out f); }

        // conditions
        protected Dictionary<string, CondPair> conds = new Dictionary<string, CondPair>();
        public bool TryGetCond(string key, out CondPair c) { return conds.TryGetValue(key, out c); }

        // <to-int>        cast to int
        // <set>           operator =
        // <add>           operator +
        // <sub>           operator -
        // <mul>           operator *
        // <div>           operator /
        // <equal>         operator ==
        // <not-equal>     operator !=
        // <shift-left>    operator <<
        // <shift-right>   operator >>
        // <less>          operator <
        // <greater>       operator >
        // <less-equal>    operator <=
        // <greater-equal> operator >=
    }
}
