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
    public abstract class BlockBase : BreakBase
    {
        public const string Separator = ".";

        protected ArrayList sentences = new ArrayList();
        public ArrayList Sentences { get { return sentences; } }

        protected OpCode first = new OpCode();
        public Val32 First { get { return first.Address; } }

        protected OpCode construct = new OpCode();
        public Val32 Construct { get { return construct.Address; } }

        protected OpCode destruct = new OpCode();
        public Val32 Destruct { get { return destruct.Address; } }

        protected OpCode last = new OpCode();
        public Val32 Last { get { return last.Address; } }

        public virtual bool AcceptsBreak { get { return false; } }
        public virtual bool AcceptsContinue { get { return false; } }
        public virtual Val32 Continue { get { return null; } }

        protected VarDeclare retVal;
        public bool HasRetVal { get { return retVal != null; } }
        public Var GetRetVal(BlockBase parent)
        {
            if (retVal == null)
                retVal = VarDeclare.New(this, "__retval", null);
            return Var.New(parent, retVal);
        }

        protected ListDictionary members = new ListDictionary();

        #region Member

        public virtual object GetMember(string name)
        {
            if (members.ContainsKey(name))
                return members.Get(name);
            return null;
        }

        public virtual object GetMemberRecursive(string name, Func<object, object> conv)
        {
            var ret = conv(GetMember(name));
            if (ret != null || Parent == null) return ret;
            return Parent.GetMemberRecursive(name, conv);
        }

        public VarDeclare[] GetUsingPointers()
        {
            var list = new ArrayList();
            var mems = members.Values;
            for (int i = 0; i < mems.Length; i++)
            {
                var mem = mems[i];
                if (mem is VarDeclare && !(mem is Arg))
                    list.Add(mem);
            }
            var ret = new VarDeclare[list.Count];
            for (int i = 0; i < list.Count; i++)
                ret[i] = list[i] as VarDeclare;
            return ret;
        }

        public bool AddMember(string name, object obj)
        {
            if (members.ContainsKey(name)) return false;
            members.Add(name, obj);
            return true;
        }

        #region int

        public ConstInt GetInt(string name)
        {
            var obj = GetMember(name);
            if (obj is ConstInt) return obj as ConstInt;
            if (Parent == null) return null;
            return Parent.GetInt(name);
        }

        public bool AddInt(string name, NodeBase value)
        {
            return AddMember(name, ConstInt.New(this, value));
        }

        #endregion

        #region string

        public ConstString GetString(string name)
        {
            return GetMemberRecursive(name,
                delegate(object o) { return o as ConstString; }) as ConstString;
        }

        public bool AddString(string name, string value)
        {
            return AddMember(name, ConstString.New(this, value));
        }

        #endregion

        public virtual VarDeclare GetVar(string name)
        {
            return GetMemberRecursive(name,
                delegate(object o) { return o as VarDeclare; }) as VarDeclare;
        }
        public bool AddVar(VarDeclare v) { return AddMember(v.Name, v); }

        public Function GetFunction(string name)
        {
            return GetMemberRecursive(name,
                delegate(object o) { return o as Function; }) as Function;
        }
        public bool AddFunction(Function f) { return AddMember(f.Name, f); }

        public Define GetStruct(string name)
        {
            return GetMemberRecursive(name,
                delegate(object o) { return o as Define; }) as Define;
        }
        public bool AddStruct(Define s) { return AddMember(s.Name, s); }

        #endregion

        public virtual bool HasStackFrame
        {
            get
            {
                bool ret = false;
                ForEachMembers(delegate(VarDeclare p, int pos)
                {
                    ret = true;
                    return true;
                }, null);
                return ret;
            }
        }

        public virtual int Level
        {
            get
            {
                if (Parent == null) return 0;
                if (HasStackFrame)
                    return Parent.Level + 1;
                else
                    return Parent.Level;
            }
        }

        protected void ForEachMembers(Func<VarDeclare, int, bool> delg1, Action<int> delg2)
        {
            int pos = 0;
            var mems = members.Values;
            for (int i = 0; i < mems.Length; i++)
            {
                var obj = mems[i];
                Type t = obj.GetType();
                if (t == typeof(VarDeclare) || t == typeof(Declare))
                {
                    var p = obj as VarDeclare;
                    var len = p.Type.Size;
                    /// todo: 64bit長をサポートする
                    if (len > Var.DefaultSize) len = Var.DefaultSize;
                    var pad = pos % len;
                    if (pad > 0) pos += len - pad;
                    if (delg1 != null && delg1(p, pos)) return;
                    if (!p.IsStatic) pos += p.Type.Size;
                }
            }
            var padv = pos % Var.DefaultSize;
            if (padv > 0) pos += Var.DefaultSize - padv;
            if (delg2 != null) delg2(pos);
        }

        protected virtual void BeforeAddCodes(OpModule codes)
        {
            if (HasStackFrame)
            {
                int stackSize = Level * 4;
                ForEachMembers(null, delegate(int size) { stackSize += size; });
                ForEachMembers(delegate(VarDeclare p, int pos)
                {
                    if (!p.IsStatic)
                        p.Address = Addr32.NewRO(Reg32.EBP, pos - stackSize);
                    return false;
                }, null);
                codes.Add(I386.Enter((ushort)stackSize, (byte)Level));
            }
            string n = FullName;
            if (!string.IsNullOrEmpty(n)
                && (Parent == null || Parent.FullName != n))
                codes.Add(I386.MovR(Reg32.EAX, codes.GetString(n)));
        }

        public override void AddCodes(OpModule codes)
        {
            codes.Add(first);
            BeforeAddCodes(codes);
            codes.Add(construct);
            for (int i = 0; i < sentences.Count; i++)
            {
                var child = sentences[i] as NodeBase;
#if DEBUG
                child.AddCodes(codes);
#else
                try
                {
                    child.AddCodes(codes);
                }
                catch (Exception ex)
                {
                    root.OnError(ex);
                }
#endif
            }
            if (!IsTerminated)
            {
                var list = new ArrayList();
                ForEachMembers(delegate(VarDeclare p, int pos)
                {
                    list.Add(p);
                    return false;
                }, null);
                var args = new VarDeclare[list.Count];
                for (int i = 0; i < args.Length; i++)
                    args[i] = list[i] as VarDeclare;
                AddDestructors(codes, args);
            }
            codes.Add(destruct);
            AfterAddCodes(codes);
            var mems = members.Values;
            for (int i = 0; i < mems.Length; i++)
            {
                var func = mems[i] as Function;
                if (func != null) func.AddCodes(codes);
            }
            for (int i = 0; i < mems.Length; i++)
            {
                var st = mems[i] as Define;
                if (st != null) st.AddCodes(codes);
            }
            codes.Add(last);
        }

        protected virtual void AfterAddCodes(OpModule codes)
        {
            if (IsTerminated) return;
            AddExitCodes(codes);
            var mems = members.Values;
            for (int i = 0; i < mems.Length; i++)
            {
                var mem = mems[i];
                if (mem is Function || mem is Define)
                {
                    codes.Add(I386.JmpD(last.Address));
                    return;
                }
            }
        }

        public bool IsTerminated
        {
            get
            {
                int len = sentences.Count;
                if (len == 0) return false;
                var n = sentences[len - 1] as NodeBase;
                return n is Break || n is Return;
            }
        }

        public virtual void AddDestructors(OpModule codes, VarDeclare[] ptrs)
        {
            if (ptrs == null) return;

            for (int i = ptrs.Length - 1; i >= 0; i--)
            {
                var p = ptrs[i];
                if (p.NeedsDtor)
                    p.Type.AddDestructorA(codes, p.GetAddress(codes, this));
            }
        }

        public virtual void AddExitCodes(OpModule codes)
        {
            if (HasStackFrame) codes.Add(I386.Leave());
        }

        public BlockBase GetBelongFunction()
        {
            for (BlockBase b = this; b != root; b = b.Parent)
            {
                if (b is Function) return b;
            }
            return root;
        }

        public virtual Define ThisStruct
        {
            get
            {
                if (Parent == null) return null;
                return Parent.ThisStruct;
            }
        }

        public string FullName
        {
            get
            {
                if (Parent == null) return "(root)";

                string ret = "";
                for (BlockBase b = this; b != root; b = b.Parent)
                {
                    if (!string.IsNullOrEmpty(b.Name))
                    {
                        if (ret == "")
                            ret = b.Name;
                        else
                            ret = b.Name + Separator + ret;
                    }
                }
                return ret;
            }
        }

        public string GetFullName(string name)
        {
            if (Parent == null) return name;
            return FullName + Separator + name;
        }

        private int anonymousNo = 0;

        public string GetAnonymousName()
        {
            return "__anonymous_" + (anonymousNo++);
        }

        public void AddDebug(OpModule codes, string message)
        {
            codes.Add(I386.Push(Reg32.EAX));
            codes.Add(I386.PushD(codes.GetString(message)));
            AddDebugCount(codes, "%s", 1);
            codes.Add(I386.Pop(Reg32.EAX));
        }

        public void AddDebugCount(OpModule codes, string format, int argCount)
        {
            codes.Add(I386.PushD(codes.GetString(format)));
            codes.Add(I386.CallD(GetFunction("printfln").First));
            codes.Add(I386.AddR(Reg32.ESP, Val32.NewI(((argCount + 1) * 4))));
        }

        private ArrayList typeInfos = new ArrayList();

        public void AddTypeInfo(NodeBase v)
        {
            GetRetVal(this);
            typeInfos.Add(v);
        }

        protected TypeBase returnType;
        protected bool doneInferReturnType = false;

        public virtual TypeBase ReturnType
        {
            get
            {
                if (doneInferReturnType || !root.IsCompiling)
                    return returnType;

                doneInferReturnType = true;
                for (int i = 0; i < typeInfos.Count; i++)
                {
                    var v = typeInfos[i] as NodeBase;
                    var t = returnType;
                    if (!InferType(v))
                    {
                        var err = string.Format(
                            "can not cast return type: {0} => {1}",
                            t.Name, v.Type.Name);
                        if (v is NodeBase)
                            throw (v as NodeBase).Abort(err);
                        throw Abort(err);
                    }
                }
                return returnType;
            }
        }

        private bool InferType(NodeBase v)
        {
            var vt = v.Type;
            if (vt == null) return true;

            var t = returnType;
            if (v is Null)
            {
                if (t == null || t is TypeReference || t is TypePointer)
                    return true;
                return false;
            }
            returnType = Types.Cast(t, Types.ToVarType(vt));
            return returnType != null;
        }

        protected bool doneMakeUp = false;

        public void MakeUp()
        {
            if (doneMakeUp) return;
            doneMakeUp = true;

            MakeUpInternal();

            var mems = members.Values;
            for (int i = 0; i < mems.Length; i++)
            {
                var obj = mems[i];
                if (obj is BlockBase)
                    (obj as BlockBase).MakeUp();
                else if (obj is VarDeclare)
                    (obj as VarDeclare).CheckClass();
            }
        }

        protected virtual void MakeUpInternal()
        {
        }

        public void AddSentence(NodeBase nb)
        {
            if (nb is VarDeclare)
            {
                if ((nb as VarDeclare).IsStatic)
                {
                    Parent.root.sentences.Add(nb);
                    return;
                }
            }
            sentences.Add(nb);
        }

        public void AddSentences(NodeBase[] nbs)
        {
            for (int i = 0; i < nbs.Length; i++)
                AddSentence(nbs[i]);
        }

        public class ListDictionary
        {
            private Hashtable dict = new Hashtable();
            private ArrayList list = new ArrayList();

            public void Add(string key, object value)
            {
                dict.Add(key, value);
                list.Add(value);
            }

            public int Count { get { return list.Count; } }

            public Object[] Values
            {
                get { return list.ToArray(); }
            }

            public object Get(string name)
            {
                if (dict.ContainsKey(name))
                    return dict[name];
                else
                    return null;
            }

            public bool ContainsKey(string name)
            {
                return dict.ContainsKey(name);
            }
        }
    }
}
