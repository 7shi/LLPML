using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Delegate : NodeBase
    {
        public const string Alloc = "__jit_alloc";
        public const string Free = "__jit_free";
        public const string Duplicate = "__jit_dup";
        public const int DefaultSize = 64;

        public static TypeFunction GetDefaultType(BlockBase parent)
        {
            return TypeDelegate.New(parent.Root, CallType.CDecl, TypeVar.Instance, null);
        }

        public NodeBase[] Args { get; protected set; }
        public NodeBase Function { get; protected set; }
        public CallType CallType { get; protected set; }
        public bool Auto { get; set; }

        public Delegate(BlockBase parent, CallType callType, NodeBase[] args)
        {
            Parent = parent;
            var len = args.Length;
            if (len < 1)
                throw Abort("delegate: arguments required");
            Args = new NodeBase[len - 1];
            Array.Copy(args, Args, len - 1);
            Function = args[len - 1];
            CallType = callType;
        }

        public Delegate(BlockBase parent, CallType callType, NodeBase[] args, NodeBase func)
        {
            Parent = parent;
            Args = args;
            Function = func;
            CallType = callType;
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
                var f = Function.Type as TypeFunction;
                if (f == null)
                    throw Abort("delegate: invalid target");
                var fargs = f.Args;
                var len = fargs.Length - Args.Length;
                if (len < 0)
                    throw Abort("delegate: argument mismatched");
                var args = new VarDeclare[len];
                Array.Copy(fargs, Args.Length, args, 0, len);
                type = TypeDelegate.New(Parent.Root, CallType, f.RetType, args);
                return type;
            }
        }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            AddCodes(codes);
            codes.AddCodes(op, dest);
        }

        public override void AddCodes(OpModule codes)
        {
            var f = Function.Type as TypeFunction;
            if (f == null)
                throw Abort("delegate: invalid target");

            var fargs = f.Args;
            var len = fargs.Length - Args.Length;
            if (len < 0)
                throw Abort("delegate: argument mismatched");

            int length = Args.Length * 5 + 8;
            if (len > 0) length += 11;
            if (f.CallType == CallType.CDecl) length += 6;
            if (CallType == CallType.Std) length += 2;
            if (length > 64)
                throw Abort("delegate: too many arguments");

            var alloc = Parent.Root.GetFunction(Alloc);
            if (alloc == null)
                throw Abort("delegate: can not find: {0}", Alloc);
            Call.AddCallCodes(codes, alloc, new[] { IntValue.New(DefaultSize) });

            codes.Add(I386.Push(Reg32.EDI));
            codes.Add(I386.Mov(Reg32.EDI, Reg32.EAX));
            int p = 0;
            if (len > 0)
            {
                //   mov ecx, len
                codes.Add(I386.MovBA(Addr32.New(Reg32.EDI), 0xb9));
                codes.Add(I386.MovA(Addr32.NewRO(Reg32.EDI, 1), Val32.NewI(len)));
                // push_arg:
                //   push dword [esp + (len * 4)]
                codes.Add(I386.MovA(Addr32.NewRO(Reg32.EDI, 5),
                    Val32.NewI((0x2474FF | ((len * 4) << 24)))));
                //   loop push_arg
                codes.Add(I386.MovWA(Addr32.NewRO(Reg32.EDI, 9), 0xfae2));
                p = 11;
            }
            var args = Args.Clone() as NodeBase[];
            Array.Reverse(args);
            foreach (var arg in args)
            {
                arg.AddCodesV(codes, "mov", null);
                // push DWORD
                codes.Add(I386.MovBA(Addr32.NewRO(Reg32.EDI, p), 0x68));
                codes.Add(I386.MovAR(Addr32.NewRO(Reg32.EDI, p + 1), Reg32.EAX));
                p += 5;
            }
            Function.AddCodesV(codes, "mov", null);
            // mov eax, DWORD
            codes.Add(I386.MovBA(Addr32.NewRO(Reg32.EDI, p), 0xb8));
            codes.Add(I386.MovAR(Addr32.NewRO(Reg32.EDI, p + 1), Reg32.EAX));
            // call eax
            codes.Add(I386.MovWA(Addr32.NewRO(Reg32.EDI, p + 5), 0xd0ff));
            p += 7;
            if (f.CallType == CallType.CDecl)
            {
                // add esp, (fargs.Length * 4)
                codes.Add(I386.MovWA(Addr32.NewRO(Reg32.EDI, p), 0xc481));
                codes.Add(I386.MovA(Addr32.NewRO(Reg32.EDI, p + 2), Val32.NewI((fargs.Length * 4))));
                p += 6;
            }
            if (CallType == CallType.CDecl)
            {
                // ret
                codes.Add(I386.MovBA(Addr32.NewRO(Reg32.EDI, p), 0xc3));
            }
            else
            {
                // ret (len * 4)
                codes.Add(I386.MovBA(Addr32.NewRO(Reg32.EDI, p), 0xc2));
                codes.Add(I386.MovWA(Addr32.NewRO(Reg32.EDI, p + 1), (ushort)(len * 4)));
            }
            codes.Add(I386.Mov(Reg32.EAX, Reg32.EDI));
            codes.Add(I386.Pop(Reg32.EDI));
        }
    }
}
