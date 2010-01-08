using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public delegate void VoidDelegate();

    public class NodeBase
    {
        protected Block parent;
        public Block Parent { get { return parent; } }

        protected string name;
        public string Name { get { return name; } }

        protected Root root;
        public Root Root { get { return root; } }

        public NodeBase()
        {
        }

        public NodeBase(Block parent)
        {
            this.parent = parent;
            root = parent.root;
        }

        public NodeBase(Block parent, string name)
            : this(parent)
        {
            this.name = name;
        }

        public NodeBase(Block parent, XmlTextReader xr) : this(parent)
        {
            Read(xr);
        }

        public static void Parse(XmlTextReader xr, VoidDelegate delg)
        {
            string self = xr.Name;
            bool empty = xr.IsEmptyElement;
            while (!empty && xr.Read())
            {
                if (xr.Name == self && xr.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                if (delg != null) delg();
            }
        }

        public virtual void Read(XmlTextReader xr)
        {
            Parse(xr, null);
        }

        public static Exception Abort(XmlTextReader xr, string msg)
        {
            return new Exception(string.Format(
                "[{0}:{1}] {2}", xr.LineNumber, xr.LinePosition, msg));
        }

        public static Exception Abort(XmlTextReader xr)
        {
            return Abort(xr, "invalid element: " + xr.Name);
        }

        public virtual void AddCodes(List<OpCode> codes, Module m) { }

        public void Set(NodeBase src)
        {
            parent = src.parent;
            root = src.root;
        }

        public int Level
        {
            get
            {
                return parent == null ? 0 : parent.Level + 1;
            }
        }

        protected void RequireName(XmlTextReader xr)
        {
            name = xr["name"];
            if (name == null) throw Abort(xr, "name required");
        }

        protected void NoChild(XmlTextReader xr)
        {
            if (!xr.IsEmptyElement)
                throw Abort(xr, "<" + xr.Name + "> can not have any children");
        }
    }
}
