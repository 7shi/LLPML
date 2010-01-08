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
        protected OpCode last = new OpCode(), next = new OpCode();
        public Val32 Last { get { return last.Address; } }

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

        public int ReadInt(XmlTextReader xr)
        {
            int? ret = null;
            string name = xr["name"];
            if (name != null)
            {
                if (!xr.IsEmptyElement)
                    throw Abort(xr, "do not specify value with name");
                ret = GetInt(name);
                if (ret == null)
                    throw Abort(xr, "undefined values: " + name);
                return (int)ret;
            }
            Parse(xr, delegate
            {
                if (xr.NodeType == XmlNodeType.Text)
                {
                    ret = (int)int.Parse(xr.Value);
                }
                else
                {
                    throw Abort(xr, "value required");
                }
            });
            if (ret == null) throw Abort(xr, "value required");
            return (int)ret;
        }

        public void ReadIntDefine(XmlTextReader xr)
        {
            string name = xr["name"];
            if (name == null) throw Abort(xr, "name required");
            int? ret = null;
            Parse(xr, delegate
            {
                if (xr.NodeType == XmlNodeType.Text)
                {
                    ret = int.Parse(xr.Value);
                }
                else
                {
                    throw Abort(xr, "value required");
                }
            });
            if (ret == null) throw Abort(xr, "value required");
            AddInt(name, (int)ret);
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

        public string ReadString(XmlTextReader xr)
        {
            string ret = null;
            string name = xr["name"];
            if (name != null)
            {
                if (!xr.IsEmptyElement)
                    throw Abort(xr, "do not specify string with name");
                ret = GetString(name);
                if (ret == null)
                    throw Abort(xr, "undefined string: " + name);
                return ret;
            }
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

        public void ReadStringDefine(XmlTextReader xr)
        {
            string name = xr["name"];
            if (name == null) throw Abort(xr, "name required");
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
            AddString(name, ret);
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
                        case "block":
                            sentences.Add(new Block(this, xr));
                            break;
                        case "call":
                            sentences.Add(new Call(this, xr));
                            break;
                        case "loop":
                            sentences.Add(new Loop(this, xr));
                            break;
                        case "for":
                            sentences.Add(new For(this, xr));
                            break;
                        case "break":
                            sentences.Add(new Break(this, xr));
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
                        case "var-int-add":
                            sentences.Add(new VarInt.Add(this, xr));
                            break;
                        case "var-int-sub":
                            sentences.Add(new VarInt.Sub(this, xr));
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

        protected virtual void BeforeAddCodes(List<OpCode> codes, Module m)
        {
            int size = Level * 4;
            foreach (VarInt.Declare v in var_ints.Values)
            {
                if (v.GetType() == typeof(VarInt.Declare))
                {
                    size += 4;
                    v.Address = new Addr32(Reg32.EBP, -size);
                }
            }
            foreach (Pointer.Declare p in ptrs.Values)
            {
                Type t = p.GetType();
                if (t == typeof(Pointer.Declare) || t == typeof(Struct.Declare))
                {
                    size += (p.Length + 3) / 4 * 4;
                    p.Address = new Addr32(Reg32.EBP, -size);
                }
            }
            codes.Add(I386.Enter((ushort)size, (byte)Level));
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            BeforeAddCodes(codes, m);
            foreach (NodeBase child in sentences)
            {
                child.AddCodes(codes, m);
            }
            codes.Add(last);
            AfterAddCodes(codes, m);
            foreach (Function func in functions.Values)
            {
                func.AddCodes(codes, m);
            }
            codes.Add(next);
        }

        protected virtual void AfterAddCodes(List<OpCode> codes, Module m)
        {
            codes.Add(I386.Leave());
            if (functions.Count > 0) codes.Add(I386.Jmp(next.Address));
        }
    }
}
