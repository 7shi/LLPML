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

        public virtual void Read(Root root, XmlTextReader xr)
        {
            Parse(xr, null);
        }

        public static Exception Abort(XmlTextReader xr, string msg)
        {
            return new Exception(string.Format(
                "[{0}:{1}] {2}", xr.LineNumber, xr.LinePosition, msg));
        }

        protected Exception Abort(XmlTextReader xr)
        {
            return Abort(xr, "invalid element: " + xr.Name);
        }

        public virtual void AddCodes(List<OpCode> codes, Module m) { }
    }
}
