using System;
using System.Collections.Generic;
using System.Text;
using Girl.LLPML.Struct;
using Girl.X86;

namespace Girl.LLPML.Parsing
{
    public partial class Parser
    {
        // 予約語
        private NodeBase Reserved()
        {
            var si = SrcInfo;
            var t = Read();
            var ret = ReservedInternal(si, t);
            if (ret != null)
                ret.SrcInfo = si;
            else if (t != null)
                Rewind();
            return ret;
        }

        private NodeBase ReservedInternal(SrcInfo si, string t)
        {
            string t2;
            NodeBase arg;
            switch (t)
            {
                case "null":
                    return Null.New(parent);
                case "true":
                    return Cast.New(parent, "bool", IntValue.One);
                case "false":
                    return Cast.New(parent, "bool", IntValue.Zero);
                case "base":
                    return Base.New(parent);
                case "new":
                    return ReadNew();
                case "function":
                    return AutoDelegate(ReadFunction(t, false));
                case "delegate":
                    return ReadDelegate();
                case "\\":
                    return AutoDelegate(Lambda());
                case "sizeof":
                    t2 = Read();
                    if (t2 != "(" && t2 != null) Rewind();
                    arg = ReadExpression();
                    if (arg == null)
                        throw parent.AbortInfo(si, "sizeof: 引数が必要です。");
                    if (t2 == "(") Check("sizeof", ")");
                    return SizeOf.New(parent, arg);
                case "addrof":
                    return AddrOf.New(parent, ReadExpression());
                case "typeof":
                    t2 = Read();
                    if (t2 != "(" && t2 != null) Rewind();
                    arg = ReadExpression();
                    if (arg == null)
                        throw parent.AbortInfo(si, "typeof: 引数が必要です。");
                    if (t2 == "(") Check("typeof", ")");
                    if (arg is TypeOf) return arg;
                    return TypeOf.New(parent, arg);
                case "__FUNCTION__":
                    return StringValue.New(parent.FullName);
                case "__FILE__":
                    return StringValue.New(si.Source);
                case "__LINE__":
                    return IntValue.New(si.Number);
                case "__VERSION__":
                    return StringValue.New("LLPML ver." + Root.VERSION);
                default:
                    return null;
            }
        }

        private NodeBase ReadDelegate()
        {
            Check("delegate", "(");
            var ct = CallType.CDecl;
            if (Read() == "(")
            {
                var t = Read();
                if (t == "stdcall")
                    ct = CallType.Std;
                else
                    throw Abort("delegate: 不明な属性です: {0}", t);
                Check("delegate", ")");
                Check("delegate", ")");
                Check("delegate", "(");
            }
            else
                Rewind();
            var args = Arguments(",", ")", false);
            if (args == null)
                throw Abort("delegate: 引数が不完全です。");
            return DelgFunc.NewCurry(parent, ct, args);
        }

        private Function Lambda()
        {
            var t = Peek();
            if (t == null) return null;

            var type = "ラムダ式";
            var ret = Function.New(this.parent, "", false);
            if (t == "(")
                ReadArgs(type, ret);
            else if (t != "=>")
            {
                Read();
                ret.Args.Add(Arg.New(ret, t, TypeVar.Instance));
            }
            Check(type, "=>");

            if (Peek() == "{")
                ReadBlock(type, ret);
            else
            {
                var parent = this.parent;
                this.parent = ret;
                var ex = ReadExpression();
                ret.AddSentence(Return.New(ret, ex));
                this.parent = parent;
            }

            if (!ret.Parent.AddFunction(ret))
                throw Abort("{0}: {1}: 定義が重複しています。", type, ret.Name);
            return ret;
        }

        private New ReadNew()
        {
            var type = Read();
            if (type == null)
                throw Abort("new: 型が必要です。");
            else if (!Tokenizer.IsWord(type))
                throw Abort("new: 型が不適切です: {0}", type);
            var br = Read();
            if (br == "(")
                Check("new", ")");
            else if (br == "[")
            {
                var len = ReadExpression();
                Check("new", "]");
                return New.New2(parent, type, len);
            }
            else if (br != null)
                Rewind();
            if (Peek() != "{") return New.New1(parent, type);

            // 無名クラス
            var anon = Define.New(parent, "", type);
            anon.IsClass = true;
            ReadBlock("anonymous class", anon);
            parent.AddStruct(anon);
            return New.New1(parent, anon.Name);
        }
    }
}
