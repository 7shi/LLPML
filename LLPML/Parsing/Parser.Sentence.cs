using System;
using System.Collections.Generic;
using System.Text;
using Girl.X86;

namespace Girl.LLPML.Parsing
{
    public partial class Parser
    {
        private NodeBase[] Sentence()
        {
            return Sentence(";");
        }

        private NodeBase[] Sentence(string separator)
        {
            if (!CanRead) throw Abort("文がありません。");

            var si = SrcInfo;
            var t = Read();
            NodeBase nb = null;
            switch (t)
            {
                case "{":
                    Rewind();
                    nb = new Block(parent);
                    ReadBlock("block", nb as Block);
                    break;
                case "struct":
                    StructDefine().SrcInfo = si;
                    return null;
                case "function":
                case "virtual":
                case "override":
                    Function(t, false).SrcInfo = si;
                    return null;
                case "static":
                    {
                        var tt = Read();
                        if (tt == "function")
                        {
                            Function(t, true).SrcInfo = si;
                            return null;
                        }
                        Rewind();
                        return Declare(true);
                    }
                case "extern":
                    Extern();
                    return null;
                case "if":
                    nb = If();
                    break;
                case "while":
                    nb = While();
                    break;
                case "for":
                    nb = For();
                    break;
                case "switch":
                    nb = Switch();
                    break;
                case "delete":
                    nb = new Struct.Delete(parent, Expression()) { SrcInfo = si };
                    break;
            }
            if (nb != null)
            {
                nb.SrcInfo = si;
                return new NodeBase[] { nb };
            }
            Rewind();

            var list = new List<NodeBase>();
            var s1 = SentenceBody();
            if (s1 != null) list.AddRange(s1);
            var sep = Read();
            if (sep == ",")
            {
                var s2 = Sentence();
                if (s2 != null) list.AddRange(s2);
            }
            else if (string.IsNullOrEmpty(separator))
            {
                Rewind();
            }
            else if (sep != separator)
            {
                if (sep != null) Rewind();
                throw Abort("{0} が必要です。", separator);
            }
            if (list.Count == 0) return null;
            return list.ToArray();
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
                    return new NodeBase[] { nb };
                }
            }

            Rewind();
            if (t == ";") return null;

            var dec = Declare(false);
            if (dec != null) return dec;

