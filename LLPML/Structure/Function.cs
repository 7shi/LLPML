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
        public bool IsStatic { get; protected set; }

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
                    var ovrfunc = new Function(parent, "override_" + name, IsStatic);
                    ovrfunc.SetOverride(this);
                    if (!parent.AddFunction(ovrfunc))
                        throw Abort("multiple definitions: " + ovrfunc.Name);
                    virtptr = new Var.Declare(
                        parent, "virtual_" + name,
                        null, /// todo: delegate type
                        new Function.Ptr(ovrfunc));
                    parent.AddSentence(virtptr);
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
                    var ovrfunc = new Function(parent, "override_" + name, IsStatic);
                    ovrfunc.SetOverride(this);
                    if (!parent.AddFunction(ovrfunc))
                        throw Abort("multiple definitions: " + ovrfunc.Name);
                    ovrptr = new Var(parent, "virtual_" + name);
                    var setvp = new Set(parent, ovrptr, new Function.Ptr(ovrfunc));
                    parent.AddSentence(setvp);
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

        public Function() { }
        public Function(BlockBase parent) : this(parent, "", false) { }

        public Function(BlockBase parent, string name, bool isStatic)
            : base(parent)
        {
            if (string.IsNullOrEmpty(name))
            {
                isAnonymous = true;
                name = this.parent.GetAnonymousFunctionName();
            }
            this.name = name;
            CallType = CallType.CDecl;

            IsStatic = isStatic;
            CheckThisArg();
            CheckAnonymousMember();
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
                isAnonymous = true;
                name = parent.GetAnonymousFunctionName();
            }

            if (xr["static"] == "1")
                IsStatic = true;

            if (xr["virtual"] == "1")
                IsVirtual = true;
            else if (xr["override"] == "1")
                IsOverride = true;

            CheckThisArg();
            CheckAnonymousMember();

            CallType = CallType.CDecl;
            if (xr["type"] == "std") CallType = CallType.Std;

            if (!parent.AddFunction(this))
                throw Abort(xr, "multiple definitions: " + name);

            base.Read(xr);
        }

        protected bool isAnonymous = false;

        public override int Level
        {
            get
            {
                if (isAnonymous) return 1;
                return base.Level;
            }
        }

        private ushort argStack;
        public override bool HasStackFrame { get { return true; } }

        public override void AddCodes(OpCodes codes)
        {
            if (IsVirtual)
            {
                var st = parent as Struct.Define;
                var offset = st.GetOffset(virtptr.Name);
                codes.AddRange(new[]
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
            var name = this.name;
            if (virtfunc != null) name = virtfunc.name;
            if (thisptr != null && name == Struct.Define.Destructor)
                ThisStruct.AddAfterDtor(codes);
            AddExitCodes(codes);
        }

        public override void AddExitCodes(OpCodes codes)
        {
            if (thisptr != null && name == Struct.Define.Constructor)
                thisptr.AddCodes(codes, "mov", null);
            else if (retVal != null)
                GetRetVal(this).AddCodes(codes, "mov", null);
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

        public void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            codes.AddCodes(op, dest, GetAddress(codes.Module));
        }

        private TypeBase GetParentType()
        {
            return new TypeReference((parent as Struct.Define).Type);
        }

        public void SetReturnType(TypeBase type)
        {
            doneInferReturnType = true;
            this.returnType = type;
        }

        protected TypeBase type;
        protected bool doneInferType = false;

        public TypeBase Type
        {
            get
            {
                if (doneInferType || !root.IsCompiling)
                    return type;

                doneInferType = true;
                type = new TypeFunction(this);
                return type;
            }
        }

        protected List<Var.Declare> autoArgs = new List<Var.Declare>();
        public Var.Declare[] GetAutoArgs()
        {
            if (autoArgs.Count == 0) return null;
            return autoArgs.ToArray();
        }

        protected void InsertArg(Arg arg)
        {
            args.Insert(0, arg);
            autoArgs.Insert(0, arg);
        }

        public override Var.Declare GetVar(string name)
        {
            if (!isAnonymous) return base.GetVar(name);

            var v = GetMember<Var.Declare>(name);
            if (v != null) return v;

            var vp = parent.GetVar(name);
            if (vp == null || vp.IsMember || vp.IsStatic) return vp;

            var arg = new Arg(this, name, vp.Type);
            InsertArg(arg);
            return arg;
        }

        protected void CheckThisArg()
        {
            if (this.parent is Struct.Define && !IsStatic)
            {
                args.Add(new Arg(this, "this", GetParentType()));
                thisptr = new Struct.This(this);
            }
        }

        protected void CheckAnonymousMember()
        {
            if (!isAnonymous || HasThis) return;

            var f = parent as Function;
            if (f == null || !f.HasThis) return;

            InsertArg(new Arg(this, "this", f.thisptr.Type));
            thisptr = new Struct.This(this);
        }
    }
}
