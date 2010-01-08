﻿using System;
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

        public SrcInfo(string source, int number, int position)
        {
            Source = source;
            Number = number;
            Position = position;
        }

        public SrcInfo(string source, XmlTextReader xr)
            : this(source, xr.LineNumber, xr.LinePosition)
        {
        }
    }
}