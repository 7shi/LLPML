using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public partial class I386
    {
        // Call

        public static OpCode Call(Reg32 op1)
        {
            switch (op1)
            {
                case Reg32.EAX:
                    return OpCode.NewBytes(Util.GetBytes2(0xff, 0xd0));
            }
            throw new Exception("The method or operation is not implemented.");
        }

        public static OpCode CallA(Addr32 op1)
        {
            return OpCode.NewA(Util.GetBytes1(0xff), Addr32.NewAdM(op1, 2));
        }

        public static OpCode CallD(Val32 op1)
        {
            return OpCode.NewDRel(Util.GetBytes1(0xe8), op1, true);
        }

        public static OpCode[] CallArgs(CallType call, Addr32 func, object[] args)
        {
            var list = new ArrayList();
            for (int i = args.Length - 1; i >= 0; i--)
            {
                var arg = args[i];
                if (arg is int) list.Add(PushD(Val32.NewI((int)arg)));
                else if (arg is uint) list.Add(PushD(Val32.New((uint)arg)));
                else if (arg is Val32) list.Add(PushD((Val32)arg));
                else if (arg is Addr32) list.Add(PushA((Addr32)arg));
                else throw new Exception("Unknown argument.");
            }
            list.Add(CallA(func));
            if (call == CallType.CDecl)
            {
                list.Add(AddR(Reg32.ESP, Val32.New((byte)(args.Length * 4))));
            }
            var ret = new OpCode[list.Count];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = list[i] as OpCode;
            return ret;
        }
    }
}
