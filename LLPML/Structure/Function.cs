using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Function : Block, IIntValue
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
            if (string.IsNullOrEmpty(name))
            {
                parent = root;
                name = parent.GetAnonymousFunctionName();
            }
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
            name = xr["name"];
            if (string.IsNullOrEmpty(name))
            {
                parent = root;
                name = parent.GetAnonymousFunctionName();
            }

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

        protected override void BeforeAddCodes(OpCodes codes)
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

            base.BeforeAddCodes(codes);
            if (thisptr == null) return;

            switch (name)
            {
                case Struct.Define.Initializer:
                    ThisStruct.AddInit(codes, new Addr32(Reg32.EBP, 8));
                    break;
                case Struct.Define.Constructor:
                    ThisStruct.AddBeforeCtor(codes);
                    break;
            }
        }

        protected override void AfterAddCodes(OpCodes codes)
        {
            if (thisptr != null && name == Struct.Define.Destructor)
                ThisStruct.AddAfterDtor(codes);
            AddExitCodes(codes);
        }

        public override void AddExitCodes(OpCodes codes)
        {
            if (thisptr != null && name == Struct.Define.Constructor)
                thisptr.AddCodes(codes, "mov", null);
            else if (members.ContainsKey("__retval"))
            {
                var retval = new Var(this, "__retval");
                retval.AddCodes(codes, "mov", null);
            }
            base.AddExitCodes(codes);
            if (CallType == CallType.Std && argStack > 0)
                codes.Add(I386.Ret(argStack));
            else
                codes.Add(I386.Ret());
        }

        public Val32 GetAddress(Module m)
        {
            return new Val32(m.Specific.ImageBase, First);
        }

        public TypeBase Type { get { return null; } }

        public void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            codes.AddCodes(op, dest, GetAddress(codes.Module));
        }
    }
}
