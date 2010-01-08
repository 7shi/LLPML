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
        public const string LLPMLVersion = "0.99.20080914";
        public string Version = LLPMLVersion;
        public string Output = "output.exe";
        public ushort Subsystem = IMAGE_SUBSYSTEM.WINDOWS_CUI;

        public Func<string, TextReader> StreamDelegate;

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
            SrcInfo = new Parsing.SrcInfo(name, xr);
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
                            var src = xr["src"];
                            if (src == null) throw Abort(xr, "<include> requires \"src\"");
                            var srcl = src.ToLower();
                            if (!included.Contains(srcl))
                            {
                                included.Add(srcl);
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
                    case "title":
                        Parse(xr, null);
                        return;
                }
            }
            base.ReadBlock(xr);
        }

        public void Read(string name, XmlTextReader xr)
        {
            included.Add(name.ToLower());
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

        protected override void BeforeAddCodes(OpModule codes)
        {
            ForEachMembers((p, pos) =>
            {
                p.Address = new Addr32(codes.Module.GetBuffer(p.Name, p.Type.Size));
                return false;
            }, null);
        }

        protected override void AfterAddCodes(OpModule codes)
        {
            AddExitCodes(codes);
            codes.Add(I386.Ret());
        }

        public override void AddExitCodes(OpModule codes)
        {
            if (retVal != null)
                GetRetVal(this).AddCodes(codes, "push", null);
            else
                codes.Add(I386.Push((Val32)0));
            codes.Add(I386.Call(codes.Module.GetFunction(
                CallType.Std, "kernel32.dll", "ExitProcess").Address));
        }

        public static string ReadTitle(XmlTextReader xr)
        {
            while (xr.Read())
            {
                if (xr.Name == "title")
                {
                    if (xr.Read() && xr.NodeType == XmlNodeType.Text)
                        return xr.Value;
                    return "";
                }
            }
            return "";
        }

        public bool IsCompiling { get; protected set; }

        public override void AddCodes(OpModule codes)
        {
            IsCompiling = true;
            OpModule.Root = this;
            MakeUpStatics(codes.Module);
            MakeUp();
            base.AddCodes(codes);
            OpModule.Root = null;
            IsCompiling = false;
        }

        private void MakeUpStatics(Module m)
        {
            foreach (var s in sentences)
            {
                var vd = s as Var.Declare;
                if (vd != null && vd.IsStatic)
                    vd.Address = new Addr32(m.GetBuffer(vd.FullName, vd.Type.Size));
            }
        }
    }
}
