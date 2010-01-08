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

        public void SetLine(XmlTextReader xr)
        {
            lineNumber = xr.LineNumber;
            linePosition = xr.LinePosition;
        }

        public void SetLine(int lineNumber, int linePosition)
        {
            this.lineNumber = lineNumber;
            this.linePosition = linePosition;
        }

        public static void Parse(XmlTextReader xr, Action delg)
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

        public Exception Abort(string format, params object[] args)
        {
            return Abort(lineNumber, linePosition, format, args);
        }

        public Exception Abort(int lineNumber, int linePosition, string format, params object[] args)
        {
            string s1 = "", s2 = "", src = root.Source;
            if (src != null) s1 = src + ": ";
            if (lineNumber > 0)
                s2 = string.Format("[{0}:{1}] ", lineNumber, linePosition);
            return new Exception(s1 + s2 + string.Format(format, args));
        }

        public Exception Abort(XmlTextReader xr, string format, params object[] args)
        {
            return Abort(xr.LineNumber, xr.LinePosition, format, args);
        }

        public Exception Abort(XmlTextReader xr)
        {
            return Abort(xr, "”FŽ¯‚Å‚«‚Ü‚¹‚ñ: {0}", xr.Name);
        }

        public virtual void AddCodes(OpCodes codes) { }

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
