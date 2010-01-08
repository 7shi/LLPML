using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public interface IIntValue
    {
        void AddCodes(List<OpCode> codes, Module m, string op, Addr32 dest);
    }
}
