using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class BlockBase : BreakBase
    {
        public const string Separator = ".";

        protected List<NodeBase> sentences = new List<NodeBase>();
        public List<NodeBase> Sentences { get { return sentences; } }

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

        protected Var.Declare retVal;
        public bool HasRetVal { get { return retVal != null; } }
        public Var GetRetVal(BlockBase parent)
        {
            if (retVal == null)
                retVal = new Var.Declare(this, "__retval");
            return new Var(parent, retVal);
        }

        protected ListDictionary members = new ListDictionary();

        #region Member

        public virtual T GetMember<T>(string name) where T : class
        {
            object obj;
            if (members.TryGetValue(name, out obj)) return obj as T;
            return null;
        }

        public virtual T GetMemberRecursive<T>(string name) where T : class
        {
            T ret = GetMember<T>(name);
            if (ret != null || Parent == null) return ret;
            return Parent.GetMemberRecursive<T>(name);
        }

        public T[] GetMembers<T>() where T : class
        {
            var list = new List<T>();
            foreach (var obj in members.Values)
            {
                if (obj is T) list.Add(obj as T);
            }
            return list.ToArray();
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
            object obj = GetMember<object>(name);
            if (obj is ConstInt) return obj as ConstInt;
            return Parent == null ? null : Parent.GetInt(name);
        }

        public bool AddInt(string name, IIntValue value)
        {
            return AddMember(name, new ConstInt(this, value));
        }

        public bool AddInt(string name, int value)
        {
            return AddInt(name, new IntValue(value));
        }

        public int ParseInt(XmlTextReader xr)
        {
            string value = null;
            Parse(xr, delegate
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Text:
                        value = xr.Value;
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    default:
                        throw Abort(xr, "value required");
                }
            });
            if (value == null) throw Abort(xr, "value required");
            return IntValue.Parse(value);
        }

        public IIntValue ReadInt(XmlTextReader xr)
        {
            string name = xr["name"];
            if (name != null)
            {
                if (!xr.IsEmptyElement)
                    throw Abort(xr, "do not specify value with name");
                var ret = GetInt(name);
                if (ret == null)
                    throw Abort(xr, "undefined values: " + name);
                return ret;
            }
            return new IntValue(ParseInt(xr));
        }

        public void ReadIntDefine(XmlTextReader xr)
        {
            string name = xr["name"];
            if (name == null)
                throw Abort(xr, "name required");
            if (!AddInt(name, ParseInt(xr)))
                throw Abort(xr, "multiple definitions: " + name);
        }

        #endregion

        #region string

        public ConstString GetString(string name)
        {
            return GetMemberRecursive<ConstString>(name);
        }

        public bool AddString(string name, string value)
        {
            return AddMember(name, new ConstString(this, value));
        }

        private string ParseString(XmlTextReader xr)
        {
            string ret = null;
            Parse(xr, delegate
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Text:
                    case XmlNodeType.Whitespace:
                        ret = xr.Value;
                        break;
                    default:
                        throw Abort(xr, "string required");
                }
            });
            if (ret == null) throw Abort(xr, "string required");
            return ret;
        }

        public string ReadString(XmlTextReader xr)
        {
            string name = xr["name"];
            if (name != null)
            {
                if (!xr.IsEmptyElement)
                    throw Abort(xr, "do not specify string with name");
                var ret = GetString(name);
                if (ret == null)
                    throw Abort(xr, "undefined string: " + name);
                return ret.Value;
            }
            return ParseString(xr);
        }

        public void ReadStringDefine(XmlTextReader xr)
        {
            string name = xr["name"];
            if (name == null)
                throw Abort(xr, "name required");
            if (!AddString(name, ParseString(xr)))
                throw Abort(xr, "multiple definitions: " + name);
        }

        public int ReadStringLength(XmlTextReader xr)
        {
            NoChild(xr);

            string name = xr["name"];
            if (name == null) throw Abort(xr, "name required");

            return GetString(name).Value.Length;
        }

        #endregion

        public virtual Var.Declare GetVar(string name) { return GetMemberRecursive<Var.Declare>(name); }
        public bool AddVar(Var.Declare v) { return AddMember(v.Name, v); }

        public Function GetFunction(string name) { return GetMemberRecursive<Function>(name); }
        public bool AddFunction(Function f) { return AddMember(f.Name, f); }

        public Struct.Define GetStruct(string name) { return GetMemberRecursive<Struct.Define>(name); }
        public bool AddStruct(Struct.Define s) { return AddMember(s.Name, s); }

        #endregion

        public BlockBase() { }
        public BlockBase(BlockBase parent) : base(parent) { }
        public BlockBase(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            base.Read(xr);
        }

        public virtual bool HasStackFrame
        {
            get
            {
                bool ret = false;
                ForEachMembers((p, pos) =>
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

        protected void ForEachMembers(Func<Var.Declare, int, bool> delg1, Action<int> delg2)
        {
            int pos = 0;
            foreach (var obj in members.Values)
            {
                Type t = obj.GetType();
                if (t == typeof(Var.Declare) || t == typeof(Struct.Declare))
                {
                    var p = obj as Var.Declare;
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
                ForEachMembers(null, size => stackSize += size);
                ForEachMembers((p, pos) =>
                {
                    if (!p.IsStatic)
                        p.Address = new Addr32(Reg32.EBP, pos - stackSize);
                    return false;
                }, null);
                codes.Add(I386.Enter((ushort)stackSize, (byte)Level));
            }
            string n = FullName;
            if (!string.IsNullOrEmpty(n)
                && (Parent == null || Parent.FullName != n))
                codes.Add(I386.Mov(Reg32.EAX, codes.GetString(n)));
        }

        public override void AddCodes(OpModule codes)
        {
            codes.Add(first);
            BeforeAddCodes(codes);
            codes.Add(construct);
            foreach (NodeBase child in sentences)
            {
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
                var mems = new List<Var.Declare>();
                ForEachMembers((p, pos) =>
                {
                    mems.Add(p);
                    return false;
                }, null);
                AddDestructors(codes, mems);
            }
            codes.Add(destruct);
            AfterAddCodes(codes);
            foreach (Function func in GetMembers<Function>())
                func.AddCodes(codes);
            foreach (Struct.Define st in GetMembers<Struct.Define>())
                st.AddCodes(codes);
            codes.Add(last);
        }

        protected virtual void AfterAddCodes(OpModule codes)
        {
            if (IsTerminated) return;
            AddExitCodes(codes);
            if (GetMembers<Function>().Length > 0
                || GetMembers<Struct.Define>().Length > 0)
            {
                codes.Add(I386.Jmp(last.Address));
            }
        }

        public bool IsTerminated
        {
            get
            {
                int len = sentences.Count;
                if (len == 0) return false;
                NodeBase n = sentences[len - 1];
                return n is Break || n is Return;
            }
        }

        public virtual void AddDestructors(
            OpModule codes, IEnumerable<Var.Declare> ptrs)
        {
            if (ptrs == null) return;

            Stack<Var.Declare> ptrs2 = new Stack<Var.Declare>(ptrs);
            while (ptrs2.Count > 0)
            {
                var p = ptrs2.Pop();
                if (p.NeedsDtor)
                    p.Type.AddDestructor(codes, p.GetAddress(codes, this));
            }
        }

        public virtual void AddExitCodes(OpModule codes)
        {
            if (HasStackFrame) codes.Add(I386.Leave());
        }

        public BlockBase GetFunction()
        {
            for (BlockBase b = this; b != root; b = b.Parent)
            {
                if (b is Function) return b;
            }
            return root;
        }

        public virtual Struct.Define ThisStruct
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
            codes.Add(I386.Push(codes.GetString(message)));
            AddDebug(codes, "%s", 1);
            codes.Add(I386.Pop(Reg32.EAX));
        }

        public void AddDebug(OpModule codes, string format, int argCount)
        {
            codes.AddRange(new[]
            {
                I386.Push(codes.GetString(format)),
                I386.Call(GetFunction("printfln").First),
                I386.Add(Reg32.ESP, (uint)((argCount + 1) * 4))
            });
        }

        private List<IIntValue> typeInfos = new List<IIntValue>();

        public void AddTypeInfo(IIntValue v)
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
                foreach (var v in typeInfos)
                {
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

        private bool InferType(IIntValue v)
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

            foreach (var obj in members.Values)
                if (obj is BlockBase)
                    (obj as BlockBase).MakeUp();
                else if (obj is Var.Declare)
                    (obj as Var.Declare).CheckClass();
        }

        protected virtual void MakeUpInternal()
        {
        }

        public void AddSentence(NodeBase nb)
        {
            if (nb is Var.Declare)
            {
                if ((nb as Var.Declare).IsStatic)
                {
                    Parent.root.sentences.Add(nb);
                    return;
                }
            }
            sentences.Add(nb);
        }

        public void AddSentences(IEnumerable<NodeBase> nbs)
        {
            foreach (var nb in nbs) AddSentence(nb);
        }

        public class ListDictionary
        {
            private Dictionary<string, object> dict = new Dictionary<string, object>();
            private List<object> list = new List<object>();

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

            public bool TryGetValue(string name, out object obj)
            {
                return dict.TryGetValue(name, out obj);
            }

            public bool ContainsKey(string name)
            {
                return dict.ContainsKey(name);
            }
        }
    }
}
