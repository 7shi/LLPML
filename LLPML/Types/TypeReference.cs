﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeReference : TypeBase
    {
        // type name
        public override string Name { get { return "var:" + Type.Name; } }

        // functions
        public override Func GetFunc(string key)
        {
            var ret = Type.GetFunc(key);
            if (ret != null) return ret;

            if (key == "equal" || key == "not-equal")
                return TypeInt.Instance.GetFunc(key);
            return null;
        }

        // conditions
        public override CondPair GetCond(string key)
        {
            var ret = Type.GetCond(key);
            if (ret != null) return ret;

            if (key == "equal" || key == "not-equal")
                return TypeInt.Instance.GetCond(key);
            return null;
        }

        public TypeReference(TypeBase type)
        {
            Type = type;
        }

        // get value
        public override void AddGetCodes(OpCodes codes, string op, Addr32 dest, Addr32 src)
        {
            codes.AddCodes(op, dest, src);
        }

        // set value
        public override void AddSetCodes(OpCodes codes, Addr32 ad)
        {
            codes.Add(I386.Mov(ad, Reg32.EAX));
        }
    }
}
