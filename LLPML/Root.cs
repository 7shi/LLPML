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
        public string Version = "0.9.20080127";
        public string Output = "output.exe";
        public ushort Subsystem = IMAGE_SUBSYSTEM.WINDOWS_CUI;

        public delegate TextReader StreamHandler(string name);
        public StreamHandler StreamDelegate;

        private List<string> included = new List<string>();

        private Stack<string> sources = new Stack<string>();
        public string Source
        {
            get
            {
                if (sources.Count == 0) return null;
                return sources.Peek();
            }
        }

        public Root()
        {
            root = this;
        }

        public Root(string name, XmlTextReader xr)
        {
            SetLine(xr);
            Read(name, xr);
        }

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
                            if (!included.Contains(src))
                            {
                                included.Add(src);
                                sources.Push(src);
                                TextReader tr = null;
                                if (StreamDelegate != null) tr = StreamDelegate(src);
                                if (tr == null) tr = new StreamReader(src);
                                XmlTextReader xr2 = new XmlTextReader(tr);
                                Read(src, xr2);
                                xr2.Close();
                                tr.Close();
                                sources.Pop();
                            }
                            return;
                        }
                }
            }
            base.ReadBlock(xr);
        }

        public void Read(string name, XmlTextReader xr)
        {
            sources.Push(name);
            while (xr.Read())
            {
                if (xr.Name == "llpml" && xr.NodeType == XmlNodeType.Element)
                {
                    Read(xr);
                }
            }
            sources.Pop();
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
            foreach (object obj in members.Values)
            {
                if (obj is Var.Declare)
                {
                    Var.Declare v = obj as Var.Declare;
                    v.Address = new Addr32(m.GetInt32(v.Name));
                }
                else if (obj is Pointer.Declare)
                {
                    Pointer.Declare p = obj as Pointer.Declare;
                    p.Address = new Addr32(m.GetBuffer(p.Name, p.Length));
                }
            }
        }

        protected override void AfterAddCodes(List<OpCode> codes, Module m)
        {
            AddExitCodes(codes, m);
            codes.Add(I386.Ret());
        }

        public override void AddExitCodes(List<OpCode> codes, Module m)
        {
            if (members.ContainsKey("__retval"))
            {
                IIntValue retval = new Var(this, "__retval") as IIntValue;
                retval.AddCodes(codes, m, "push", null);
            }
            else
                codes.Add(I386.Push((Val32)0));
            codes.Add(I386.Call(m.GetFunction(
                CallType.Std, "kernel32.dll", "ExitProcess").Address));
        }
    }
}
