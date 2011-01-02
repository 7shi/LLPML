using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Girl.LLPML.Parsing
{
    public class SrcInfo
    {
        public string Source { get; private set; }
        public int Number { get; private set; }
        public int Position { get; private set; }

        public static SrcInfo New(string source, int number, int position)
        {
            var ret = new SrcInfo();
            ret.Source = source;
            ret.Number = number;
            ret.Position = position;
            return ret;
        }

        public override string ToString()
        {
            return string.Format(
                "{0}: {1}, {2}", Source, Number, Position);
        }
    }
}
