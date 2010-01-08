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
            if (!CanRead) throw Abort("const: 型が指定されていません。");

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
            throw Abort("const: 型が指定されていません。");
        }

        private delegate void DeclareHandler(string name, bool eq, int ln, int lp);

        private void ReadDeclare(string category, VoidDelegate delg1, DeclareHandler delg2)
        {
            if (!CanRead) throw Abort("{0}: 名前が必要です。", category);

            var ln = tokenizer.LineNumber;
            var lp = tokenizer.LinePosition;
            var name = Read();
            if (!Tokenizer.IsWord(name))
            {
                Rewind();
                throw Abort("{0}: 名前が不適切です: {1}", category, name);
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
                        throw Abort("const int: 等号がありません。");
                    var v = Expression() as IntValue;
                    if (v == null)
                        throw parent.Abort(ln, lp, "const int: 定数値が必要です。");
                    parent.AddInt(name, v.Value);
                });
        }

        private void StringDeclare()
        {
            ReadDeclare("const string", null,
                (name, eq, ln, lp) =>
                {
                    if (!eq)
                        throw Abort("const string: 等号がありません。");
                    var v = String();
                    if (v == null)
                        throw Abort("const string: 文字列が必要です。");
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
                        throw Abort("var: 型が必要です。");
                    type = Read();
                    if (!Tokenizer.IsWord(type))
                    {
                        if (type != null) Rewind();
                        throw Abort("var: 型が必要です。");
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
                throw Abort("{0}: {{{{ が必要です。", type);
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
                    throw Abort("{0}: }}}} が必要です。", type);
                }
            }
        }
    }
}
