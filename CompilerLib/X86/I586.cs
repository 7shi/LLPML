using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public class I586
    {
        public static OpCode Cpuid()
        {
            return new OpCode(Util.GetBytes2(0x0f, 0xa2));
        }
    }
}
