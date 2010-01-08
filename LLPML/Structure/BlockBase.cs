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

        protected Dictionary<string, object> members
            = new Dictionary<string, object>();

        public const string Separator = ".";

        #region Member

        public virtual T GetMember<T>(string name) where T : class
        {
            object obj;
            if (members.TryGetValue(name, out obj)) return obj as T;
            return null;
        }

        public T GetMemberRecursive<T>(string name) where T : class
        {
            T ret = GetMember<T>(name);
            if (ret != null) return ret;
            return parent == null ? null : parent.GetMemberRecursive<T>(name);
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

        public int? GetInt(string name)
        {
            object obj = GetMember<object>(name);
            if (obj is int) return (int?)(int)obj;
            return parent == null ? null : parent.GetInt(name);
        }

        public bool AddInt(string name, int value)
        {
            return AddMember(name, value);
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

        public int ReadInt(XmlTextReader xr)
        {
            string name = xr["name"];
            if (name != null)
            {
                if (!xr.IsEmptyElement)
                    throw Abort(xr, "do not specify value with name");
                int? ret = GetInt(name);
                if (ret == null)
                    throw Abort(xr, "undefined values: " + name);
                return (int)ret;
            }
            return ParseInt(xr);
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

        public string GetString(string name) { return GetMemberRecursive<string>(name); }
        public bool AddString(string name, string value) { return AddMember(name, value); }

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
                string ret = GetString(name);
                if (ret == null)
                    throw Abort(xr, "undefined string: " + name);
                return ret;
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

            return GetString(name).Length;
        }

        #endregion

        public Var.Declare GetVar(string name) { return GetMemberRecursive<Var.Declare>(name); }
        public bool AddVar(Var.Declare v) { return AddMember(v.Name, v); }

        public Pointer.Declare GetPointer(string name) { return GetMemberRecursive<Pointer.Declare>(name); }
        public bool AddPointer(Pointer.Declare p) { return AddMember(p.Name, p); }

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

        public int Level
        {
            get
            {
                if (parent == null) return 0;
                if (HasStackFrame)
                    return parent.Level + 1;
                else
                    return parent.Level;
            }
        }

        protected void ForEachMembers(Func<Pointer.Declare, int, bool> delg1, Action<int> delg2)
        {
            int pos = 0;
            foreach (var obj in members.Values)
            {
                Type t = obj.GetType();
                if (t == typeof(Var.Declare)
                    || t == typeof(Pointer.Declare)
                    || t == typeof(Struct.Declare))
                {
                    var p = obj as Pointer.Declare;
                    var len = p.TypeSize;
                    if (len == 0) len = p.Length;
                    if (len > Var.DefaultSize) len = Var.DefaultSize;
                    var pad = pos % len;
                    if (pad > 0) pos += len - pad;
                    if (delg1 != null && delg1(p, pos)) return;
                    pos += p.Length;
                }
            }
            var padv = pos % Var.DefaultSize;
            if (padv > 0) pos += Var.DefaultSize - padv;
            if (delg2 != null) delg2(pos);
        }

        protected virtual void BeforeAddCodes(List<OpCode> codes, Module m)
        {
            if (HasStackFrame)
            {
                int stackSize = Level * 4;
                ForEachMembers(null, size => stackSize += size);
                ForEachMembers((p, pos) =>
                {
                    p.Address = new Addr32(Reg32.EBP, pos - stackSize);
                    return false;
                }, null);
                codes.Add(I386.Enter((ushort)stackSize, (byte)Level));
            }
            string n = GetName();
            if (!string.IsNullOrEmpty(n))
                codes.Add(I386.Mov(Reg32.EAX, m.GetString(n)));
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            CheckStructs();
            codes.Add(first);
            BeforeAddCodes(codes, m);
            codes.Add(construct);
            foreach (NodeBase child in sentences)
            {
                child.AddCodes(codes, m);
            }
            if (!IsTerminated)
                AddDestructors(codes, m, GetMembers<Pointer.Declare>());
            codes.Add(destruct);
            AfterAddCodes(codes, m);
            foreach (Function func in GetMembers<Function>())
            {
                func.AddCodes(codes, m);
            }
            foreach (Struct.Define st in GetMembers<Struct.Define>())
            {
                st.AddCodes(codes, m);
            }
            codes.Add(last);
        }

        private void CheckStructs()
        {
            foreach (Struct.Define st in GetMembers<Struct.Define>())
            {
                st.CheckStruct();
            }
        }

        protected virtual void AfterAddCodes(List<OpCode> codes, Module m)
        {
            if (IsTerminated) return;
            AddExitCodes(codes, m);
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
            List<OpCode> codes, Module m, IEnumerable<Pointer.Declare> ptrs)
        {
            if (ptrs == null) return;

            Stack<Pointer.Declare> ptrs2 = new Stack<Pointer.Declare>(ptrs);
            while (ptrs2.Count > 0)
            {
                Struct.Declare st = ptrs2.Pop() as Struct.Declare;
                if (st == null) continue;

                st.GetStruct().AddDestructor(codes, m,
                    GetPointer(st.Name).GetAddress(codes, m, this));
            }
        }

        public virtual void AddExitCodes(List<OpCode> codes, Module m)
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
                if (parent == null) return null;
                return parent.ThisStruct;
            }
        }

        public string GetName()
        {
            if (parent == null) return "(root)";

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
}
