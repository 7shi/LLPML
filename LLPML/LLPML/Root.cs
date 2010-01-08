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
        public string Version = "0.2.20070805";
        public string Output = "output.exe";
        public ushort Subsystem = IMAGE_SUBSYSTEM.WINDOWS_CUI;

        public Root() { root = this; }
        public Root(XmlTextReader xr) : this() { Read(xr); }

        public override void Read(XmlTextReader xr)
        {
            if (xr["version"] != null) Version = xr["version"];
            if (xr["output"] != null) Output = xr["output"];
            if (xr["subsystem"] == "WINDOWS_GUI") Subsystem = IMAGE_SUBSYSTEM.WINDOWS_GUI;
            base.Read(xr);
        }

        private Dictionary<string, Extern> externs = new Dictionary<string, Extern>();
        public Extern GetFunction(string name)
        {
            if (externs.ContainsKey(name)) return externs[name];
            Extern ret = new Extern();
            externs.Add(name, ret);
            return ret;
        }
        public void SetFunction(string name, Extern function)
        {
            if (externs.ContainsKey(name))
            {
                externs[name].Set(function);
            }
            else
            {
                externs.Add(name, function);
            }
        }

        protected override void BeforeAddCodes(List<OpCode> codes, Module m)
        {
            foreach (string name in var_ints.Keys)
            {
                var_ints[name].addr = new Addr32(m.GetInt32(name));
            }
            foreach (string name in ptrs.Keys)
            {
                LocalPointer b = ptrs[name];
                b.ptr = m.GetBuffer(name, b.len);
            }
        }

        protected override void AfterAddCodes(List<OpCode> codes, Module m)
        {
            Function ExitProcess = m.GetFunction(CallType.Std, "kernel32.dll", "ExitProcess");
            codes.AddRange(ExitProcess.Invoke(0));
        }
    }
}
