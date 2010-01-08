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
        public CallType CallType { get; set; }

        protected List<DeclareBase> args
            = new List<DeclareBase>();
        public List<DeclareBase> Args { get { return args; } }

        private Var thisptr;
        public bool HasThis { get { return thisptr != null; } }

        public Function()
        {
        }

        public Function(BlockBase parent, string name)
            : base(parent)
        {
            this.name = name;
            CallType = CallType.CDecl;

            if (parent is Struct.Define)
            {
                args.Add(new Arg(this, "this", parent.Name));
                thisptr = new Struct.This(this);
            }
        }

        public Function(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

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

        public override void Read(XmlTextReader xr)
        {
            RequiresName(xr);

            if (parent is Struct.Define)
            {
                args.Add(new Arg(this, "this", parent.Name));
                thisptr = new Struct.This(this);
            }

            CallType = CallType.CDecl;
            if (xr["type"] == "std") CallType = CallType.Std;

            if (!parent.AddFunction(this))
                throw Abort(xr, "multiple definitions: " + name);

            base.Read(xr);
        }

        private ushort argStack;
        public override bool HasStackFrame { get { return true; } }

        protected override void BeforeAddCodes(List<OpCode> codes, Module m)
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

            base.BeforeAddCodes(codes, m);

            if (thisptr != null && name == "ctor")
                ThisStruct.AddBeforeCtor(codes, m, thisptr);
        }

        protected override void AfterAddCodes(List<OpCode> codes, Module m)
        {
            if (thisptr != null && name == "dtor")
                ThisStruct.AddAfterDtor(codes, m, thisptr);
            AddExitCodes(codes, m);
        }

        public override void AddExitCodes(List<OpCode> codes, Module m)
        {
            if (thisptr != null && name == "ctor")
            {
                (thisptr as IIntValue).AddCodes(codes, m, "mov", null);
            }
            else if (members.ContainsKey("__retval"))
            {
                IIntValue retval = new Var(this, "__retval") as IIntValue;
                retval.AddCodes(codes, m, "mov", null);
            }
            base.AddExitCodes(codes, m);
            if (CallType == CallType.Std && argStack > 0)
                codes.Add(I386.Ret(argStack));
            else
                codes.Add(I386.Ret());
        }
    }
}
