using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML
{
    public class TypeFunction : TypeVarBase
    {
        // type name
        public override string Name
        {
            get { return GetName("function"); }
        }

        public string GetName(string type)
        {
            var sb = new StringBuilder();
            sb.Append(type);
            if (CallType == CallType.Std)
                sb.Append("((stdcall))");
            sb.Append("(");
            bool first = true;
            for (int i = 0; i < Args.Length; i++)
            {
                var arg = Args[i];
                if (first) first = false; else sb.Append(",");
                sb.Append(arg.Type.Name);
            }
            sb.Append(")");
            if (RetType != null)
            {
                var rt = RetType.Name;
                if (rt.StartsWith("var:"))
                    rt = rt.Substring(4);
                sb.Append(":" + rt);
            }
            return sb.ToString();
        }

        public void CheckArgs(NodeBase target, NodeBase[] args)
        {
            if (!(args.Length == Args.Length
                || (HasParams && args.Length >= Args.Length)))
                throw target.Abort("argument mismatched: {0}", target.Name);
            for (int i = 0; i < Args.Length; i++)
            {
                var t1 = args[i].Type;
                var t2 = Args[i].Type;
                if (t1 != null && t1.Cast(t2) == null)
                    throw target.Abort(
                        "can not cast arg {0}: {1}: {2} => {3}",
                        i + 1, Args[i].Name, t1.Name, t2.Name);
            }
        }

        public TypeBase RetType { get; protected set; }
        public VarDeclare[] Args { get; protected set; }
        public CallType CallType { get; protected set; }
        public bool HasParams { get; protected set; }

        public static TypeFunction New(Function f)
        {
            var ret = new TypeFunction();
            var args = new VarDeclare[f.Args.Count];
            for (int i = 0; i < args.Length; i++)
                args[i] = f.Args[i] as VarDeclare;
            ret.init(f.CallType, f.ReturnType, args);
            return ret;
        }

        protected void init(CallType callType, TypeBase retType, VarDeclare[] args)
        {
            CallType = callType;
            RetType = retType;
            var list = new ArrayList();
            if (args == null)
                HasParams = true;
            else
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    if (arg is ArgPtr)
                        HasParams = true;
                    else
                        list.Add(arg);
                }
            var ret = new VarDeclare[list.Count];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = list[i] as VarDeclare;
            this.Args = ret;
        }
    }

    public class TypeDelegate : TypeFunction
    {
        // type name
        public override string Name
        {
            get { return GetName("delegate"); }
        }

        // set value
        public override void AddSetCodes(OpModule codes, Addr32 ad)
        {
            var flag = !ad.IsAddress && ad.Register == Var.DestRegister;
            if (flag) codes.Add(I386.Push(ad.Register));

            codes.Add(I386.Push(Reg32.EAX));
            codes.Add(I386.PushA(ad));
            codes.Add(codes.GetCall("delegate", DelgFunc.Free));
            codes.Add(I386.AddR(Reg32.ESP, Val32.New(4)));
            codes.Add(codes.GetCall("delegate", DelgFunc.Duplicate));
            codes.Add(I386.AddR(Reg32.ESP, Val32.New(4)));

            if (flag) codes.Add(I386.Pop(ad.Register));
            base.AddSetCodes(codes, ad);
        }

        // type constructor
        public override bool NeedsCtor { get { return true; } }
        public override void AddConstructor(OpModule codes)
        {
            codes.Add(I386.MovRA(Reg32.EAX, Addr32.New(Reg32.ESP)));
            codes.Add(I386.MovA(Addr32.New(Reg32.EAX), Val32.New(0)));
        }

        // type destructor
        public override bool NeedsDtor { get { return true; } }
        public override void AddDestructor(OpModule codes)
        {
            codes.Add(I386.MovRA(Reg32.EAX, Addr32.New(Reg32.ESP)));
            codes.Add(I386.PushA(Addr32.New(Reg32.EAX)));
            codes.Add(codes.GetCall("delegate", DelgFunc.Free));
            codes.Add(I386.AddR(Reg32.ESP, Val32.New(4)));
        }

        public static TypeDelegate New(
            BlockBase parent, CallType callType, TypeBase retType, VarDeclare[] args)
        {
            var ret = new TypeDelegate();
            ret.init(callType, retType, args);
            ret.Parent = parent;
            return ret;
        }
    }
}
