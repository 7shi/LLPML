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
        TypeBase Type { get; }
        void AddCodes(OpModule codes, string op, Addr32 dest);
    }
}
