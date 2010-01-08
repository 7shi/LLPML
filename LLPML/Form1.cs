#define LLPML

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;
using Girl.LLPML;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            OpCode.Test();
            InitializeComponent();
            ReadSample("template.xml");
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor cur = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
#if !DEBUG
            try
#endif
            {
                Parser parser = new Parser(textBox1.Text);
                string exe = GetFullName(parser.Output);
                parser.Module.Link(exe);
                textBox2.AppendText("出力: " + exe + "\r\n");
                Process.Start(exe);
            }
#if !DEBUG
            catch (Exception ex)
            {
                textBox2.AppendText(ex.ToString() + "\r\n");
            }
#endif
            Cursor.Current = cur;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ReadSample(string xml)
        {
            string path = GetFullName("Samples");
            StreamReader sr = new StreamReader(Path.Combine(path, xml));
            textBox1.Clear();
            textBox1.AppendText(sr.ReadToEnd());
            sr.Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadSample("template.xml");
        }

        private void sample1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if LLPML
            ReadSample("01.xml");
#else
            string exe = GetOutput("01.exe");
            Module module = new Module(IMAGE_SUBSYSTEM.WINDOWS_GUI);
            List<OpCode> c = new List<OpCode>();

            Function MessageBox = module.GetFunction(CallType.Std, "user32.dll", "MessageBoxW");
            Function ExitProcess = module.GetFunction(CallType.Std, "kernel32.dll", "ExitProcess");

            c.AddRange(MessageBox.Invoke(0, "こんにちは、世界！", "だいあろぐ", 0));
            c.AddRange(ExitProcess.Invoke(0));

            module.Text.OpCodes = c.ToArray();
            try { module.Link(exe); }
            catch (IOException) { }
            Process.Start(exe);
#endif
        }

        private void sample2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if LLPML
            ReadSample("02.xml");
#else
            string exe = GetFullName("02.exe");
            Module module = new Module();
            List<OpCode> c = new List<OpCode>();

            const int STD_INPUT_HANDLE = -10;
            const int STD_OUTPUT_HANDLE = -11;
            Function ExitProcess = module.GetFunction(CallType.Std, "kernel32.dll", "ExitProcess");
            Function GetStdHandle = module.GetFunction(CallType.Std, "kernel32.dll", "GetStdHandle");
            Function ReadConsole = module.GetFunction(CallType.Std, "kernel32.dll", "ReadConsoleW");
            Function WriteConsole = module.GetFunction(CallType.Std, "kernel32.dll", "WriteConsoleW");

            // HANDLE stdin = GetStdHandle(STD_INPUT_HANDLE);
            Addr32 stdin = new Addr32(module.GetInt32("stdin"));
            c.AddRange(GetStdHandle.Invoke(STD_INPUT_HANDLE));
            c.Add(I386.Mov(stdin, Reg32.EAX));

            // HANDLE stdout = GetStdHandle(STD_OUTPUT_HANDLE);
            Addr32 stdout = new Addr32(module.GetInt32("stdout"));
            c.AddRange(GetStdHandle.Invoke(STD_OUTPUT_HANDLE));
            c.Add(I386.Mov(stdout, Reg32.EAX));

            Ref<uint> dummy = module.GetInt32("dummy");
            string hello = "こんにちは、世界！\r\n";
            c.AddRange(WriteConsole.Invoke(stdout, hello, hello.Length, dummy, 0));

            string wait = "\r\nPress [Enter] to exit.\r\n";
            c.AddRange(WriteConsole.Invoke(stdout, wait, wait.Length, dummy, 0));

            Ref<uint> buffer = module.GetBuffer("buffer", 16);
            c.AddRange(ReadConsole.Invoke(stdin, buffer, 1, dummy, 0));

            c.AddRange(ExitProcess.Invoke(0));

            module.Text.OpCodes = c.ToArray();
            try { module.Link(exe); }
            catch (IOException) { }
            Process.Start(exe);
#endif
        }

        private void sample3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if LLPML
            ReadSample("03.xml");
#else
            string exe = GetFullName("03.exe");
            Module module = new Module(IMAGE_SUBSYSTEM.WINDOWS_GUI);
            List<OpCode> c = new List<OpCode>();

            Function MessageBox = module.GetFunction(CallType.Std, "user32.dll", "MessageBoxW");
            Function ExitProcess = module.GetFunction(CallType.Std, "kernel32.dll", "ExitProcess");

            c.Add(I386.Push(3));
            OpCode label = new OpCode();
            c.Add(label);
            c.AddRange(MessageBox.Invoke(0, "こんにちは、世界！", "だいあろぐ", 0));
            c.Add(I386.Dec(new Addr32(Reg32.ESP)));
            c.Add(I386.Jnz(label.Address));
            c.Add(I386.Add(Reg32.ESP, 4));
            c.AddRange(ExitProcess.Invoke(0));

            module.Text.OpCodes = c.ToArray();
            try { module.Link(exe); }
            catch (IOException) { }
            Process.Start(exe);
#endif
        }

        private string GetFullName(string exe)
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath);
            return Path.Combine(path, exe);
        }
    }
}
