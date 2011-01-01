using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Extern : Function
    {
        private string module, alias;

        public Extern()
        {
        }

        public Extern(BlockBase parent, string name, string module, string alias)
            : base(parent, name, false)
        {
            this.module = module;
            this.alias = alias;
        }

        public Extern(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        protected override void ReadBlock(XmlTextReader xr)
        {
            switch (xr.NodeType)
            {
                case XmlNodeType.Element:
                    switch (xr.Name)
                    {
                        case "arg":
                            args.Add(new Arg(this, xr));
                            return;
                        case "arg-ptr":
                            args.Add(new ArgPtr(this, xr));
                            return;
                        default:
                            throw Abort(xr, "extern: invalid element: {0}", xr.Name);
                    }

                case XmlNodeType.Whitespace:
                case XmlNodeType.Comment:
                    break;

                default:
                    throw Abort(xr, "extern: argument required");
            }
        }

        public override void Read(XmlTextReader xr)
        {
            module = xr["module"];
            alias = xr["alias"];
            var suffix = xr["suffix"];
            base.Read(xr);

            if (suffix != null)
            {
                if (alias == null)
                    alias = name + suffix;
                else
                    alias += suffix;
            }
        }

        public override void AddCodes(OpModule codes)
        {
            codes.Add(first);
            string n = alias != null ? alias : name;
            codes.Add(I386.Jmp(codes.Module.GetFunction(module, n)));
        }
    }
}
