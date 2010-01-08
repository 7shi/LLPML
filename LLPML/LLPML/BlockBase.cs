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

        #region int

        protected Dictionary<string, int> ints
            = new Dictionary<string, int>();

        public int? GetInt(string name)
        {
            if (ints.ContainsKey(name)) return ints[name];
            return parent == null ? null : parent.GetInt(name);
        }

        public void AddInt(string name, int value)
        {
            if (ints.ContainsKey(name))
                throw new Exception("multiple definitions: " + name);
            ints.Add(name, value);
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
            if (name == null) throw Abort(xr, "name required");
            AddInt(name, ParseInt(xr));
        }

        #endregion

        #region string

        protected Dictionary<string, string> strings
            = new Dictionary<string, string>();

        public string GetString(string name)
        {
            if (strings.ContainsKey(name)) return strings[name];
            return parent == null ? null : parent.GetString(name);
        }

        public void AddString(string name, string value)
        {
            if (strings.ContainsKey(name))
                throw new Exception("multiple definitions: " + name);
            strings.Add(name, value);
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
            if (name == null) throw Abort(xr, "name required");
            AddString(name, ParseString(xr));
        }

        public int ReadStringLength(XmlTextReader xr)
        {
            NoChild(xr);

            string name = xr["name"];
            if (name == null) throw Abort(xr, "name required");

            return GetString(name).Length;
        }

        #endregion

        #region var

        protected Dictionary<string, Var.Declare> vars
            = new Dictionary<string, Var.Declare>();

        public Var.Declare GetVar(string name)
        {
            if (vars.ContainsKey(name)) return vars[name];
            return parent == null ? null : parent.GetVar(name);
        }

        public void AddVar(Var.Declare src)
        {
            if (vars.ContainsKey(src.Name))
                throw new Exception("multiple definitions: " + src.Name);
            vars.Add(src.Name, src);
        }

        #endregion

        #region pointer

        protected Dictionary<string, Pointer.Declare> ptrs
            = new Dictionary<string, Pointer.Declare>();

        public Pointer.Declare GetPointer(string name)
        {
            if (ptrs.ContainsKey(name)) return ptrs[name];
            return parent == null ? null : parent.GetPointer(name);
        }

        public void AddPointer(Pointer.Declare src)
        {
            if (ptrs.ContainsKey(src.Name))
                throw new Exception("multiple definitions: " + src.Name);
            ptrs.Add(src.Name, src);
        }

        public Pointer.Declare[] GetPointers()
        {
            Pointer.Declare[] ret = new Pointer.Declare[ptrs.Values.Count];
            ptrs.Values.CopyTo(ret, 0);
            return ret;
        }

        #endregion

        #region function

        protected Dictionary<string, Function> functions
            = new Dictionary<string, Function>();

        public Function GetFunction(string name)
        {
            if (functions.ContainsKey(name)) return functions[name];
            return parent == null ? null : parent.GetFunction(name);
        }

        public void AddFunction(Function f)
        {
            if (functions.ContainsKey(f.Name))
                throw new Exception("multiple definitions: " + f.Name);
            functions.Add(f.Name, f);
        }

        #endregion

        #region struct

        protected Dictionary<string, Struct.Define> structs
            = new Dictionary<string, Struct.Define>();

        public Struct.Define GetStruct(string name)
        {
            if (structs.ContainsKey(name)) return structs[name];
            return parent == null ? null : parent.GetStruct(name);
        }

        public void AddStruct(Struct.Define s)
        {
            if (structs.ContainsKey(s.Name))
                throw new Exception("multiple definitions: " + s.Name);
            structs.Add(s.Name, s);
        }

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
                foreach (Var.Declare v in vars.Values)
                {
                    if (v.GetType() == typeof(Var.Declare))
                    {
                        return true;
                    }
                }
                foreach (Pointer.Declare p in ptrs.Values)
                {
                    Type t = p.GetType();
                    if (t == typeof(Pointer.Declare) || t == typeof(Struct.Declare))
                    {
                        return true;
                    }
                }
                return false;
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

        protected virtual void BeforeAddCodes(List<OpCode> codes, Module m)
        {
            int stack = Level * 4;
            foreach (Var.Declare v in vars.Values)
            {
                if (v.GetType() == typeof(Var.Declare))
                {
                    stack += 4;
                    v.Address = new Addr32(Reg32.EBP, -stack);
                }
            }
            foreach (Pointer.Declare p in ptrs.Values)
            {
                Type t = p.GetType();
                if (t == typeof(Pointer.Declare) || t == typeof(Struct.Declare))
                {
                    stack += (p.Length + 3) / 4 * 4;
                    p.Address = new Addr32(Reg32.EBP, -stack);
                }
            }
            if (HasStackFrame)
            {
                codes.Add(I386.Enter((ushort)stack, (byte)Level));
            }
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            codes.Add(first);
            BeforeAddCodes(codes, m);
            codes.Add(construct);
            foreach (NodeBase child in sentences)
            {
                child.AddCodes(codes, m);
            }
            if (!IsTerminated) AddDestructors(codes, m, ptrs.Values);
            codes.Add(destruct);
            AfterAddCodes(codes, m);
            foreach (Function func in functions.Values)
            {
                func.AddCodes(codes, m);
            }
            codes.Add(last);
        }

        protected virtual void AfterAddCodes(List<OpCode> codes, Module m)
        {
            if (IsTerminated) return;
            AddExitCodes(codes, m);
            if (functions.Count > 0) codes.Add(I386.Jmp(last.Address));
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

        public void AddDestructors(
            List<OpCode> codes, Module m, IEnumerable<Pointer.Declare> ptrs)
        {
            if (ptrs == null) return;

            Stack<Pointer.Declare> ptrs2 = new Stack<Pointer.Declare>(ptrs);
            while (ptrs2.Count > 0)
            {
                Struct.Declare st = ptrs2.Pop() as Struct.Declare;
                if (st == null) continue;

                st.GetStruct().AddDestructor(codes, m, GetPointer(st.Name).Address);
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
    }
}
