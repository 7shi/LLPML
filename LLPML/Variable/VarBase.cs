using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class VarBase : NodeBase
    {
        public VarBase() { }
        public VarBase(BlockBase parent) : base(parent) { }
        public VarBase(BlockBase parent, string name) : base(parent, name) { }
        public VarBase(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public abstract string Type { get; }
        public abstract int TypeSize { get; }
        public abstract bool IsArray { get; }
        public abstract Struct.Define GetStruct();
        public abstract Addr32 GetAddress(List<OpCode> codes, Module m);
    }
}
