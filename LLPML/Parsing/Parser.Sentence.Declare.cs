using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.LLPML.Parsing
{
    public partial class Parser
    {
        private NodeBase[] Declare()
        {
            if (!CanRead) return null;

            var t = Read();
            if (t == "var") return VarDeclare();

            if (Tokenizer.IsWord(t) && CanRead)
            {
                var name = Read();
                if (Tokenizer.IsWord(name) && CanRead)
                {
                    switch (Peek())
                    {
                        case ",":
                        case ";":
                        case "=":
                            Rewind();
                            return StructDeclare(t);
                    }
                }
                Rewind();
            }

            Rewind();
            return null;
        }

        private void ConstDeclare()
        {
            if (!CanRead) throw Abort("const: �^���w�肳��Ă��܂���B");

            var t = Read();
            switch (t)
            {
                case "int":
                    IntDeclare();
                    return;
                case "string":
                    StringDeclare();
                    return;
            }

            Rewind();
            throw Abort("const: �^���w�肳��Ă��܂���B");
        }

        private delegate void DeclareHandler(string name, bool eq, int ln, int lp);

        private void ReadDeclare(string category, VoidDelegate delg1, DeclareHandler delg2)
        {
            if (!CanRead) throw Abort("{0}: ���O���K�v�ł��B", category);

            var ln = tokenizer.LineNumber;
            var lp = tokenizer.LinePosition;
            var name = Read();
            if (!Tokenizer.IsWord(name))
            {
                Rewind();
                throw Abort("{0}: ���O���s�K�؂ł�: {1}", category, name);
            }

            if (delg1 != null) delg1();

            var eq = Read();
            if (eq == "=")
                delg2(name, true, ln, lp);
            else
            {
                if (eq != null) Rewind();
                delg2(name, false, ln, lp);
            }

            var sep = Read();
            if (sep != ",")
                Rewind();
            else if (sep != null)
                ReadDeclare(category, delg1, delg2);
        }

        private void IntDeclare()
        {
            ReadDeclare("const int", null,
                (name, eq, ln, lp) =>
                {
                    if (!eq)
                        throw Abort("const int: ����������܂���B");
                    var v = Expression() as IntValue;
                    if (v == null)
                        throw parent.Abort(ln, lp, "const int: �萔�l���K�v�ł��B");
                    parent.AddInt(name, v.Value);
                });
        }

        private void StringDeclare()
        {
            ReadDeclare("const string", null,
                (name, eq, ln, lp) =>
                {
                    if (!eq)
                        throw Abort("const string: ����������܂���B");
                    var v = String();
                    if (v == null)
                        throw Abort("const string: �����񂪕K�v�ł��B");
                    parent.AddString(name, v.Value);
                });
        }

        private Var.Declare[] VarDeclare()
        {
            var list = new List<Var.Declare>();
            string type = null;
            ReadDeclare("var",
                () =>
                {
                    type = null;

                    var t = Read();
                    if (t != ":")
                    {
                        if (t != null) Rewind();
                        return;
                    }

                    if (!CanRead)
                        throw Abort("var: �^���K�v�ł��B");
                    type = Read();
                    if (!Tokenizer.IsWord(type))
                    {
                        if (type != null) Rewind();
                        throw Abort("var: �^���K�v�ł��B");
                    }
                },
                (name, eq, ln, lp) =>
                {
                    var v = new Var.Declare(parent, name, type);
                    v.SetLine(ln, lp);
                    if (eq) v.Value = Expression();
                    list.Add(v);
                });
            return list.ToArray();
        }

        private Struct.Declare[] StructDeclare(string type)
        {
            var list = new List<Struct.Declare>();
            ReadDeclare(type, null,
                (name, eq, ln, lp) =>
                {
                    var st = new Struct.Declare(parent, name, type);
                    st.SetLine(ln, lp);
                    if (eq) ReadInitializers(st, type);
                    list.Add(st);
                });
            return list.ToArray();
        }

        private void ReadInitializers(Struct.Declare st, string type)
        {
            var ret = new List<object>();
            var br = Read();
            if (br != "{")
            {
                if (br != null) Rewind();
                throw Abort("{0}: {{{{ ���K�v�ł��B", type);
            }
            for (; ; )
            {
                if (Peek() == "{")
                {
                    var st2 = new Struct.Declare(st);
                    st2.SetLine(tokenizer.LineNumber, tokenizer.LinePosition);
                    ReadInitializers(st2, type);
                    st.Values.Add(st2);
                }
                else
                    st.Values.Add(Expression());

                var t = Read();
                if (t == "}")
                    break;
                else if (t != ",")
                {
                    if (t != null) Rewind();
                    throw Abort("{0}: }}}} ���K�v�ł��B", type);
                }
            }
        }
    }
}
