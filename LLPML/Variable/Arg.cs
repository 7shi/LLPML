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
        public Arg(BlockBase parent, string name, TypeBase type) : base(parent, name, type) { }

        private VarDeclare target;

        public Arg(BlockBase parent, VarDeclare target)
            : base(parent, target.Name)
        {
            this.target = target;
        }

        protected override void Init() { }

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
        public ArgPtr(BlockBase parent, string name) : base(parent, name, null) { }

        protected override void Init()
        {
            type = TypeArray.NewInt(TypeInt.Instance, 1);
        }
    }
}
