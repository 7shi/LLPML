using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Invoke : Call
    {
        public Invoke()
        {
        }

        public Invoke(BlockBase parent, string name, params IIntValue[] args)
            : base(parent, name, args)
        {
        }

        public Invoke(Invoke src, IIntValue target)
            : base(src.parent, src.name)
        {
            this.args.Add(target);
            this.args.AddRange(src.args);
        }

        public Invoke(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        private bool initialized = false;

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            if (!initialized && args.Count > 0)
            {
                IIntValue v = args[0];
                string type = null;
                if (v is Struct.MemberPtr)
                {
                    type = (v as Struct.MemberPtr).Type;
                }
                else if (v is Struct.Member)
                {
                    Member mem = v as Struct.Member;
                    type = mem.Type;
                    args[0] = new MemberPtr(mem);
                }
                else if (v is Var)
                {
                    type = (v as Var).Type;
                }
                else if (v is Pointer)
                {
                    type = (v as Pointer).Type;
                }
                if (type == null)
                    throw Abort("struct instance or pointer required: " + name);
                Define st2 = parent.GetStruct(type);
                if (st2 == null)
                    throw Abort("undefined struct: " + type);
                Method target = st2.GetMethod(name);
                if (target == null)
                    throw Abort("undefined method: " + st2.GetMemberName(name));
                name = target.Name;
                initialized = true;
            }
            base.AddCodes(codes, m);
        }
    }
}
