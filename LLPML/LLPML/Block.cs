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
        public ValueWrap Last { get { return last.Address; } }

        #region int

        private Dictionary<string, int> ints = new Dictionary<string, int>();

        public int GetInt(string name)
        {
            if (ints.ContainsKey(name)) return ints[name];
            return parent == null ? 0 : parent.GetInt(name);
        }

        public int ReadInt(XmlTextReader xr)
        {
            int? ret = null;
            string name = xr["name"];
            string len = xr["len"];
            Parse(xr, delegate
            {
                if (xr.NodeType == XmlNodeType.Text)
                {
                    ret = int.Parse(xr.Value);
                }
            });
            if (name != null)
            {
                if (ret == null)
                {
                    ret = GetInt(name);
                }
                else
                {
                    ints[name] = (int)ret;
                }
            }
            if (len != null) ret = GetString(len).Length;
            return (int)ret;
        }

        #endregion

        #region string

        private Dictionary<string, string> strings = new Dictionary<string, string>();

        public string GetString(string name)
        {
            if (strings.ContainsKey(name)) return strings[name];
            return parent == null ? null : parent.GetString(name);
        }

        public string ReadString(XmlTextReader xr)
        {
            string ret = null;
            string name = xr["name"];
            Parse(xr, delegate
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Text:
                    case XmlNodeType.Whitespace:
                        ret = xr.Value;
                        break;
                    default:
                        throw Abort(xr, "text required");
                }
            });
            if (name != null)
            {
                if (ret == null)
                {
                    ret = GetString(name);
                }
                else
                {
                    strings[name] = ret;
                }
            }
            return ret;
        }

        #endregion

        #region var-int

        protected Dictionary<string, VarInt.Define> var_ints
            = new Dictionary<string, VarInt.Define>();

        public VarInt.Define GetVarInt(string name)
        {
            if (var_ints.ContainsKey(name)) return var_ints[name];
            return parent == null ? null : parent.GetVarInt(name);
        }

        public void AddVarInt(VarInt.Define src)
        {
            if (var_ints.ContainsKey(src.Name))
                throw new Exception("multiple definition: " + src.Name);
            var_ints.Add(src.Name, src);
        }

        #endregion

        #region pointer

        protected Dictionary<string, Pointer.Define> ptrs
            = new Dictionary<string, Pointer.Define>();

        public Pointer.Define GetPointer(string name)
        {
            if (ptrs.ContainsKey(name)) return ptrs[name];
            return parent == null ? null : parent.GetPointer(name);
        }

        public void AddPointer(Pointer.Define src)
        {
            if (ptrs.ContainsKey(src.Name))
                throw new Exception("multiple definition: " + src.Name);
            ptrs.Add(src.Name, src);
        }

        #endregion

        #region function

        private Dictionary<string, Function> functions
            = new Dictionary<string, Function>();

        public Function GetFunction(string name)
        {
            if (functions.ContainsKey(name)) return functions[name];
            return parent == null ? null : parent.GetFunction(name);
        }

        public void AddFunction(Function f)
        {
            if (functions.ContainsKey(f.Name))
                throw new Exception("multiple definition: " + f.Name);
            functions.Add(f.Name, f);
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
                        case "int":
                            ReadInt(xr);
                            break;
                        case "string":
                            ReadString(xr);
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
                        case "var-int-define":
                            sentences.Add(new VarInt.Define(this, xr));
                            break;
                        case "var-int-let":
                            sentences.Add(new VarInt.Let(this, xr));
                            break;
                        case "var-int-inc":
                            sentences.Add(new VarInt.Inc(this, xr));
                            break;
                        case "var-int-dec":
                            sentences.Add(new VarInt.Dec(this, xr));
                            break;
                        case "var-int-add":
                            sentences.Add(new VarInt.Add(this, xr));
                            break;
                        case "var-int-sub":
                            sentences.Add(new VarInt.Sub(this, xr));
                            break;
                        case "ptr-define":
                            sentences.Add(new Pointer.Define(this, xr));
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
            foreach (VarInt.Define v in var_ints.Values)
            {
                if (v.GetType() == typeof(VarInt.Define))
                {
                    size += 4;
                    v.Address = new Addr32(Reg32.EBP, -size);
                }
            }
            foreach (Pointer.Define p in ptrs.Values)
            {
                if (p.GetType() == typeof(Pointer.Define))
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
