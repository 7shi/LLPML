using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Girl.LLPML.Struct;

namespace Girl.LLPML.Parsing
{
    public partial class Parser
    {
        private NodeBase[] ReadDeclare(bool isStatic)
        {
            if (!CanRead) return null;

            var t = Read();
            if (t == "var")
                return DeclareVar(isStatic);
            else if (t == "delegate" && Peek() != "(")
                return DelegateDeclare(isStatic);

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
                            return TypedDeclare(t, isStatic);
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
                    ConstIntDeclare();
                    return;
                case "string":
                    ConstStringDeclare();
                    return;
            }

            //Rewind();
            throw Abort("const: 型が指定されていません。");
        }

        private void ReadDeclareInternal(string category, Action delg1, Action<string, bool, SrcInfo, NodeBase> delg2)
        {
            if (!CanRead) throw Abort("{0}: 名前が必要です。", category);

            var si = SrcInfo;
            var name = Read();
            if (!Tokenizer.IsWord(name))
            {
                //Rewind();
                throw Abort("{0}: 名前が不適切です: {1}", category, name);
            }

            NodeBase array = null;
            var br1 = Read();
            if (br1 == "[")
            {
                array = ReadExpression();
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
                ReadDeclareInternal(category, delg1, delg2);
        }

        private void ConstIntDeclare()
        {
            ReadDeclareInternal("const int", null,
                delegate(string name, bool eq, SrcInfo si, NodeBase array)
                {
                    if (array != null)
                        throw parent.AbortInfo(si, "const int: 配列は宣言できません。");
                    if (!eq)
                        throw Abort("const int: 等号がありません。");
                    parent.AddInt(name, ReadExpression());
                });
        }

        private void ConstStringDeclare()
        {
            ReadDeclareInternal("const string", null,
                delegate(string name, bool eq, SrcInfo si, NodeBase array)
                {
                    if (array != null)
                        throw parent.AbortInfo(si, "const string: 配列は宣言できません。");
                    if (!eq)
                        throw Abort("const string: 等号がありません。");
                    var v = ReadString();
                    if (v == null)
                        throw Abort("const string: 文字列が必要です。");
                    parent.AddString(name, v.Value);
                });
        }

        private VarDeclare[] DeclareVar(bool isStatic)
        {
            var list = new ArrayList();
            string type = null;
            ReadDeclareInternal("var",
                delegate()
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
                        //if (type != null) Rewind();
                        throw Abort("var: 型が必要です。");
                    }
                    var ar = Read();
                    if (ar == "*")
                        type += ar;
                    else if (ar == "[")
                    {
                        ar = Read();
                        if (ar == "]")
                            type += "[]";
                        else
                        {
                            if (ar != null) Rewind();
                            Rewind();
                        }
                    }
                    else if (ar != null)
                        Rewind();
                },
                delegate(string name, bool eq, SrcInfo si, NodeBase array)
                {
                    VarDeclare v;
                    var tb = Types.GetType(parent, type);
                    if (array == null)
                    {
                        if (tb != null) tb = Types.ToVarType(tb);
                        try
                        {
                            var vd = VarDeclare.New(parent, name, tb);
                            if (eq) vd.Value = ReadExpression();
                            v = vd;
                        }
                        catch
                        {
                            throw parent.AbortInfo(si, "var: 宣言が重複しています: {0}", name);
                        }
                    }
                    else
                    {
                        /// TODO: 配列を初期化できるようにする
                        if (eq)
                            throw parent.AbortInfo(si, "var: 配列を初期化できません。");
                        v = VarDeclare.Array(parent, name, Types.ToVarType(tb), array);
                    }
                    v.SrcInfo = si;
                    v.IsStatic = isStatic;
                    list.Add(v);
                });
            var ret = new VarDeclare[list.Count];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = list[i] as VarDeclare;
            return ret;
        }

        private VarDeclare[] TypedDeclare(string type, bool isStatic)
        {
            var list = new ArrayList();
            ReadDeclareInternal(type, null,
                delegate(string name, bool eq, SrcInfo si, NodeBase array)
                {
                    VarDeclare v;
                    var tb = Types.GetType(parent, type);
                    if (array == null)
                    {
                        var vs = Types.GetValueType(type);
                        if (vs == null)
                        {
                            v = Declare.New(parent, name, type);
                            if (eq) ReadInitializers(v as Declare, type);
                        }
                        else
                        {
                            var vd = VarDeclare.New(parent, name, tb);
                            if (eq)
                            {
                                var ex = ReadExpression();
                                vd.Value = ex;
                            }
                            v = vd;
                        }
                    }
                    else
                    {
                        /// todo: 配列を初期化できるようにする
                        if (eq)
                            throw parent.AbortInfo(si, "{0}: 配列を初期化できません。", type);
                        if (tb == null) tb = TypeInt.Instance;
                        v = VarDeclare.Array(parent, name, tb, array);
                    }
                    v.SrcInfo = si;
                    v.IsStatic = isStatic;
                    list.Add(v);
                });
            var ret = new VarDeclare[list.Count];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = list[i] as VarDeclare;
            return ret;
        }

        private void ReadInitializers(Declare st, string type)
        {
            Check(type, "{");
            for (; ; )
            {
                if (Peek() == "{")
                {
                    var st2 = Declare.NewDecl(st);
                    st2.SrcInfo = SrcInfo;
                    ReadInitializers(st2, type);
                    st.Values.Add(st2);
                }
                else
                    st.Values.Add(ReadExpression());

                var t = Read();
                if (t == "}")
                    break;
                else if (t != ",")
                {
                    //if (t != null) Rewind();
                    throw Abort("{0}: }}}} が必要です。", type);
                }
            }
        }

        private VarDeclare[] DelegateDeclare(bool isStatic)
        {
            var list = new ArrayList();
            var type = DelgFunc.GetDefaultType(parent);
            ReadDeclareInternal("delegate", null,
                delegate(string name, bool eq, SrcInfo si, NodeBase array)
                {
                    VarDeclare v;
                    if (array == null)
                    {
                        var vd = VarDeclare.New(parent, name, type);
                        if (eq) vd.Value = ReadExpression();
                        v = vd;
                    }
                    else
                    {
                        if (eq)
                            throw parent.AbortInfo(si, "var: 配列を初期化できません。");
                        v = VarDeclare.Array(parent, name, type, array);
                    }
                    v.SrcInfo = si;
                    v.IsStatic = isStatic;
                    list.Add(v);
                });
            var ret = new VarDeclare[list.Count];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = list[i] as VarDeclare;
            return ret;
        }
    }
}
