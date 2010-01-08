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
using Girl.LLPML;
using Girl.PE;
using Girl.X86;

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
                StringReader sr = new StringReader(textBox1.Text);
                XmlTextReader xr = new XmlTextReader(sr);
                Root root = null;
                while (xr.Read())
                {
                    if (xr.Name == "llpml" && xr.NodeType == XmlNodeType.Element)
                    {
                        root = new Root(xr);
                    }
                }
                xr.Close();
                sr.Close();

                Module module = new Module();
                module.Specific.SubSystem = root.Subsystem;

                List<OpCode> codes = new List<OpCode>();
                root.AddCodes(codes, module);
                module.Text.OpCodes = codes.ToArray();

                string exe = GetFullName(root.Output);
                module.Link(exe);
                textBox2.AppendText("èoóÕ: " + exe + "\r\n");
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
            ReadSample("01.xml");
        }

        private void sample2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadSample("02.xml");
        }

        private void sample3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadSample("03.xml");
        }

        private string GetFullName(string exe)
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath);
            return Path.Combine(path, exe);
        }
    }
}
