using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Function : Block
    {
        protected string name;
        public string Name { get { return name; } }

        protected CallType type;
        public CallType Type { get { return type; } }

        protected OpCode entry = new OpCode();
        public Ptr<uint> Address { get { return entry.Address; } }

        protected List<VarBase> args = new List<VarBase>();

        public Function() { }
        public Function(Block parent, XmlTextReader xr) : base(parent, xr) { }

        protected override void ReadBlock(XmlTextReader xr)
        {
            if (xr.NodeType == XmlNodeType.Element)
            {
                switch (xr.Name)
                {
                    case "arg-int":
                        args.Add(new ArgInt(this, xr));
                        return;
                    case "arg-ptr":
                        args.Add(new ArgPtr(this, xr));
                        return;
                }
            }
            base.ReadBlock(xr);
        }

        public override void Read(XmlTextReader xr)
        {
            type = CallType.CDecl;
            if (xr["type"] == "std") type = CallType.Std;

            name = xr["name"];
            parent.SetFunction(name, this);

            base.Read(xr);
        }

        private ushort stack;

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            codes.Add(entry);
            stack = 0;
            foreach (VarBase arg in args)
            {
                arg.Address = new Addr32(Reg32.EBP, stack + 8);
                stack += 4;
            }
            base.AddCodes(codes, m);
        }

        protected override void AfterAddCodes(List<OpCode> codes, Module m)
        {
            base.AfterAddCodes(codes, m);
            if (type == CallType.Std && stack > 0)
            {
                codes.Add(I386.Ret(stack));
            }
            else
            {
                codes.Add(I386.Ret());
            }
        }
    }
}
