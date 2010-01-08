using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Block : NodeBase
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

        #region var-int

        protected Dictionary<string, VarInt.Declare> var_ints
            = new Dictionary<string, VarInt.Declare>();

        public VarInt.Declare GetVarInt(string name)
        {
            if (var_ints.ContainsKey(name)) return var_ints[name];
            return parent == null ? null : parent.GetVarInt(name);
        }

        public void AddVarInt(VarInt.Declare src)
        {
            if (var_ints.ContainsKey(src.Name))
                throw new Exception("multiple definitions: " + src.Name);
            var_ints.Add(src.Name, src);
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

        public Block() { }
        public Block(Block parent) : base(parent) { }
        public Block(Block parent, XmlTextReader xr) : base(parent, xr) { }

        protected virtual void ReadBlock(XmlTextReader xr)
        {
            switch (xr.NodeType)
            {
                case XmlNodeType.Element:
                    switch (xr.Name)
                    {
                        case "int-declare":
                            ReadIntDefine(xr);
                            break;
                        case "string-declare":
                            ReadStringDefine(xr);
                            break;
                        case "function":
                            new Function(this, xr);
                            break;
                        case "extern":
                            new Extern(this, xr);
                            break;
                        case "return":
                            sentences.Add(new Return(this, xr));
                            return;
                        case "block":
                            sentences.Add(new Block(this, xr));
                            break;
                        case "call":
                            sentences.Add(new Call(this, xr));
                            break;
                        case "if":
                            sentences.Add(new If(this, xr));
                            break;
                        case "switch":
                            sentences.Add(new Switch(this, xr));
                            break;
                        case "for":
                            sentences.Add(new For(this, xr));
                            break;
                        case "do":
                            sentences.Add(new Do(this, xr));
                            break;
                        case "while":
                            sentences.Add(new While(this, xr));
                            break;
                        case "break":
                            sentences.Add(new Break(this, xr));
                            break;
                        case "continue":
                            sentences.Add(new Continue(this, xr));
                            break;
                        case "let":
                            sentences.Add(new Let(this, xr));
                            break;
                        case "var-int-declare":
                            sentences.Add(new VarInt.Declare(this, xr));
                            break;
                        case "inc":
                            sentences.Add(new Inc(this, xr));
                            break;
                        case "dec":
                            sentences.Add(new Dec(this, xr));
                            break;
                        case "post-inc":
                            sentences.Add(new PostInc(this, xr));
                            break;
                        case "post-dec":
                            sentences.Add(new PostDec(this, xr));
                            break;
                        case "var-int-add":
                            sentences.Add(new VarInt.Add(this, xr));
                            break;
                        case "var-int-sub":
                            sentences.Add(new VarInt.Sub(this, xr));
                            break;
                        case "var-int-mul":
                            sentences.Add(new VarInt.Mul(this, xr));
                            break;
                        case "var-int-unsigned-mul":
                            sentences.Add(new VarInt.UnsignedMul(this, xr));
                            break;
                        case "var-int-div":
                            sentences.Add(new VarInt.Div(this, xr));
                            break;
                        case "var-int-unsigned-div":
                            sentences.Add(new VarInt.UnsignedDiv(this, xr));
                            break;
                        case "var-int-and":
                            sentences.Add(new VarInt.And(this, xr));
                            break;
                        case "var-int-or":
                            sentences.Add(new VarInt.Or(this, xr));
                            break;
                        case "var-int-shift-left":
                            sentences.Add(new VarInt.ShiftLeft(this, xr));
                            break;
                        case "var-int-shift-right":
                            sentences.Add(new VarInt.ShiftRight(this, xr));
                            break;
                        case "var-int-unsigned-shift-left":
                            sentences.Add(new VarInt.UnsignedShiftLeft(this, xr));
                            break;
                        case "var-int-unsigned-shift-right":
                            sentences.Add(new VarInt.UnsignedShiftRight(this, xr));
                            break;
                        case "ptr-declare":
                            sentences.Add(new Pointer.Declare(this, xr));
                            break;
                        case "struct-define":
                            new Struct.Define(this, xr);
                            break;
                        case "struct-declare":
                            sentences.Add(new Struct.Declare(this, xr));
                            break;
                        default:
                            throw Abort(xr);
                    }
                    break;

                case XmlNodeType.Whitespace:
                case XmlNodeType.Comment:
                    break;

                default:
                    throw Abort(xr, "element required");
            }
        }

        public override void Read(XmlTextReader xr)
        {
            Parse(xr, delegate
            {
                ReadBlock(xr);
            });
        }

        public virtual bool HasStackFrame
        {
            get
            {
                foreach (VarInt.Declare v in var_ints.Values)
                {
                    if (v.GetType() == typeof(VarInt.Declare))
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
            foreach (VarInt.Declare v in var_ints.Values)
            {
                if (v.GetType() == typeof(VarInt.Declare))
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

        public void AddExitCodes(List<OpCode> codes, Module m)
        {
            if (HasStackFrame) codes.Add(I386.Leave());
        }
    }
}
