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
        private List<NodeBase> sentences = new List<NodeBase>();

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
                if (xr.NodeType == XmlNodeType.Text)
                {
                    ret = xr.Value;
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

        public class LocalVarInt
        {
            public Block scope;
            public Addr32 addr;

            public LocalVarInt(Block scope) { this.scope = scope; }
        }

        protected Dictionary<string, LocalVarInt> var_ints = new Dictionary<string, LocalVarInt>();

        public LocalVarInt GetVarInt(string name)
        {
            if (var_ints.ContainsKey(name)) return var_ints[name];
            return parent == null ? null : parent.GetVarInt(name);
        }

        public void AddVarInt(string name)
        {
            if (var_ints.ContainsKey(name)) return;
            var_ints.Add(name, new LocalVarInt(this));
        }

        public LocalVarInt ReadVarInt(XmlTextReader xr)
        {
            return GetVarInt(new VarInt(this, xr).Name);
        }

        #endregion

        #region pointer

        public class LocalPointer
        {
            public Block scope;
            public Ptr<uint> ptr;
            public Addr32 addr;
            public int len;

            public LocalPointer(Block scope, int len)
            {
                this.scope = scope;
                this.len = len;
            }
        };

        protected Dictionary<string, LocalPointer> ptrs = new Dictionary<string, LocalPointer>();

        public LocalPointer GetPointer(string name)
        {
            if (ptrs.ContainsKey(name)) return ptrs[name];
            return parent == null ? null : parent.GetPointer(name);
        }

        public void AddPointer(string name, int len)
        {
            if (ptrs.ContainsKey(name)) return;
            ptrs.Add(name, new LocalPointer(this, len));
        }

        public LocalPointer ReadPointer(XmlTextReader xr)
        {
            return GetPointer(new Pointer(this, xr).Name);
        }

        #endregion

        public Block() { }
        public Block(Block parent, XmlTextReader xr) : base(parent, xr) { }

        protected void ReadBlock(XmlTextReader xr)
        {
            if (xr.NodeType == XmlNodeType.Element)
            {
                switch (xr.Name)
                {
                    case "block":
                        sentences.Add(new Block(this, xr));
                        break;
                    case "extern":
                        new Extern(this, xr);
                        break;
                    case "call":
                        sentences.Add(new Call(this, xr));
                        break;
                    case "int":
                        ReadInt(xr);
                        break;
                    case "string":
                        ReadString(xr);
                        break;
                    case "var-int":
                        sentences.Add(new VarInt(this, xr));
                        break;
                    case "ptr":
                        sentences.Add(new Pointer(this, xr));
                        break;
                    case "loop":
                        sentences.Add(new Loop(this, xr));
                        break;
                    default:
                        throw Abort(xr);
                }
            }
            else if (xr.NodeType != XmlNodeType.Whitespace)
            {
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
            foreach (string name in var_ints.Keys)
            {
                size += 4;
                var_ints[name].addr = new Addr32(Reg32.EBP, -size);
            }
            foreach (string name in ptrs.Keys)
            {
                LocalPointer b = ptrs[name];
                size += (b.len + 3) / 4 * 4;
                b.addr = new Addr32(Reg32.EBP, -size);
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
            AfterAddCodes(codes, m);
        }

        protected virtual void AfterAddCodes(List<OpCode> codes, Module m)
        {
            codes.Add(I386.Leave());
        }
    }
}
