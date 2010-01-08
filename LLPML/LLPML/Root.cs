using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Root : Block
    {
        public string Version = "0.3.20070814";
        public string Output = "output.exe";
        public ushort Subsystem = IMAGE_SUBSYSTEM.WINDOWS_CUI;

        public delegate TextReader StreamHandler(string name);
        public StreamHandler StreamDelegate;

        public Root() { root = this; }
        public Root(XmlTextReader xr) : this() { Read(xr); }

        protected override void ReadBlock(XmlTextReader xr)
        {
            if (xr.NodeType == XmlNodeType.Element)
            {
                switch (xr.Name)
                {
                    case "include":
                        {
                            string src = xr["src"];
                            if (src == null) throw Abort(xr, "<include> requires \"src\"");
                            TextReader tr = null;
                            if (StreamDelegate != null) tr = StreamDelegate(src);
                            if (tr == null) tr = new StreamReader(src);
                            XmlTextReader xr2 = new XmlTextReader(tr);
                            while (xr2.Read())
                            {
                                if (xr2.Name == "llpml" && xr2.NodeType == XmlNodeType.Element)
                                {
                                    Read(xr2);
                                }
                            }
                            xr2.Close();
                            tr.Close();
                            return;
                        }
                }
            }
            base.ReadBlock(xr);
        }

        public override void Read(XmlTextReader xr)
        {
            if (xr["version"] != null) Version = xr["version"];
            if (xr["output"] != null) Output = xr["output"];
            if (xr["subsystem"] == "WINDOWS_GUI") Subsystem = IMAGE_SUBSYSTEM.WINDOWS_GUI;
            base.Read(xr);
        }

        protected override void BeforeAddCodes(List<OpCode> codes, Module m)
        {
            foreach (string name in var_ints.Keys)
            {
                var_ints[name].Address = new Addr32(m.GetInt32(name));
            }
            foreach (string name in ptrs.Keys)
            {
                Pointer.Declare p = ptrs[name];
                p.Address = new Addr32(m.GetBuffer(name, p.Length));
            }
        }

        protected override void AfterAddCodes(List<OpCode> codes, Module m)
        {
            Girl.PE.Function ExitProcess = m.GetFunction(CallType.Std, "kernel32.dll", "ExitProcess");
            codes.AddRange(ExitProcess.Invoke(0));
            codes.Add(I386.Ret());
        }
    }
}
