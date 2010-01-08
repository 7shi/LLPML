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
            if (!CanRead) throw Abort("��������܂���B");

            var si = tokenizer.SrcInfo;
            var t = Read();
            NodeBase nb = null;
            switch (t)
            {
                case "{":
                    Rewind();
                    nb = new Block(parent);
                    ReadBlock(nb as Block, "block");
                    break;
                case "struct":
                    StructDefine().SrcInfo = si;
                    return null;
                case "function":
                    Function().SrcInfo = si;
                    return null;
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
                throw Abort("{0} ���K�v�ł��B", separator);
            }
            if (list.Count == 0) return null;
            return list.ToArray();
        }

        private NodeBase[] SentenceBody()
        {
            if (!CanRead) throw Abort("�����K�v�ł��B");

            var t = Read();
            if (t == "const")
            {
                ConstDeclare();
                return null;
            }
            else
            {
                var si = tokenizer.SrcInfo;
                var nb = CheckReserved(t);
                if (nb != null)
                {
                    nb.SrcInfo = si;
                    return new NodeBase[] { nb };
                }
            }

            Rewind();
            if (t == ";") return null;

            var dec = Declare();
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
                    return new Return(parent, Expression());
                case "break":
                    {
                        var brk = new Break(parent);
                        if (!brk.CanBreak())
                            throw Abort("break: �����ł͎g�p�ł��܂���B");
                        return brk;
                    }
                case "continue":
                    {
                        var con = new Continue(parent);
                        if (!con.CanContinue())
                            throw Abort("continue: �����ł͎g�p�ł��܂���B");
                        return con;
                    }
            }
            return null;
        }

        private Struct.Define StructDefine()
        {
            if (!CanRead) throw Abort("struct: ���O���K�v�ł��B");

            var name = Read();
            if (!Tokenizer.IsWord(name))
            {
                Rewind();
                throw Abort("struct: ���O���s�K�؂ł�: {0}", name);
            }

            string baseType = null;
            var t = Read();
            if (t == ":")
            {
                if (!CanRead)
                    throw Abort("struct: {0}: �^���K�v�ł��B", name);
                baseType = Read();
                if (!Tokenizer.IsWord(baseType))
                {
                    Rewind();
                    throw Abort("struct: {0}: �^���K�v�ł��B", name);
                }
                if (name == baseType)
                    throw Abort("struct: {0}: �p���ł��܂���: {1}", name, baseType);
            }
            else
                Rewind();

            var ret = new Struct.Define(parent, name, baseType);
            ReadBlock(ret, "struct");
            if (!parent.AddStruct(ret))
                throw Abort("struct: {0}: ��`���d�����Ă��܂��B", name);
            return ret;
        }

        private void Check(string type, string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            var t = Read();
            if (t != text)
            {
                if (t != null) Rewind();
                throw Abort("{0}: {1} ���K�v�ł��B", type, text);
            }
        }

        private void ReadBlock(BlockBase block, string type)
        {
            Check(type, "{");

            var parent = this.parent;
            this.parent = block;

            for (; ; )
            {
                var t = Read();
                if (t == null)
                    throw Abort("{0}: {1}: }}}} ���K�v�ł��B", type, block.Name);
                if (t == "}") break;

                Rewind();
                var s = Sentence();
                if (s != null) block.Sentences.AddRange(s);
            }

            this.parent = parent;
        }

        private Function Function()
        {
            if (!CanRead) throw Abort("function: ���O���K�v�ł��B");

            var name = Read();
            CallType ct = CheckCallType(CallType.CDecl, ref name);
            if (!Tokenizer.IsWord(name))
            {
                Rewind();
                throw Abort("function: ���O���s�K�؂ł�: {0}", name);
            }

            var ret = new Function(parent, name);
            ret.CallType = ct;
            ReadArgs("function", ret);
            ReadBlock(ret, "function");
            if (!parent.AddFunction(ret))
                throw Abort("function: {0}: ��`���d�����Ă��܂��B", name);
            return ret;
        }

        private Extern[] Extern()
        {
            if (!CanRead) throw Abort("extern: ���O���K�v�ł��B");

            var module = String();
            if (module == null) throw Abort("extern: ���W���[�������K�v�ł��B");

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
                var si = tokenizer.SrcInfo;
                var name = Read();
                CallType ct2 = CheckCallType(ct1, ref name);
                var sfx2 = CheckSuffix(sfx1, ref name);
                if (!Tokenizer.IsWord(name))
                {
                    Rewind();
                    throw Abort("extern: ���O���s�K�؂ł�: {0}", name);
                }
                string alias = null;
                if (sfx2 != null) alias = name + sfx2;

                var ex = new Extern(parent, name, module.Value, alias);
                ex.SrcInfo = si;
                ex.CallType = ct2;
                ReadArgs("extern", ex);
                if (!parent.AddFunction(ex))
                    throw Abort("function: {0}: ��`���d�����Ă��܂��B", name);
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
                    throw Abort("{0}: {1}: ) ���K�v�ł��B", tp, f.Name);
                if (t == ")") break;
                if (first)
                    Rewind();
                else if (t != ",")
                    throw Abort("{0}: {1}: ) ���K�v�ł��B", tp, f.Name);
                first = false;

                var arg = Read();
                if (!Tokenizer.IsWord(arg))
                {
                    if (arg != null) Rewind();
                    throw Abort("{0}: {1}: �����̖��O���s�K�؂ł�: {1}", tp, f.Name, arg);
                }

                string type = null;
                var colon = Read();
                if (colon == ":")
                {
                    if (!CanRead)
                        throw Abort("{0}: {1}: {2}: �����Ɍ^���K�v�ł��B", tp, f.Name, arg);
                    type = Read();
                    if (type == "params")
                    {
                        f.Args.Add(new ArgPtr(f, arg));
                        continue;
                    }
                    else if (!Tokenizer.IsWord(type))
                    {
                        if (type != null) Rewind();
                        throw Abort("{0}: {1}: {2}: �����̌^���s�K�؂ł��B", tp, f.Name, arg);
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

                f.Args.Add(new Arg(f, arg, type));
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
            var si = tokenizer.SrcInfo;
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
                if (s != null) ret.Sentences.AddRange(s);
            }
            if (ret == null)
                throw p.Abort(si, "{0}: �u���b�N���K�v�ł��B", type);
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
                throw Abort("do: while ������܂���B");
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

            var si = tokenizer.SrcInfo;
            var expr = Expression() as IIntValue;
            if (expr == null)
                throw parent.Abort(si, "switch: �l���K�v�ł��B");

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
                        throw Abort("switch: }}}} ���K�v�ł��B");
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
                                throw Abort("case: �l���K�v�ł��B");
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
                                throw Abort("switch: �������K�v�ł��B");
                            if (scb.Block == null)
                                scb.Block = new Block(target);
                            var p = parent;
                            parent = target;
                            var s = Sentence();
                            if (s != null) scb.Block.Sentences.AddRange(s);
                            parent = p;
                            break;
                        }
                }
            }
        }
    }
}
