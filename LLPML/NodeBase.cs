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
        public BlockBase Parent { get; set; }

        protected string name;
        public string Name { get { return name; } }

        protected Root root;
        public Root Root { get { return root; } }

        public virtual TypeBase Type { get { return null; } }

        public Parsing.SrcInfo SrcInfo { get; set; }

        public NodeBase()
        {
        }

        public NodeBase(BlockBase parent)
        {
            Parent = parent;
            root = parent.root;
        }

        public NodeBase(BlockBase parent, string name)
            : this(parent)
        {
            this.name = name;
        }

        public NodeBase(BlockBase parent, XmlTextReader xr)
            : this(parent)
        {
            SrcInfo = new Parsing.SrcInfo(root.Source, xr);
            Read(xr);
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

        private Parsing.SrcInfo GetSrcInfo()
        {
            var ret = SrcInfo;
            if (ret != null) return ret;

            if (Parent == null) return null;
            return Parent.GetSrcInfo();
        }

        public Exception Abort(string format, params object[] args)
        {
            return Abort(GetSrcInfo(), format, args);
        }

        public Exception Abort(Parsing.SrcInfo si, string format, params object[] args)
        {
            string s1 = "", s2 = "";
            if (si != null)
            {
                s1 = si.Source + ": ";
                s2 = string.Format("[{0}:{1}] ", si.Number, si.Position);
            }
            return new Exception(s1 + s2 + string.Format(format, args));
        }

        public Exception Abort(XmlTextReader xr, string format, params object[] args)
        {
            return Abort(new Parsing.SrcInfo(root.Source, xr), format, args);
        }

        public Exception Abort(XmlTextReader xr)
        {
            return Abort(xr, "”FŽ¯‚Å‚«‚Ü‚¹‚ñ: {0}", xr.Name);
        }

        public virtual void AddCodes(OpModule codes) { }
        public virtual void AddCodes(OpModule codes, string op, Addr32 dest) { }

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
