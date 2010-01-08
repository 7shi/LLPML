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
        public const string VERSION = "1.2.2008.1013";
        public string Version = VERSION;
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
            if (xr["subsystem"] != null) SetSubsystem(xr["subsystem"]);
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
            switch (Subsystem)
            {
                case IMAGE_SUBSYSTEM.WINDOWS_CUI:
                case IMAGE_SUBSYSTEM.WINDOWS_GUI:
                    if (retVal != null)
                        GetRetVal(this).AddCodes(codes, "push", null);
                    else
                        codes.Add(I386.Push((Val32)0));
                    codes.Add(I386.Call(codes.Module.GetFunction(
                        CallType.Std, "kernel32.dll", "ExitProcess").Address));
                    break;
                default:
                    if (retVal != null)
                        GetRetVal(this).AddCodes(codes, "mov", null);
                    else
                        codes.Add(I386.Xor(Reg32.EAX, Reg32.EAX));
                    break;
            }
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
            switch (Subsystem)
            {
                case IMAGE_SUBSYSTEM.WINCE_GUI:
                    codes.Module.Specific.ImageBase = 0x10000;
                    codes.Module.Specific.SectionAlignment = 0x1000;
                    break;
            }
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

        public bool SetSubsystem(string subsys)
        {
            switch (subsys)
            {
                case "WINDOWS_CUI":
                    Subsystem = IMAGE_SUBSYSTEM.WINDOWS_CUI;
                    return true;
                case "WINDOWS_GUI":
                    Subsystem = IMAGE_SUBSYSTEM.WINDOWS_GUI;
                    return true;
                case "WINCE_GUI":
                    Subsystem = IMAGE_SUBSYSTEM.WINCE_GUI;
                    return true;
            }
            return false;
        }
    }
}
