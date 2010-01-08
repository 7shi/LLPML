using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Cast : Pointer
    {
        private IIntValue source;
        private string type;

        public Cast(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            name = "__cast";

            type = xr["type"];
            if (type == null)
                throw Abort(xr, "requires type");

            Parse(xr, delegate
            {
                IIntValue v = IntValue.Read(parent, xr, true);
                if (v != null)
                {
                    if (source != null)
                        throw Abort(xr, "too many sources");
                    source = v;
                }
            });

            if (source == null)
                throw Abort(xr, "requires a source");
        }

        public override string Type
        {
            get
            {
                return type;
            }
        }

        public override void GetValue(List<OpCode> codes, Module m)
        {
            source.AddCodes(codes, m, "mov", null);
        }
    }
}
