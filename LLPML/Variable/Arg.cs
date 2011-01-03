using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Arg : VarDeclare
    {
        private VarDeclare target;

        public static new Arg New(BlockBase parent, string name, TypeBase type)
        {
            var ret = new Arg();
            ret.init1(parent, name, type);
            return ret;
        }

        public static Arg NewVar(BlockBase parent, VarDeclare target)
        {
            var ret = new Arg();
            ret.init1(parent, target.Name, null);
            ret.target = target;
            return ret;
        }

        protected override void init2() { }

        public override TypeBase Type
        {
            get
            {
                if (target != null) return target.Type;
                return base.Type;
            }
        }
    }

    public class ArgPtr : Arg
    {
        public static ArgPtr New(BlockBase parent, string name)
        {
            var ret = new ArgPtr();
            ret.init1(parent, name, null);
            return ret;
        }

        protected override void init2()
        {
            type = TypeArray.NewInt(TypeInt.Instance, 1);
        }
    }
}
