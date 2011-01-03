using System;
using System.Collections.Generic;
using System.Text;
using Girl.LLPML.Parsing;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public abstract class NodeBase
    {
        private BlockBase parent;
        public BlockBase Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                root = parent.root;
            }
        }

        protected string name;
        public string Name { get { return name; } }

        protected Root root;
        public Root Root { get { return root; } }

        public virtual TypeBase Type { get { return null; } }

        private SrcInfo srcInfo;
        public SrcInfo SrcInfo
        {
            get
            {
                if (srcInfo != null)
                    return srcInfo;
                else if (Parent != null)
                    return Parent.SrcInfo;
                else
                    return null;
            }
            set { srcInfo = value; }
        }

        public Exception Abort(string format, params object[] args)
        {
            return AbortInfo(SrcInfo, format, args);
        }

        public Exception AbortInfo(SrcInfo si, string format, params object[] args)
        {
            var s1 = "";
            var s2 = "";
            if (si != null)
            {
                s1 = si.Source + ": ";
                s2 = string.Format("[{0}:{1}] ", si.Number, si.Position);
            }
            return new Exception(s1 + s2 + string.Format(format, args));
        }

        public virtual void AddCodes(OpModule codes) { }
        public virtual void AddCodesV(OpModule codes, string op, Addr32 dest) { }
    }
}
