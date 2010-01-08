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
#if DEBUG
            I386.Test();
#endif
            InitializeComponent();
            ReadSample(textBox1, "template.xml");
            for (int i = 1; ; i++)
            {
                string xml = string.Format("{0:00}.xml", i);
                if (!File.Exists(GetSampleFileName(xml))) break;
                AddTab(xml);
            }
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor cur = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
#if !DEBUG
            try
#endif
            {
                TextBox tb = tabControl1.SelectedTab.Controls[0] as TextBox;
                StringReader sr = new StringReader(tb.Text);
                XmlTextReader xr = new XmlTextReader(sr);
                Root root = new Root();
                if (tb.Tag is string)
                {
                    root.Output = Path.GetFileNameWithoutExtension(tb.Tag as string) + ".exe";
                }
                while (xr.Read())
                {
                    if (xr.Name == "llpml" && xr.NodeType == XmlNodeType.Element)
                    {
                        root.Read(xr);
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

        private string GetSampleFileName(string xml)
        {
            return Path.Combine(GetFullName("Samples"), xml);
        }

        private void ReadSample(TextBox tb, string xml)
        {
            StreamReader sr = new StreamReader(GetSampleFileName(xml));
            tb.Clear();
            tb.AppendText(sr.ReadToEnd());
            tb.SelectionStart = 0;
            sr.Close();
        }

        private TabPage AddTab(string xml, string title, string output)
        {
            TabPage page = new TabPage(title);
            TextBox tb = new TextBox();
            tb.Multiline = true;
            tb.WordWrap = false;
            tb.ScrollBars = ScrollBars.Both;
            tb.Dock = DockStyle.Fill;
            tb.Tag = output;
            ReadSample(tb, xml);
            page.Controls.Add(tb);
            tabControl1.TabPages.Add(page);
            return page;
        }

        private TabPage AddTab(string xml)
        {
            return AddTab(xml, xml, xml);
        }

        private int newCount = 1;

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newCount++;
            tabControl1.SelectedTab = AddTab("template.xml", "New " + newCount, null);
        }

        private string GetFullName(string exe)
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath);
            return Path.Combine(path, exe);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl1.SelectedTab.Controls[0].Focus();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabCount < 2) return;
            tabControl1.TabPages.Remove(tabControl1.SelectedTab);
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            closeToolStripMenuItem.Enabled = tabControl1.TabCount > 1;
        }
    }
}
