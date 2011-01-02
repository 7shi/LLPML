using System;
using System.Collections.Generic;
using System.Text;
using Girl.X86;

namespace Girl.LLPML
{
    public class CondPair
    {
        public Cc Condition { get; private set; }
        public Cc NotCondition { get; private set; }

        public static CondPair New(Cc condition, Cc notCondition)
        {
            var ret = new CondPair();
            ret.Condition = condition;
            ret.NotCondition = notCondition;
            return ret;
        }
    }
}
