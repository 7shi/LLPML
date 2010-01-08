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

        protected List<Var.Declare> args
            = new List<Var.Declare>();
        public List<Var.Declare> Args { get { return args; } }

        private Var thisptr;
        public bool HasThis { get { return thisptr != null; } }

        protected Function virtfunc;

        protected Var.Declare virtptr;
        public bool IsVirtual
        {
            get { return virtptr != null; }

            set
            {
                if (!value)
                {
                    if (virtptr != null)
                        throw Abort("can not remove virtual");
                }
                else if (parent is Struct.Define)
                {
                    var ovrfunc = new Function(parent, "override_" + name);
                    ovrfunc.SetOverride(this);
                    if (!parent.AddFunction(ovrfunc))
                        throw Abort("multiple definitions: " + ovrfunc.Name);
                    virtptr = new Var.Declare(
                        parent, "virtual_" + name,
                        null, /// todo: delegate type
                        new Function.Ptr(ovrfunc));
                    parent.Sentences.Add(virtptr);
                }
                else
                    throw Abort("can not make virtual");
            }
        }

        protected Var ovrptr;
        public bool IsOverride
        {
            get { return ovrptr != null; }

            set
            {
                if (!value)
                {
                    if (ovrptr != null)
                        throw Abort("can not remove override");
                }
                else if (parent is Struct.Define)
                {
                    var st = (parent as Struct.Define).GetBaseStruct();
                    Function vf = null;
                    if (st != null) vf = st.GetFunction(name);
                    if (vf == null || (!vf.IsVirtual && !vf.IsOverride))
                        throw Abort("can not find virtual: {0}", name);
                    first = vf.first;
                    var ovrfunc = new Function(parent, "override_" + name);
                    ovrfunc.SetOverride(this);
                    if (!parent.AddFunction(ovrfunc))
                        throw Abort("multiple definitions: " + ovrfunc.Name);
                    ovrptr = new Var(parent, "virtual_" + name);
                    var setvp = new Set(parent, ovrptr, new Function.Ptr(ovrfunc));
                    parent.Sentences.Add(setvp);
                }
                else
                    throw Abort("can not make override");
            }
        }

        protected void SetOverride(Function virtfunc)
        {
            this.virtfunc = virtfunc;
            args = virtfunc.args;
            members = virtfunc.members;
            sentences = virtfunc.sentences;
            SrcInfo = virtfunc.SrcInfo;
            construct = virtfunc.construct;
            destruct = virtfunc.destruct;
            last = virtfunc.last;
        }

        public Function()
        {
        }

        public Function(BlockBase parent, string name)
            : base(parent)
        {
            if (string.IsNullOrEmpty(name))
            {
                this.parent = root;
                name = this.parent.GetAnonymousFunctionName();
            }
            this.name = name;
            CallType = CallType.CDecl;

            if (this.parent is Struct.Define)
            {
                args.Add(new Arg(this, "this", GetParentType()));
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

            if (xr["virtual"] == "1")
                IsVirtual = true;
            else if (xr["override"] == "1")
                IsOverride = true;

            if (parent is Struct.Define)
            {
                args.Add(new Arg(this, "this", GetParentType()));
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

        public override void AddCodes(OpCodes codes)
        {
            if (IsVirtual)
            {
                var st = parent as Struct.Define;
                var offset = st.GetOffset(virtptr.Name);
                codes.AddRange(new OpCode[]
                {
                    first,
                    I386.Mov(Reg32.EAX, new Addr32(Reg32.ESP, 4)),
                    I386.Jmp(new Addr32(Reg32.EAX, offset))
                });
            }
            else if (!IsOverride)
            {
                base.AddCodes(codes);
            }
        }

        protected override void BeforeAddCodes(OpCodes codes)
        {
            argStack = 0;
            foreach (var arg in args)
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

        private TypeBase GetParentType()
        {
            return new TypeReference((parent as Struct.Define).Type);
        }
    }
}
