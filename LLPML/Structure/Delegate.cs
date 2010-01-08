using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Delegate : NodeBase, IIntValue
    {
        public const string Alloc = "__jit_alloc";
        public const string Free = "__jit_free";
        public const string Duplicate = "__jit_dup";
        public const int DefaultSize = 64;

        public static TypeFunction GetDefaultType(BlockBase parent)
        {
            return new TypeDelegate(parent.Root, CallType.CDecl, TypeVar.Instance, null);
        }

        public IIntValue[] Args { get; protected set; }
        public IIntValue Function { get; protected set; }
        public CallType CallType { get; protected set; }

        public Delegate(BlockBase parent, CallType callType, IIntValue[] args)
            : base(parent)
        {
            var len = args.Length;
            if (len < 1)
                throw Abort("delegate: arguments required");
            Args = new IIntValue[len - 1];
            Array.Copy(args, Args, len - 1);
            Function = args[len - 1];
            CallType = callType;
        }

        public Delegate(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            CallType = CallType.CDecl;
            if (xr["type"] == "std") CallType = CallType.Std;

            var args = new List<IIntValue>();
            IIntValue last = null;
            Parse(xr, delegate
            {
                var vs = IntValue.Read(parent, xr);
                if (vs == null) return;
                foreach (var v in vs)
                {
                    if (last != null) args.Add(last);
                    last = v;
                }
            });
            if (last == null)
                throw Abort("delegate: arguments required");
            Args = args.ToArray();
            Function = last;
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
                var f = Function.Type as TypeFunction;
                if (f == null)
                    throw Abort("delegate: invalid target");
                var fargs = f.Args;
                var len = fargs.Length - Args.Length;
                if (len < 0)
                    throw Abort("delegate: argument mismatched");
                var args = new Var.Declare[len];
                Array.Copy(fargs, Args.Length, args, 0, len);
                type = new TypeDelegate(parent.Root, CallType, f.RetType, args);
                return type;
            }
        }

        public void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            AddCodes(codes);
            codes.AddCodes(op, dest);
        }

        public override void AddCodes(OpCodes codes)
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

            var alloc = parent.Root.GetFunction(Alloc);
            if (alloc == null)
                throw Abort("delegate: can not find: {0}", Alloc);
            Call.AddCodes(codes, alloc, new[] { new IntValue(DefaultSize) });

            codes.AddRange(new[]
            {
                I386.Push(Reg32.EDI),
                I386.Mov(Reg32.EDI, Reg32.EAX),
            });
            int p = 0;
            if (len > 0)
            {
                codes.AddRange(new[]
                {
                    //   mov ecx, len
                    I386.MovB(new Addr32(Reg32.EDI), 0xb9),
                    I386.Mov(new Addr32(Reg32.EDI, 1), (uint)len),
                    // push_arg:
                    //   push dword [esp + (len * 4)]
                    I386.Mov(new Addr32(Reg32.EDI, 5),
                        (uint)(0x2474FF | ((len * 4) << 24))),
                    //   loop push_arg
                    I386.MovW(new Addr32(Reg32.EDI, 9), 0xfae2),
                });
                p = 11;
            }
            var args = Args.Clone() as IIntValue[];
            Array.Reverse(args);
            foreach (var arg in args)
            {
                arg.AddCodes(codes, "mov", null);
                codes.AddRange(new[]
                {
                    // push DWORD
                    I386.MovB(new Addr32(Reg32.EDI, p), 0x68),
                    I386.Mov(new Addr32(Reg32.EDI, p + 1), Reg32.EAX),
                });
                p += 5;
            }
            Function.AddCodes(codes, "mov", null);
            codes.AddRange(new[]
            {
                // mov eax, DWORD
                I386.MovB(new Addr32(Reg32.EDI, p), 0xb8),
                I386.Mov(new Addr32(Reg32.EDI, p + 1), Reg32.EAX),
                // call eax
                I386.MovW(new Addr32(Reg32.EDI, p + 5), 0xd0ff),
            });
            p += 7;
            if (f.CallType == CallType.CDecl)
            {
                codes.AddRange(new[]
                {
                    // add esp, (fargs.Length * 4)
                    I386.MovW(new Addr32(Reg32.EDI, p), 0xc481),
                    I386.Mov(new Addr32(Reg32.EDI, p + 2), (uint)(fargs.Length * 4)),
                });
                p += 6;
            }
            if (CallType == CallType.CDecl)
            {
                // ret
                codes.Add(I386.MovB(new Addr32(Reg32.EDI, p), 0xc3));
            }
            else
            {
                // ret (len * 4)
                codes.AddRange(new[]
                {
                    I386.MovB(new Addr32(Reg32.EDI, p), 0xc2),
                    I386.MovW(new Addr32(Reg32.EDI, p + 1), (ushort)(len * 4)),
                });
            }
            codes.AddRange(new[]
            {
                I386.Mov(Reg32.EAX, Reg32.EDI),
                I386.Pop(Reg32.EDI),
            });
        }
    }
}
