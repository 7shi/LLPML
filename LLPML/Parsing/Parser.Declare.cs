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
                            return TypedDeclare(t);
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

        private delegate void DeclareHandler(string name, bool eq, SrcInfo si, int? array);

        private void ReadDeclare(string category, Action delg1, DeclareHandler delg2)
        {
            if (!CanRead) throw Abort("{0}: ���O���K�v�ł��B", category);

            var si = tokenizer.SrcInfo;
            var name = Read();
            if (!Tokenizer.IsWord(name))
            {
                Rewind();
                throw Abort("{0}: ���O���s�K�؂ł�: {1}", category, name);
            }

            int? array = null;
            var br1 = Read();
            if (br1 == "[")
            {
                var len = Read();
                if (!Tokenizer.IsDigit(len))
                {
                    Rewind();
                    throw Abort("{0}: �z��̃T�C�Y���K�v�ł��B", category);
                }
                array = int.Parse(len);
                Check(category, "]");
            }
            else
                Rewind();

            if (delg1 != null) delg1();

            var eq = Read();
            if (eq == "=")
                delg2(name, true, si, array);
            else
            {
                if (eq != null) Rewind();
                delg2(name, false, si, array);
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
                (name, eq, si, array) =>
                {
                    if (array != null)
                        throw parent.Abort(si, "const int: �z��͐錾�ł��܂���B");
                    if (!eq)
                        throw Abort("const int: ����������܂���B");
                    var v = Expression() as IntValue;
                    if (v == null)
                        throw parent.Abort(si, "const int: �萔�l���K�v�ł��B");
                    parent.AddInt(name, v.Value);
                });
        }

        private void StringDeclare()
        {
            ReadDeclare("const string", null,
                (name, eq, si, array) =>
                {
                    if (array != null)
                        throw parent.Abort(si, "const string: �z��͐錾�ł��܂���B");
                    if (!eq)
                        throw Abort("const string: ����������܂���B");
                    var v = String();
                    if (v == null)
                        throw Abort("const string: �����񂪕K�v�ł��B");
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
                        throw Abort("var: �^���K�v�ł��B");
                    type = Read();
                    if (!Tokenizer.IsWord(type))
                    {
                        if (type != null) Rewind();
                        throw Abort("var: �^���K�v�ł��B");
                    }
                    var ar = Read();
                    if (ar == "[")
                    {
                        Check("var", "]");
                        type += "[]";
                    }
                    else if (ar != null)
                        Rewind();
                },
                (name, eq, si, array) =>
                {
                    Pointer.Declare p;
                    if (array == null)
                    {
                        var vd = new Var.Declare(parent, name, type);
                        if (eq)
                        {
                            var ex = Expression();
                            vd.Value = ex;
                            if (type == null)
                            {
                                // �^���_
                                if (ex is VarBase)
                                {
                                    var vb = ex as VarBase;
                                    if (vb.TypeName != null)
                                    {
                                        if (vb.IsArray)
                                            vd.TypeName = vb.TypeName + "[]";
                                        else
                                            vd.TypeName = vb.TypeName;
                                    }
                                }
                            }
                        }
                        p = vd;
                    }
                    else
                    {
                        if (eq)
                            throw parent.Abort(si, "var: �z����������ł��܂���B");
                        var type2 = type == null ? "var" : "var:" + type;
                        p = new Pointer.Declare(parent, name, type2, (int)array);
                    }
                    p.SrcInfo = si;
                    list.Add(p);
                });
            return list.ToArray();
        }

        private Pointer.Declare[] TypedDeclare(string type)
        {
            var list = new List<Pointer.Declare>();
            ReadDeclare(type, null,
                (name, eq, si, array) =>
                {
                    Pointer.Declare p;
                    if (array == null)
                    {
                        var vs = SizeOf.GetValueSize(type);
                        if (vs == 0)
                        {
                            p = new Struct.Declare(parent, name, type);
                            if (eq) ReadInitializers(p as Struct.Declare, type);
                        }
                        else
                        {
                            var vd = new Var.Declare(parent, name, type);
                            if (eq)
                            {
                                var ex = Expression();
                                vd.Value = ex;
                            }
                            p = vd;
                        }
                    }
                    else
                    {
                        if (eq)
                            throw parent.Abort(si, "{0}: �z����������ł��܂���B", type);
                        p = new Pointer.Declare(parent, name, type, (int)array);
                    }
                    p.SrcInfo = si;
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
                    st2.SrcInfo = tokenizer.SrcInfo;
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
