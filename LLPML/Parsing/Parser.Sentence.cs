using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Girl.LLPML.Struct;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Parsing
{
    public partial class Parser
    {
        private NodeBase[] Sentence()
        {
            return SentenceWith(";");
        }

        private NodeBase[] SentenceWith(string separator)
        {
            if (!CanRead) throw Abort("文がありません。");

            var si = SrcInfo;
            var t = Read();
            NodeBase nb = null;
            switch (t)
            {
                case "#":
                    Directive();
                    return null;
                case "{":
                    Rewind();
                    nb = Block.New(parent);
                    ReadBlock("block", nb as Block);
                    break;
                case "struct":
                case "class":
                    StructDefine(t).SrcInfo = si;
                    return null;
                case "function":
                case "virtual":
                case "override":
                    ReadFunction(t, false).SrcInfo = si;
                    return null;
                case "static":
                    if (Read() == "function")
                    {
                        ReadFunction(t, true).SrcInfo = si;
                        return null;
                    }
                    Rewind();
                    return ReadDeclare(true);
                case "extern":
                    ReadExtern();
                    return null;
                case "if":
                    nb = ReadIf();
                    break;
                case "while":
                    nb = ReadWhile();
                    break;
                case "for":
                    nb = ReadFor();
                    break;
                case "switch":
                    nb = ReadSwitch();
                    break;
                case "new":
                    nb = Expression.New(parent, ReadNew());
                    break;
            }
            if (nb != null)
            {
                nb.SrcInfo = si;
                var ret = new NodeBase[1];
                ret[0] = nb;
                return ret;
            }
            Rewind();

            var list = new ArrayList();
            var s1 = SentenceBody();
            if (s1 != null)
            {
                for (int i = 0; i < s1.Length; i++)
                    list.Add(s1[i]);
            }
            var sep = Read();
            if (sep == ",")
            {
                var s2 = Sentence();
                if (s2 != null)
                {
                    for (int i = 0; i < s2.Length; i++)
                        list.Add(s2[i]);
                }
            }
            else if (string.IsNullOrEmpty(separator))
            {
                Rewind();
            }
            else if (sep != separator)
            {
                //if (sep != null) Rewind();
                throw Abort("{0} が必要です。", separator);
            }
            if (list.Count == 0) return null;
            var ret2 = new NodeBase[list.Count];
            for (int i = 0; i < ret2.Length; i++)
                ret2[i] = list[i] as NodeBase;
            return ret2;
        }

        private NodeBase[] SentenceBody()
        {
            if (!CanRead) throw Abort("文が必要です。");

            var t = Read();
            if (t == "const")
            {
                ConstDeclare();
                return null;
            }
            else
            {
                var si = SrcInfo;
                var nb = CheckReserved(t);
                if (nb != null)
                {
                    nb.SrcInfo = si;
                    var ret = new NodeBase[1];
                    ret[0] = nb;
                    return ret;
                }
            }

            Rewind();
            if (t == ";") return null;

            var dec = ReadDeclare(false);
            if (dec != null) return dec;

            return SentenceExpression();
        }

        private NodeBase[] SentenceExpression()
        {
            var e = Expression.New(parent, ReadExpression());
            e.SrcInfo = SrcInfo;
            var ret = new NodeBase[1];
            ret[0] = e;
            return ret;
        }

        private NodeBase CheckReserved(string t)
        {
            switch (t)
            {
                case "do":
                    return ReadDo();
                case "return":
                    if (CanRead && Peek() != ";")
                        return Return.New(parent, ReadExpression());
                    else
                        return Return.New(parent, null);
                case "break":
                    {
                        var brk = Break.New(parent);
                        if (!brk.CanBreak())
                            throw Abort("break: ここでは使用できません。");
                        return brk;
                    }
                case "continue":
                    {
                        var con = Continue.New(parent);
                        if (!con.CanContinue())
                            throw Abort("continue: ここでは使用できません。");
                        return con;
                    }
            }
            return null;
        }

        private Define StructDefine(string type)
        {
            if (!CanRead) throw Abort("{0}: 名前が必要です。", type);

            var name = Read();
            if (!Tokenizer.IsWord(name))
            {
                //Rewind();
                throw Abort("{0}: 名前が不適切です: {1}", type, name);
            }

            string baseType = null;
            var t = Read();
            if (t == ":")
            {
                if (!CanRead)
                    throw Abort("{0}: {1}: 型が必要です。", type, name);
                baseType = Read();
                if (!Tokenizer.IsWord(baseType))
                {
                    //Rewind();
                    throw Abort("{0}: {1}: 型が必要です。", type, name);
                }
                if (name == baseType)
                    throw Abort("{0}: {1}: 継承できません: {2}", type, name, baseType);
            }
            else
                Rewind();

            var ret = parent.GetStruct(name);
            var first = ret == null;
            if (first)
                ret = Define.New(parent, name, baseType);
            else if (baseType != null)
            {
                if (ret.BaseType == null)
                    ret.BaseType = baseType;
                else if (baseType != ret.BaseType)
                    throw Abort("{0}: 基本クラスが異なります: {1} != {2}",
                        ret.FullName, ret.BaseType, baseType);
            }
            if (type == "class")
            {
                if (!first && !ret.IsClass)
                    throw Abort("{0}: {1}: 構造体として定義されています。", type, name);
                ret.IsClass = true;
            }
            else if (!first && ret.IsClass)
                throw Abort("{0}: {1}: クラスとして定義されています。", type, name);
            ReadBlock(type, ret);
            if (first && !parent.AddStruct(ret))
                throw Abort("{0}: {1}: 定義が重複しています。", type, name);
            return ret;
        }

        private void Check(string type, string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            var t = Read();
            if (t != text)
            {
                //if (t != null) Rewind();
                throw Abort("{0}: {1} が必要です。", type, text);
            }
        }

        private void ReadBlock(string type, BlockBase block)
        {
            Check(type, "{");

            var parent = this.parent;
            this.parent = block;

            for (; ; )
            {
                var t = Read();
                if (t == null)
                    throw Abort("{0}: {1}: }}}} が必要です。", type, block.Name);
                if (t == "}") break;

                Rewind();
                var s = Sentence();
                if (s != null) block.AddSentences(s);
            }

            this.parent = parent;
        }

        private NodeBase AutoDelegate(Function f)
        {
            if (f == null) return null;

            var autoArgs = f.GetAutoArgs();
            if (autoArgs == null) return f;

            var args = new NodeBase[autoArgs.Length];
            for (int i = 0; i < args.Length; i++)
                args[i] = Var.NewName(parent, autoArgs[i].Name);
            var ret = Delegate.New(parent, f.CallType, args, f);
            ret.Auto = true;
            return ret;
        }

        private Function ReadFunction(string type, bool isStatic)
        {
            if (!CanRead) throw Abort("{0}: 定義が必要です。", type);

            var name = Read();
            CallType ct = CheckCallType(CallType.CDecl, ref name);
            if (name == "(" || name == ":" || name == "{")
            {
                name = "";
                Rewind();
            }
            else if (!Tokenizer.IsWord(name))
            {
                //Rewind();
                throw Abort("{0}: 名前が不適切です: {1}", type, name);
            }

            var ret = Function.New(this.parent, name, isStatic);
            ret.CallType = ct;
            if (Peek() == "(") ReadArgs(type, ret);

            var comma = Read();
            if (comma == ":")
            {
                var ftype = Read();
                if (!Tokenizer.IsWord(ftype))
                {
                    //Rewind();
                    throw Abort("{0}: 型が不適切です: {1}", type, ftype);
                }
                ret.SetReturnType(Types.GetVarType(this.parent, ftype));
            }
            else
                Rewind();

            ReadBlock(type, ret);

            if (!ret.Parent.AddFunction(ret))
                throw Abort("{0}: {1}: 定義が重複しています。", type, ret.Name);
            if (type == "virtual")
                ret.IsVirtual = true;
            else if (type == "override")
                ret.IsOverride = true;
            return ret;
        }

        private Extern[] ReadExtern()
        {
            if (!CanRead) throw Abort("extern: 名前が必要です。");

            var module = String();
            if (module == null) throw Abort("extern: モジュール名が必要です。");

            var t = Peek();
            CallType ct1 = CheckCallType(CallType.CDecl, ref t);
            t = Peek();
            var sfx1 = CheckSuffix(null, ref t);
            var br1 = Read();
            var loop = false;
            if (br1 == "{") loop = true; else Rewind();

            var list = new ArrayList();
            for (; ; )
            {
                var si = SrcInfo;
                var name = Read();
                CallType ct2 = CheckCallType(ct1, ref name);
                var sfx2 = CheckSuffix(sfx1, ref name);
                if (!Tokenizer.IsWord(name))
                {
                    //Rewind();
                    throw Abort("extern: 名前が不適切です: {0}", name);
                }
                string alias = null;
                if (sfx2 != null) alias = name + sfx2;

                var ex = Extern.New(parent, name, module.Value, alias);
                ex.SrcInfo = si;
                ex.CallType = ct2;
                if (Peek() == "(") ReadArgs("extern", ex);

                var comma = Read();
                if (comma == ":")
                {
                    var ftype = Read();
                    if (!Tokenizer.IsWord(ftype))
                    {
                        //Rewind();
                        throw Abort("extern: 型が不適切です: {0}", ftype);
                    }
                    ex.SetReturnType(Types.GetVarType(this.parent, ftype));
                }
                else
                    Rewind();

                if (!parent.AddFunction(ex))
                    throw Abort("extern: {0}: 定義が重複しています。", name);
                list.Add(ex);

                if (!loop) break;
                Check("extern", ";");
                var br2 = Read();
                if (br2 == "}") break;
                Rewind();
            }
            var ret = new Extern[list.Count];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = list[i] as Extern;
            return ret;
        }

        private CallType CheckCallType(CallType ct, ref string t)
        {
            if (t == "__stdcall")
            {
                t = Read();
                return CallType.Std;
            }
            else if (t == "__cdecl")
            {
                t = Read();
                return CallType.CDecl;
            }
            return ct;
        }

        private string CheckSuffix(string sfx, ref string t)
        {
            if (t == "__widecharset")
            {
                t = Read();
                return "W";
            }
            else if (t == "__ansicharset")
            {
                t = Read();
                return "A";
            }
            else if (t == "__nocharset")
            {
                t = Read();
                return null;
            }
            return sfx;
        }

        private void ReadArgs(string tp, Function f)
        {
            Check(tp, "(");
            var first = true;
            for (; ; )
            {
                var si = SrcInfo;
                var t = Read();
                if (t == null)
                    throw Abort("{0}: {1}: ) が必要です。", tp, f.Name);
                if (t == ")") break;
                if (first)
                    Rewind();
                else if (t != ",")
                    throw Abort("{0}: {1}: ) が必要です。", tp, f.Name);
                first = false;

                var arg = Read();
                if (!Tokenizer.IsWord(arg))
                {
                    //if (arg != null) Rewind();
                    throw Abort("{0}: {1}: 引数の名前が不適切です: {1}", tp, f.Name, arg);
                }

                string type = null;
                var colon = Read();
                if (colon == ":")
                {
                    if (!CanRead)
                        throw Abort("{0}: {1}: {2}: 引数に型が必要です。", tp, f.Name, arg);
                    type = Read();
                    if (type == "params")
                    {
                        f.Args.Add(ArgPtr.New(f, arg));
                        continue;
                    }
                    else if (!Tokenizer.IsWord(type))
                    {
                        //if (type != null) Rewind();
                        throw Abort("{0}: {1}: {2}: 引数の型が不適切です。", tp, f.Name, arg);
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
                }
                else if (colon != null)
                    Rewind();

                var argt = Types.GetVarType(parent, type);
                var farg = Arg.New(f, arg, argt);
                farg.SrcInfo = si;
                f.Args.Add(farg);
            }
        }

        private Cond ReadCond(BlockBase parent, string type)
        {
            return ReadCondWith(parent, type, "(", ")");
        }

        private Cond ReadCondWith(BlockBase parent, string type, string start, string end)
        {
            Check(type, start);
            if (Read() == end) return null;
            Rewind();

            var p = this.parent;
            this.parent = parent;
            var ret = Cond.New(parent, ReadExpression());
            this.parent = p;

            Check(type, end);

            return ret;
        }

        private Block ReadSentenceBlock(BlockBase parent, string type)
        {
            return ReadSentenceBlockWith(parent, type, null, ";");
        }

        private Block ReadSentenceBlockWith(BlockBase parent, string type, BlockBase target, string separator)
        {
            Block ret = null;
            var p = this.parent;
            var si = SrcInfo;
            if (Peek() == "{")
            {
                this.parent = parent;
                var s = SentenceWith(separator);
                if (s != null && s.Length == 1) ret = s[0] as Block;
            }
            else
            {
                ret = Block.New(parent);
                if (target == null) target = ret;
                this.parent = target;
                var s = SentenceWith(separator);
                if (s != null) ret.AddSentences(s);
            }
            if (ret == null)
                throw p.AbortInfo(si, "{0}: ブロックが必要です。", type);
            this.parent = p;
            return ret;
        }

        private While ReadWhile()
        {
            var ret = While.New(parent);
            ret.Cond = ReadCond(ret, "while");
            ret.Block = ReadSentenceBlock(ret, "while");
            return ret;
        }

        private Do ReadDo()
        {
            var ret = Do.New(parent);
            ret.Block = ReadSentenceBlock(ret, "do");
            var t = Read();
            if (t != "while")
            {
                //if (t != null) Rewind();
                throw Abort("do: while がありません。");
            }
            ret.Cond = ReadCond(ret, "do");
            return ret;
        }

        private If ReadIf()
        {
            var ret = If.New(parent);
            ReadIfInternal(ret);
            return ret;
        }

        private void ReadIfInternal(If target)
        {
            var cb1 = new If.CondBlock(target);
            cb1.Cond = ReadCond(target, "if");
            cb1.Block = ReadSentenceBlock(target, "if");
            target.Blocks.Add(cb1);

            var t1 = Read();
            if (t1 != "else")
            {
                if (t1 != null) Rewind();
                return;
            }

            var t2 = Read();
            if (t2 == "if")
            {
                ReadIfInternal(target);
                return;
            }

            Rewind();
            var cb2 = new If.CondBlock(target);
            cb2.Block = ReadSentenceBlock(target, "else");
            target.Blocks.Add(cb2);
        }

        private For ReadFor()
        {
            Check("for", "(");
            var ret = For.New(parent);
            ret.Init = ReadSentenceBlockWith(ret, "for", ret, ";");
            ret.Cond = ReadCondWith(ret, "for", "", ";");
            if (Read() != ")")
            {
                Rewind();
                ret.Loop = ReadSentenceBlockWith(ret, "for", null, "");
                Check("for", ")");
            }
            ret.Block = ReadSentenceBlock(ret, "for");
            return ret;
        }

        private Switch ReadSwitch()
        {
            Check("switch", "(");

            var si = SrcInfo;
            var expr = ReadExpression() as NodeBase;
            if (expr == null)
                throw parent.AbortInfo(si, "switch: 値が必要です。");

            Check("switch", ")");
            Check("switch", "{");

            var ret = Switch.New(parent, expr);
            ReadCase(ret);
            return ret;
        }

        private void ReadCase(Switch target)
        {
            var scb = new CaseBlock();
            for (; ; )
            {
                var t = Read();
                switch (t)
                {
                    case null:
                        throw Abort("switch: }}}} が必要です。");
                    case "case":
                    case "default":
                        if (scb.Block != null)
                        {
                            target.Blocks.Add(scb);
                            scb = new CaseBlock();
                        }
                        if (scb.Case == null)
                            scb.Case = Case.New(target);
                        if (t == "case")
                        {
                            var v = ReadExpression() as NodeBase;
                            if (v == null)
                                throw Abort("case: 値が必要です。");
                            Check("case", ":");
                            scb.Case.Values.Add(v);
                        }
                        else
                            Check("default", ":");
                        break;
                    case "}":
                        if (scb.Block != null) target.Blocks.Add(scb);
                        return;
                    default:
                        {
                            Rewind();
                            if (scb.Case == null)
                                throw Abort("switch: 条件が必要です。");
                            if (scb.Block == null)
                                scb.Block = Block.New(target);
                            var p = parent;
                            parent = target;
                            var s = Sentence();
                            if (s != null) scb.Block.AddSentences(s);
                            parent = p;
                            break;
                        }
                }
            }
        }

        private void Directive()
        {
            var t = Read();
            if (t == null) throw Abort("ディレクティブが必要です。");

            switch (t)
            {
                case "pragma":
                    Pragma();
                    break;
                default:
                    throw Abort("不明なディレクティブです: {0}", t);
            }
        }

        private void Pragma()
        {
            var t = Read();
            if (t == null) throw Abort("pragma: 指示が必要です。");

            switch (t)
            {
                case "subsystem":
                    PragmaSubsystem();
                    break;
                case "output":
                    PragmaOutput();
                    break;
                default:
                    throw Abort("pragma: 不明な指示です: {0}", t);
            }
        }

        private void PragmaSubsystem()
        {
            Check("pragma: subsystem", "(");

            var t = Read();
            if (t == null)
                throw Abort("pragma: subsystem: サブシステム名が必要です。");
            else if (!parent.Root.SetSubsystem(t))
                throw Abort("pragma: subsystem: 不明なサブシステム名です: {0}", t);

            Check("pragma: subsystem", ")");
        }

        private void PragmaOutput()
        {
            Check("pragma: output", "(");

            var output = String();
            if (output == null)
                throw Abort("pragma: output: 出力名を文字列で指定してください。");

            parent.Root.Output = output.Value;
            Check("pragma: output", ")");
        }
    }
}
