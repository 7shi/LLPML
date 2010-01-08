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

namespace Sample
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
        }

        private TextData selectedData;
        private TreeNode workArea, library, window, console;

        public Form1()
        {
            InitializeComponent();
            treeView1.Nodes.AddRange(new TreeNode[]
            {
                workArea = new TreeNode("ワークエリア"),
                library = new TreeNode("ライブラリ"),
                window = new TreeNode("ウィンドウ"),
                console = new TreeNode("コンソール"),
            });
            treeView1.ExpandAll();
            var dir = new DirectoryInfo(GetFullName("Samples"));
            foreach (var xml in dir.GetFiles("*.xml"))
            {
                var name = xml.Name;
                if (name.Length >= 2 && name[0] == 'c' && char.IsNumber(name[1]))
                    ReadSample(console, name);
                else if (name.Length >= 2 && name[0] == 'w' && char.IsNumber(name[1]))
                    ReadSample(window, name);
                else
                    ReadSample(library, name);
            }
            newToolStripMenuItem.PerformClick();
        }

        private void ReadSample(TreeNode parent, string xml)
        {
            var node = CreateItem(xml);
            var td = (node.Tag) as TextData;
            var sr = new StringReader(td.Text);
            var xr = new XmlTextReader(sr);
            var title = Root.ReadTitle(xr);
            if (title != "") node.Text += "(" + title + ")";
            xr.Close();
            sr.Close();
            parent.Nodes.Add(node);
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedData == null) return;

            var cur = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            DateTime start = DateTime.Now;
#if !DEBUG
            try
            {
#endif
            var sr = new StringReader(textBox1.Text);
            var xr = new XmlTextReader(sr);
            var root = new Root();
            root.StreamDelegate = name =>
            {
                var n = library.Nodes[name];
                if (n == null) n = console.Nodes[name];
                if (n == null) n = window.Nodes[name];
                if (n == null) return null;
                var td = n.Tag as TextData;
                if (td == null) return null;
                return new StringReader(td.Text);
            };
            if (selectedData.Output != null)
                root.Output = Path.GetFileNameWithoutExtension(selectedData.Output) + ".exe";
            root.Read(selectedData.Name, xr);
            xr.Close();
            sr.Close();

            var module = new Module();
            module.Specific.SubSystem = root.Subsystem;

            var codes = new OpModule(module);
            root.AddCodes(codes);
            module.Text.OpCodes = codes.ToArray();

            var exe = GetFullName(root.Output);
            module.Link(exe);
            var time = (DateTime.Now - start).TotalMilliseconds;
            textBox2.AppendText("出力: " + exe + "\r\n");
            textBox2.AppendText("所要時間: " + time + "ms\r\n");
            switch (root.Subsystem)
            {
                case IMAGE_SUBSYSTEM.WINDOWS_CUI:
                case IMAGE_SUBSYSTEM.WINDOWS_GUI:
                    Process.Start(exe);
                    break;
            }
#if !DEBUG
            }
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

        private TreeNode CreateItem(string xml, string title, string output)
        {
            var ret = new TreeNode(title);
            ret.Name = title;
            ret.Tag = new TextData(title, ReadSample(xml), output);
            return ret;
        }

        private TreeNode CreateItem(string xml)
        {
            return CreateItem(xml, xml, xml);
        }

        private int newCount = 1;

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var n = CreateItem("template.xml", "New " + newCount, null);
            workArea.Nodes.Add(n);
            treeView1.SelectedNode = n;
            newCount++;
        }

        private string GetFullName(string exe)
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath);
            return Path.Combine(path, exe);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var n = treeView1.SelectedNode;
            if (n == null || n.Parent == null) return;

            var nn = n.NextVisibleNode;
            if (nn == null) nn = n.PrevVisibleNode;
            treeView1.SelectedNode = nn;
            n.Remove();
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var n = treeView1.SelectedNode;
            closeToolStripMenuItem.Enabled = n != null && n.Parent != null;
            nextToolStripMenuItem.Enabled = n != null && n.NextVisibleNode != null;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TextData sel = null;
            var n = e.Node;
            if (n != null) sel = n.Tag as TextData;
            if (sel == selectedData) return;

            if (selectedData != null)
                selectedData.From(textBox1);
            selectedData = sel;
            if (selectedData != null)
            {
                textBox1.Enabled = true;
                selectedData.To(textBox1);
            }
            else
            {
                textBox1.Clear();
                textBox1.Enabled = false;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "LLPML ver." + Root.LLPMLVersion, "About...");
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var n = treeView1.SelectedNode;
            if (n == null) return;
            var nn = n.NextVisibleNode;
            if (nn == null) return;
            treeView1.SelectedNode = nn;
        }
    }
}
