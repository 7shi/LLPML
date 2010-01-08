using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Function : Block
    {
        protected CallType type;
        public CallType Type { get { return type; } }

        protected OpCode entry = new OpCode();
        public Val32 Address { get { return entry.Address; } }

        protected List<DeclareBase> args
            = new List<DeclareBase>();

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
                    case "return":
                        sentences.Add(new Return(this, xr));
                        return;
                }
            }
            base.ReadBlock(xr);
        }

        public override void Read(XmlTextReader xr)
        {
            RequireName(xr);

            type = CallType.CDecl;
            if (xr["type"] == "std") type = CallType.Std;

            parent.AddFunction(this);

            base.Read(xr);
        }

        private ushort stack;

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            codes.Add(entry);

            stack = 0;
            foreach (DeclareBase arg in args)
            {
                arg.Address = new Addr32(Reg32.EBP, stack + 8);
                stack += 4;
            }

            NodeBase nlast = null;
            foreach (NodeBase n in sentences)
            {
                if (n is Return) (n as Return).IsLast = false;
                nlast = n;
            }
            if (nlast is Return) (nlast as Return).IsLast = true;

            base.AddCodes(codes, m);
        }

        protected override void AfterAddCodes(List<OpCode> codes, Module m)
        {
            foreach (VarInt.Declare v in var_ints.Values)
            {
                if (v.Name == "__retval") codes.Add(I386.Mov(Reg32.EAX, v.Address));
            }
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
