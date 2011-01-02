using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        public const string VERSION = "1.8.2011.0102";
        public string Version = VERSION;
        public string Output = "output.exe";
        public ushort Subsystem = IMAGE_SUBSYSTEM.WINDOWS_CUI;

        public Func<string, TextReader> StreamDelegate;

        private StringCollection included = new StringCollection();

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

        protected override void BeforeAddCodes(OpModule codes)
        {
            ForEachMembers((p, pos) =>
            {
                p.Address = Addr32.NewD(codes.Module.GetBuffer(p.Name, p.Type.Size));
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
                        GetRetVal(this).AddCodesV(codes, "push", null);
                    else
                        codes.Add(I386.PushD(Val32.New(0)));
                    codes.Add(I386.CallA(codes.Module.GetFunction(
                        "kernel32.dll", "ExitProcess")));
                    break;
                default:
                    if (retVal != null)
                        GetRetVal(this).AddCodesV(codes, "mov", null);
                    else
                        codes.Add(I386.Xor(Reg32.EAX, Reg32.EAX));
                    break;
            }
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
                var vd = s as VarDeclare;
                if (vd != null && vd.IsStatic)
                    vd.Address = Addr32.NewD(m.GetBuffer(vd.FullName, vd.Type.Size));
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

        public event Action<Exception> Error;
        public void OnError(Exception ex)
        {
            if (Error != null) Error(ex);
        }
    }
}
