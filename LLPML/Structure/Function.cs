using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.LLPML.Struct;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Function : Block
    {
        public CallType CallType { get; set; }
        public bool IsStatic { get; protected set; }

        protected ArrayList args = new ArrayList();
        public ArrayList Args { get { return args; } }

        private Var thisptr;
        public bool HasThis { get { return thisptr != null; } }

        protected Function virtfunc;
        protected VarDeclare virtptr;

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
                else if (Parent is Define)
                {
                    var ovrfunc = Function.New(Parent, "override_" + name, IsStatic);
                    ovrfunc.SetOverride(this);
                    if (!Parent.AddFunction(ovrfunc))
                        throw Abort("multiple definitions: " + ovrfunc.Name);
                    virtptr = VarDeclare.New(
                        Parent, "virtual_" + name,
                        null /// todo: delegate type
                        );
                    virtptr.Value = Variant.New(ovrfunc);
                    Parent.AddSentence(virtptr);
                }
                else
                    throw Abort("can not make virtual");
            }
        }

        protected Function ovrfunc;
        protected Var ovrptr;

        public bool IsOverride
        {
            get { return ovrfunc != null; }

            set
            {
                if (!value)
                {
                    if (ovrptr != null)
                        throw Abort("can not remove override");
                }
                else if (Parent is Define)
                {
                    ovrfunc = Function.New(Parent, "override_" + name, IsStatic);
                    ovrfunc.SetOverride(this);
                    if (!Parent.AddFunction(ovrfunc))
                        throw Abort("multiple definitions: " + ovrfunc.Name);
                }
                else
                    throw Abort("can not make override");
            }
        }

        protected void SetOverride(Function virtfunc)
        {
            this.virtfunc = virtfunc;
            retVal = virtfunc.retVal;
            args = virtfunc.args;
            members = virtfunc.members;
            sentences = virtfunc.sentences;
            SrcInfo = virtfunc.SrcInfo;
            construct = virtfunc.construct;
            destruct = virtfunc.destruct;
            last = virtfunc.last;
        }

        public static Function New(BlockBase parent, string name, bool isStatic)
        {
            var ret = new Function();
            ret.init2(parent, name, isStatic);
            return ret;
        }

        protected void init2(BlockBase parent, string name, bool isStatic)
        {
            init(parent);
            if (string.IsNullOrEmpty(name))
            {
                isAnonymous = true;
                this.name = Parent.GetAnonymousName();
            }
            else
                this.name = name;
            CallType = CallType.CDecl;
            IsStatic = isStatic;
            CheckThisArg();
            CheckAnonymousMember();
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

        public override void AddCodes(OpModule codes)
        {
            if (IsVirtual)
            {
                var st = Parent as Define;
                var offset = st.GetOffset(virtptr.Name);
                codes.Add(first);
                codes.Add(I386.MovRA(Reg32.EAX, Addr32.NewRO(Reg32.ESP, 4)));
                codes.Add(I386.Jmp(Addr32.NewRO(Reg32.EAX, offset)));
            }
            else if (!IsOverride)
            {
                base.AddCodes(codes);
            }
        }

        protected bool ArgNeededGC(VarDeclare arg)
        {
            if (arg.Name == "this") return false;
            var tr = arg.Type as TypeReference;
            return tr != null && tr.UseGC;
        }

        protected override void BeforeAddCodes(OpModule codes)
        {
            argStack = 0;
            for (int i = 0; i < args.Count; i++)
            {
                var arg = args[i] as VarDeclare;
                arg.Address = Addr32.NewRO(Reg32.EBP, argStack + 8);
                argStack += 4;
            }

            for (int i = 0; i < sentences.Count; i++)
            {
                var n = sentences[i] as NodeBase;
                if (n is Return)
                    (n as Return).IsLast = i == sentences.Count - 1;
            }

            base.BeforeAddCodes(codes);
            for (int i = 0; i < args.Count; i++)
            {
                var arg = args[i] as VarDeclare;
                if (!arg.Type.Check())
                    throw arg.Abort("undefined type: {0}: {1}", arg.Name, arg.Type.Name);
                if (ArgNeededGC(arg))
                {
                    codes.Add(I386.MovRA(Reg32.EAX, arg.Address));
                    TypeReference.AddReferenceCodes(codes);
                }
            }
            if (thisptr == null) return;

            switch (name)
            {
                case Define.Initializer:
                    ThisStruct.AddInit(codes, Addr32.NewRO(Reg32.EBP, 8));
                    break;
                case Define.Constructor:
                    ThisStruct.AddBeforeCtor(codes);
                    break;
            }
        }

        protected override void AfterAddCodes(OpModule codes)
        {
            var name = this.name;
            if (virtfunc != null) name = virtfunc.name;
            if (thisptr != null && name == Define.Destructor)
                ThisStruct.AddAfterDtor(codes);
            AddExitCodes(codes);
        }

        public override void AddDestructors(OpModule codes, VarDeclare[] ptrs)
        {
            base.AddDestructors(codes, ptrs);

            var args2 = args.ToArray();
            for (int i = args2.Length - 1; i >= 0; i--)
            {
                var arg = args2[i] as VarDeclare;
                if (ArgNeededGC(arg))
                    arg.Type.AddDestructorA(codes, arg.GetAddress(codes, this));
            }
        }

        public override void AddExitCodes(OpModule codes)
        {
            if (thisptr != null && name == Define.Constructor)
                thisptr.AddCodesV(codes, "mov", null);
            else if (retVal != null)
                GetRetVal(this).AddCodesV(codes, "mov", null);
            base.AddExitCodes(codes);
            if (CallType == CallType.Std && argStack > 0)
                codes.Add(I386.RetW(argStack));
            else
                codes.Add(I386.Ret());
        }

        public Val32 GetAddress(Module m)
        {
            return Val32.New2(Val32.New(m.Specific.ImageBase), First);
        }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            codes.AddCodesV(op, dest, codes.GetAddress(this));
        }

        public void SetReturnType(TypeBase type)
        {
            doneInferReturnType = true;
            this.returnType = type;
        }

        protected TypeBase type;
        protected bool doneInferType = false;

        public override TypeBase Type
        {
            get
            {
                if (doneInferType || !root.IsCompiling)
                    return type;

                doneInferType = true;
                type = TypeFunction.New(this);
                return type;
            }
        }

        protected ArrayList autoArgs = new ArrayList();
        public VarDeclare[] GetAutoArgs()
        {
            if (autoArgs.Count == 0) return null;
            var ret = new VarDeclare[autoArgs.Count];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = autoArgs[i] as VarDeclare;
            return ret;
        }

        protected void InsertArg(Arg arg)
        {
            args.Insert(0, arg);
            autoArgs.Insert(0, arg);
        }

        public override VarDeclare GetVar(string name)
        {
            if (!isAnonymous) return base.GetVar(name);

            var v = GetMember(name) as VarDeclare;
            if (v != null) return v;

            var vp = Parent.GetVar(name);
            if (vp == null || vp.IsMember || vp.IsStatic) return vp;

            var arg = Arg.NewVar(this, vp);
            InsertArg(arg);
            return arg;
        }

        protected void CheckThisArg()
        {
            if (Parent is Define && !IsStatic)
            {
                var type = Types.ToVarType((Parent as Define).Type);
                args.Add(Arg.New(this, "this", type));
                thisptr = This.New(this);
            }
        }

        protected void CheckAnonymousMember()
        {
            if (!isAnonymous || HasThis) return;

            var f = Parent as Function;
            if (f == null || !f.HasThis) return;

            InsertArg(Arg.New(this, "this", f.thisptr.Type));
            thisptr = This.New(this);
        }

        protected override void MakeUpInternal()
        {
            if (HasThis && CallType != CallType.CDecl &&
                (name == Define.Constructor
                || name == Define.Destructor))
            {
                throw Abort("{0}: must be __cdecl", FullName);
            }
            if (IsOverride)
            {
                var st = (Parent as Define).GetBaseStruct();
                Function vf = null;
                if (st != null) vf = st.GetFunction(name);
                if (vf == null || (!vf.IsVirtual && !vf.IsOverride))
                    throw Abort("can not find virtual: {0}", name);
                first = vf.first;
                ovrptr = Var.NewName(Parent, "virtual_" + name);
                var setvp = Set.New(Parent, ovrptr, Variant.New(ovrfunc));
                Parent.AddSentence(setvp);
            }
        }
    }
}