            var e = Expression() as NodeBase;
            return new NodeBase[] { e };
        }

        private NodeBase CheckReserved(string t)
        {
            switch (t)
            {
                case "do":
                    return Do();
                case "return":
                    if (CanRead && Peek() != ";")
                        return new Return(parent, Expression());
                    return new Return(parent);
                case "break":
                    {
                        var brk = new Break(parent);
                        if (!brk.CanBreak())
                            throw Abort("break: ここでは使用できません。");
                        return brk;
                    }
                case "continue":
                    {
                        var con = new Continue(parent);
                        if (!con.CanContinue())
                            throw Abort("continue: ここでは使用できません。");
                        return con;
                    }
            }
            return null;
        }

        private Struct.Define StructDefine()
        {
            if (!CanRead) throw Abort("struct: 名前が必要です。");

            var name = Read();
            if (!Tokenizer.IsWord(name))
            {
                Rewind();
                throw Abort("struct: 名前が不適切です: {0}", name);
            }

            string baseType = null;
            var t = Read();
            if (t == ":")
            {
                if (!CanRead)
                    throw Abort("struct: {0}: 型が必要です。", name);
                baseType = Read();
                if (!Tokenizer.IsWord(baseType))
                {
                    Rewind();
                    throw Abort("struct: {0}: 型が必要です。", name);
                }
                if (name == baseType)
                    throw Abort("struct: {0}: 継承できません: {1}", name, baseType);
            }
            else
                Rewind();

            var ret = new Struct.Define(parent, name, baseType);
            ReadBlock("struct", ret);
            if (!parent.AddStruct(ret))
                throw Abort("struct: {0}: 定義が重複しています。", name);
            return ret;
        }

        private void Check(string type, string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            var t = Read();
            if (t != text)
            {
                if (t != null) Rewind();
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

        private IIntValue AutoDelegate(Function f)
        {
            if (f == null) return null;

            var autoArgs = f.GetAutoArgs();
            if (autoArgs == null) return f;

            var args = new IIntValue[autoArgs.Length];
            for (int i = 0; i < args.Length; i++)
                args[i] = new Var(parent, autoArgs[i].Name);
            return new Delegate(parent, f.CallType, args, f)
            {
                SrcInfo = f.SrcInfo,
                Auto = true,
            };
        }

        private Function Function(string type, bool isStatic)
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
                Rewind();
                throw Abort("{0}: 名前が不適切です: {1}", type, name);
            }

            var ret = new Function(this.parent, name, isStatic);
            ret.CallType = ct;
            if (Peek() == "(") ReadArgs(type, ret);

            var comma = Read();
            if (comma == ":")
            {
                var ftype = Read();
                if (!Tokenizer.IsWord(ftype))
                {
                    Rewind();
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

        private Extern[] Extern()
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

            var list = new List<Extern>();
            for (; ; )
            {
                var si = SrcInfo;
                var name = Read();
                CallType ct2 = CheckCallType(ct1, ref name);
                var sfx2 = CheckSuffix(sfx1, ref name);
                if (!Tokenizer.IsWord(name))
                {
                    Rewind();
                    throw Abort("extern: 名前が不適切です: {0}", name);
                }
                string alias = null;
                if (sfx2 != null) alias = name + sfx2;

                var ex = new Extern(parent, name, module.Value, alias);
                ex.SrcInfo = si;
                ex.CallType = ct2;
                if (Peek() == "(") ReadArgs("extern", ex);

                var comma = Read();
                if (comma == ":")
                {
                    var ftype = Read();
                    if (!Tokenizer.IsWord(ftype))
                    {
                        Rewind();
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
            return list.ToArray();
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
                    if (arg != null) Rewind();
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
                        f.Args.Add(new ArgPtr(f, arg));
                        continue;
                    }
                    else if (!Tokenizer.IsWord(type))
                    {
                        if (type != null) Rewind();
                        throw Abort("{0}: {1}: {2}: 引数の型が不適切です。", tp, f.Name, arg);
                    }
                    var ar = Read();
                    if (ar == "[")
                    {
                        Check(tp, "]");
                        type += "[]";
                    }
                    else if (ar != null)
                        Rewind();
                }
                else if (colon != null)
                    Rewind();

                var argt = Types.ConvertVarType(Types.GetType(parent, type));
                f.Args.Add(new Arg(f, arg, argt));
            }
        }

        private Cond ReadCond(BlockBase parent, string type)
        {
            return ReadCond(parent, type, "(", ")");
        }

        private Cond ReadCond(BlockBase parent, string type, string start, string end)
        {
            Check(type, start);
            if (Read() == end) return null;
            Rewind();

            var p = this.parent;
            this.parent = parent;
            var ret = new Cond(parent, Expression());
            this.parent = p;

            Check(type, end);

            return ret;
        }

        private Block ReadSentenceBlock(BlockBase parent, string type)
        {
            return ReadSentenceBlock(parent, type, null, ";");
        }

        private Block ReadSentenceBlock(BlockBase parent, string type, BlockBase target, string separator)
        {
            Block ret = null;
            var p = this.parent;
            var si = SrcInfo;
            if (Peek() == "{")
            {
                this.parent = parent;
                var s = Sentence(separator);
                if (s != null && s.Length == 1) ret = s[0] as Block;
            }
            else
            {
                ret = new Block(parent);
                if (target == null) target = ret;
                this.parent = target;
                var s = Sentence(separator);
                if (s != null) ret.AddSentences(s);
            }
            if (ret == null)
                throw p.Abort(si, "{0}: ブロックが必要です。", type);
            this.parent = p;
            return ret;
        }

        private While While()
        {
            var ret = new While(parent);
            ret.Cond = ReadCond(ret, "while");
            ret.Block = ReadSentenceBlock(ret, "while");
            return ret;
        }

        private Do Do()
        {
            var ret = new Do(parent);
            ret.Block = ReadSentenceBlock(ret, "do");
            var t = Read();
            if (t != "while")
            {
                if (t != null) Rewind();
                throw Abort("do: while がありません。");
            }
            ret.Cond = ReadCond(ret, "do");
            return ret;
        }

        private If If()
        {
            var ret = new If(parent);
            If(ret);
            return ret;
        }

        private void If(If target)
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
                If(target);
                return;
            }

            Rewind();
            var cb2 = new If.CondBlock(target);
            cb2.Block = ReadSentenceBlock(target, "else");
            target.Blocks.Add(cb2);
        }

        private For For()
        {
            Check("for", "(");
            var ret = new For(parent);
            ret.Init = ReadSentenceBlock(ret, "for", ret, ";");
            ret.Cond = ReadCond(ret, "for", "", ";");
            if (Read() != ")")
            {
                Rewind();
                ret.Loop = ReadSentenceBlock(ret, "for", null, "");
                Check("for", ")");
            }
            ret.Block = ReadSentenceBlock(ret, "for");
            return ret;
        }

        private Switch Switch()
        {
            Check("switch", "(");

            var si = SrcInfo;
            var expr = Expression() as IIntValue;
            if (expr == null)
                throw parent.Abort(si, "switch: 値が必要です。");

            Check("switch", ")");
            Check("switch", "{");

            var ret = new Switch(parent, expr);
            Switch(ret);
            return ret;
        }

        private void Switch(Switch target)
        {
            var scb = new Switch.CaseBlock();
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
                            scb = new Switch.CaseBlock();
                        }
                        if (scb.Case == null)
                            scb.Case = new Switch.Case(target);
                        if (t == "case")
                        {
                            var v = Expression() as IIntValue;
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
                                scb.Block = new Block(target);
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
    }
}
