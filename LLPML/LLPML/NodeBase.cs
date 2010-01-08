using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class NodeBase
    {
        protected BlockBase parent;
        public BlockBase Parent { get { return parent; } }

        protected string name;
        public string Name { get { return name; } }

        protected Root root;
        public Root Root { get { return root; } }

        protected int lineNumber, linePosition;

        public NodeBase()
        {
        }

        public NodeBase(BlockBase parent)
        {
            this.parent = parent;
            root = parent.root;
        }

        public NodeBase(BlockBase parent, string name)
            : this(parent)
        {
            this.name = name;
        }

        public NodeBase(BlockBase parent, XmlTextReader xr) : this(parent)
        {
            SetLine(xr);
            Read(xr);
        }

        protected void SetLine(XmlTextReader xr)
        {
            lineNumber = xr.LineNumber;
            linePosition = xr.LinePosition;
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

        public Exception Abort(string msg)
        {
            return Abort(lineNumber, linePosition, msg);
        }

        public static Exception Abort(int lineNumber, int linePosition, string msg)
        {
            if (lineNumber < 1) return new Exception(msg);
            return new Exception(string.Format(
                "[{0}:{1}] {2}", lineNumber, linePosition, msg));
        }

        public static Exception Abort(XmlTextReader xr, string msg)
        {
            return Abort(xr.LineNumber, xr.LinePosition, msg);
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

        protected void NoChild(XmlTextReader xr)
        {
            if (!xr.IsEmptyElement)
                throw Abort(xr, "<" + xr.Name + "> can not have any children");
        }

        protected void RequiresName(XmlTextReader xr)
        {
            name = xr["name"];
            if (name == null) throw Abort(xr, "name required");
        }
    }
}
