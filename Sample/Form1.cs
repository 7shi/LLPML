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

namespace _Sample
{
    public partial class Form1 : Form
    {
        private class TextData
        {
            public string Name, Text, Output;
            public int SelectionStart, SelectionLength;

            public TextData(string name, string text, string output)
            {
                Name = name;
                Text = text;
                Output = output;
            }

            public void From(TextBox tb)
            {
                Text = tb.Text;
                SelectionStart = tb.SelectionStart;
                SelectionLength = tb.SelectionLength;
            }

            public void To(TextBox tb)
            {
                tb.Clear();
                tb.Text = Text;
                tb.SelectionStart = SelectionStart;
                tb.SelectionLength = SelectionLength;
                tb.ScrollToCaret();
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private TextData selectedData;

        public Form1()
        {
#if DEBUG
            I386.Test();
#endif
            InitializeComponent();
            AddItem("stdio.xml");
            AddItem("finish.xml");
            AddItem("win32.xml");
            for (int i = 1; ; i++)
            {
                string xml = string.Format("{0:00}.xml", i);
                if (!File.Exists(GetSampleFileName(xml))) break;
                AddItem(xml);
            }
            newToolStripMenuItem.PerformClick();
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
                Root root = new Root();
                root.StreamDelegate = delegate(string name)
                {
                    foreach (TextData td in listBox1.Items)
                    {
                        if (td.Name == name)
                        {
                            return new StringReader(td.Text);
                        }
                    }
                    return null;
                };
                if (selectedData.Output != null)
                {
                    root.Output = Path.GetFileNameWithoutExtension(selectedData.Output) + ".exe";
                }
                root.Read(selectedData.Name, xr);
                xr.Close();
                sr.Close();

                Module module = new Module();
                module.Specific.SubSystem = root.Subsystem;

                List<OpCode> codes = new List<OpCode>();
                root.AddCodes(codes, module);
                module.Text.OpCodes = codes.ToArray();

                string exe = GetFullName(root.Output);
                module.Link(exe);
                textBox2.AppendText("�o��: " + exe + "\r\n");
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

        private string ReadSample(string xml)
        {
            StreamReader sr = new StreamReader(GetSampleFileName(xml));
            string ret = sr.ReadToEnd();
            sr.Close();
            return ret;
        }

        private TextData AddItem(string xml, string title, string output)
        {
            TextData ret = new TextData(title, ReadSample(xml), output);
            listBox1.Items.Add(ret);
            return ret;
        }

        private TextData AddItem(string xml)
        {
            return AddItem(xml, xml, xml);
        }

        private int newCount = 1;

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddItem("template.xml", "New " + newCount, null);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            newCount++;
        }

        private string GetFullName(string exe)
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath);
            return Path.Combine(path, exe);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = listBox1.SelectedIndex;
            if (id < 0) return;
            selectedData = null;
            listBox1.Items.RemoveAt(id);
            listBox1.SelectedIndex = Math.Min(id, listBox1.Items.Count - 1);
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            int c = listBox1.Items.Count;
            closeToolStripMenuItem.Enabled = c > 0;
            nextToolStripMenuItem.Enabled = listBox1.SelectedIndex < c - 1;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = listBox1.SelectedIndex;
            TextData sel = null;
            if (id >= 0) sel = listBox1.Items[id] as TextData;
            if (sel == selectedData) return;

            textBox1.Focus();
            if (selectedData != null) selectedData.From(textBox1);
            selectedData = sel;
            if (selectedData != null) selectedData.To(textBox1);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Root r = new Root();
            MessageBox.Show(this, "LLPML ver." + r.Version, "About...");
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = listBox1.SelectedIndex;
            if (id < listBox1.Items.Count - 1)
                listBox1.SelectedIndex = id + 1;
        }
    }
}