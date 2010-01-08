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
                    case "arg":
                        args.Add(new Arg(this, xr));
                        return;
                    case "arg-ptr":
                        args.Add(new ArgPtr(this, xr));
                        return;
                }
            }
            base.ReadBlock(xr);
        }

        protected virtual void ReadName(XmlTextReader xr)
        {
            RequiresName(xr);
        }

        public override void Read(XmlTextReader xr)
        {
            ReadName(xr);

            type = CallType.CDecl;
            if (xr["type"] == "std") type = CallType.Std;

            parent.AddFunction(this);

            base.Read(xr);
        }

        private ushort argStack;
        public override bool HasStackFrame { get { return true; } }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            argStack = 0;
            foreach (DeclareBase arg in args)
            {
                arg.Address = new Addr32(Reg32.EBP, argStack + 8);
                argStack += 4;
            }

            for (int i = 0; i < sentences.Count; i++)
            {
                NodeBase n = sentences[i];
                if (n is Return)
                    (n as Return).IsLast = i == sentences.Count - 1;
            }

            base.AddCodes(codes, m);
        }

        protected override void AfterAddCodes(List<OpCode> codes, Module m)
        {
            codes.Add(I386.Leave());
            if (type == CallType.Std && argStack > 0)
            {
                codes.Add(I386.Ret(argStack));
            }
            else
            {
                codes.Add(I386.Ret());
            }
        }
    }
}
