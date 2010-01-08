using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Root : Block
    {
        public string Version = "0.1.20070729";
        public string Output = "output.exe";
        public ushort Subsystem = IMAGE_SUBSYSTEM.WINDOWS_CUI;

        public Root() { }
        public Root(XmlTextReader xr) { Read(this, xr); }

        public override void Read(Root root, XmlTextReader xr)
        {
            if (xr["version"] != null) Version = xr["version"];
            if (xr["output"] != null) Output = xr["output"];
            if (xr["subsystem"] == "WINDOWS_GUI") Subsystem = IMAGE_SUBSYSTEM.WINDOWS_GUI;
            base.Read(root, xr);
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            var_ints.Clear();
            ptrs.Clear();
            base.AddCodes(codes, m);
            Function ExitProcess = m.GetFunction(CallType.Std, "kernel32.dll", "ExitProcess");
            codes.AddRange(ExitProcess.Invoke(0));
        }

        private Dictionary<string, Extern> functions = new Dictionary<string, Extern>();
        public Extern GetFunction(string name)
        {
            if (functions.ContainsKey(name)) return functions[name];
            Extern ret = new Extern();
            functions.Add(name, ret);
            return ret;
        }
        public void SetFunction(string name, Extern function)
        {
            if (functions.ContainsKey(name))
            {
                functions[name].Set(function);
            }
            else
            {
                functions.Add(name, function);
            }
        }

        private Dictionary<string, int> ints = new Dictionary<string, int>();
        public Dictionary<string, int> Ints { get { return ints; } }

        private Dictionary<string, string> strs = new Dictionary<string, string>();
        public Dictionary<string, string> Strings { get { return strs; } }

        private Dictionary<string, Addr32> var_ints = new Dictionary<string, Addr32>();
        public Addr32 GetVarInt(string name)
        {
            if (var_ints.ContainsKey(name)) return var_ints[name];
            Addr32 ret = new Addr32();
            var_ints.Add(name, ret);
            return ret;
        }
        public void SetVarInt(string name, Addr32 v)
        {
            if (var_ints.ContainsKey(name))
            {
                var_ints[name].Set(v);
            }
            else
            {
                var_ints.Add(name, v);
            }
        }

        private Dictionary<string, Ref<uint>> ptrs = new Dictionary<string, Ref<uint>>();
        public Ref<uint> GetPointer(string name)
        {
            if (ptrs.ContainsKey(name)) return ptrs[name];
            Ref<uint> ret = new Ref<uint>();
            ptrs.Add(name, ret);
            return ret;
        }
        public void SetPointer(string name, Ref<uint> v)
        {
            if (ptrs.ContainsKey(name))
            {
                ptrs[name].Set(v);
            }
            else
            {
                ptrs.Add(name, v);
            }
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
                    ret = ints[name];
                }
                else
                {
                    ints[name] = (int)ret;
                }
            }
            if (len != null) ret = strs[len].Length;
            return (int)ret;
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
                    ret = strs[name];
                }
                else
                {
                    strs[name] = ret;
                }
            }
            return ret;
        }

        public Addr32 ReadVarInt(XmlTextReader xr)
        {
            VarInt v = new VarInt(this, xr);
            return GetVarInt(v.Name);
        }

        public Ref<uint> ReadPointer(XmlTextReader xr)
        {
            Pointer p = new Pointer(this, xr);
            return GetPointer(p.Name);
        }
    }
}
