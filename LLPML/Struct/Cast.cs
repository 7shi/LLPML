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
        public IIntValue Source { get; private set; }

        private string type;
        public override string TypeName { get { return type; } }

        public override int TypeSize
        {
            get { return SizeOf.GetTypeSize(parent, type); }
        }

        private bool isArray = false;
        public override bool IsArray { get { return isArray; } }

        private void SetType(string type)
        {
            if (type.EndsWith("[]"))
            {
                this.type = type.Substring(0, type.Length - 2).TrimEnd();
                isArray = true;
            }
            else
            {
                this.type = type;
                isArray = false;
            }
        }

        public Cast(BlockBase parent, string type, IIntValue source)
        {
            this.parent = parent;
            name = "__cast";
            SetType(type);
            Source = source;
        }

        public Cast(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            name = "__cast";

            SetType(xr["type"]);
            if (type == null)
                throw Abort(xr, "requires type");

            Parse(xr, delegate
            {
                IIntValue[] v = IntValue.Read(parent, xr);
                if (v != null)
                {
                    if (v.Length > 1 || Source != null)
                        throw Abort(xr, "too many sources");
                    Source = v[0];
                }
            });

            if (Source == null)
                throw Abort(xr, "requires a source");
        }

        public override Addr32 GetAddress(OpCodes codes)
        {
            if (Source is Pointer)
                return (Source as Pointer).GetAddress(codes);
            else if (Source is Struct.Member)
                return (Source as Struct.Member).GetAddress(codes);

            Source.AddCodes(codes, "mov", null);
            codes.Add(I386.Mov(Reg32.EDX, Reg32.EAX));
            return new Addr32(Reg32.EDX);
        }

        public override void GetValue(OpCodes codes)
        {
            Source.AddCodes(codes, "mov", null);
        }
    }
}
