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
                        case "[":
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

        private delegate void DeclareHandler(string name, bool eq, int ln, int lp, int? array);

        private void ReadDeclare(string category, Action delg1, DeclareHandler delg2)
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

            int? array = null;
            var br1 = Read();
            if (br1 == "[")
            {
                var len = Read();
                if (!Tokenizer.IsDigit(len))
                {
                    Rewind();
                    throw Abort("{0}: 配列のサイズが必要です。", category);
                }
                array = int.Parse(len);
                Check(category, "]");
            }
            else
                Rewind();

            if (delg1 != null) delg1();

            var eq = Read();
            if (eq == "=")
                delg2(name, true, ln, lp, array);
            else
            {
                if (eq != null) Rewind();
                delg2(name, false, ln, lp, array);
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
                (name, eq, ln, lp, array) =>
                {
                    if (array != null)
                        throw parent.Abort(ln, lp, "const int: 配列は宣言できません。");
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
                (name, eq, ln, lp, array) =>
                {
                    if (array != null)
                        throw parent.Abort(ln, lp, "const string: 配列は宣言できません。");
                    if (!eq)
                        throw Abort("const string: 等号がありません。");
                    var v = String();
                    if (v == null)
                        throw Abort("const string: 文字列が必要です。");
                    parent.AddString(name, v.Value);
                });
        }

        private Pointer.Declare[] VarDeclare()
        {
            var list = new List<Pointer.Declare>();
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
                (name, eq, ln, lp, array) =>
                {
                    Pointer.Declare p;
                    if (array == null)
                    {
                        p = new Var.Declare(parent, name, type);
                        if (eq) (p as Var.Declare).Value = Expression();
                    }
                    else
                    {
                        if (eq)
                            throw parent.Abort(ln, lp, "var: 配列を初期化できません。");
                        var type2 = type == null ? "var" : "var:" + type;
                        p = new Pointer.Declare(parent, name, type2, (int)array);
                    }
                    p.SetLine(ln, lp);
                    list.Add(p);
                });
            return list.ToArray();
        }

        private Pointer.Declare[] StructDeclare(string type)
        {
            var list = new List<Pointer.Declare>();
            ReadDeclare(type, null,
                (name, eq, ln, lp, array) =>
                {
                    Pointer.Declare p;
                    if (array == null)
                    {
                        p = new Struct.Declare(parent, name, type);
                        if (eq) ReadInitializers(p as Struct.Declare, type);
                    }
                    else
                    {
                        if (eq)
                            throw parent.Abort(ln, lp, "var: 配列を初期化できません。");
                        p = new Pointer.Declare(parent, name, type, (int)array);
                    }
                    p.SetLine(ln, lp);
                    list.Add(p);
                });
            return list.ToArray();
        }

        private void ReadInitializers(Struct.Declare st, string type)
        {
            var ret = new List<object>();
            Check(type, "{");
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
