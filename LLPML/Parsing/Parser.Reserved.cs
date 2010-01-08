using System;
using System.Collections.Generic;
using System.Text;
using Girl.X86;

namespace Girl.LLPML.Parsing
{
    public partial class Parser
    {
        // 予約語
        private IIntValue Reserved()
        {
            var si = SrcInfo;
            var t = Read();
            switch (t)
            {
                case "null":
                    return new Null(parent) { SrcInfo = si };
                case "true":
                    return new Cast(parent, "bool", new IntValue(1)) { SrcInfo = si };
                case "false":
                    return new Cast(parent, "bool", new IntValue(0)) { SrcInfo = si };
                case "function":
                    {
                        var f = Function(t);
                        if (f == null) break;
                        f.SrcInfo = si;
                        return AutoDelegate(f);
                    }
                case "delegate":
                    {
                        Check("delegate", "(");
                        CallType ct = CallType.CDecl;
                        if (Read() == "(")
                        {
                            var cts = Read();
                            if (cts == "stdcall")
                                ct = CallType.Std;
                            else
                                throw Abort("delegate: 不明な属性です: {0}", cts);
                            Check("delegate", ")");
                            Check("delegate", ")");
                            Check("delegate", "(");
                        }
                        else
                            Rewind();
                        var args = Arguments(",", ")", false);
                        if (args == null)
                            throw Abort("delegate: 引数が不完全です。");
                        return new Delegate(parent, ct, args) { SrcInfo = si };
                    }
                case "\\":
                    {
                        var lambda = Lambda();
                        if (lambda == null) break;
                        lambda.SrcInfo = si;
                        return AutoDelegate(lambda);
                    }
                case "sizeof":
                    {
                        var br = Read();
                        if (br != "(")
                            if (br != null) Rewind();
                        var arg = Read();
                        if (arg == null)
                            throw parent.Abort(si, "sizeof: 引数が必要です。");
                        if (br == "(") Check("sizeof", ")");
                        return new SizeOf(parent, arg) { SrcInfo = si };
                    }
                case "addrof":
                    {
                        var ex = Expression() as Var;
                        if (ex == null)
                            throw parent.Abort(si, "addrof: 引数が不適切です。");
                        return new AddrOf(parent, ex) { SrcInfo = si };
                    }
                case "typeof":
                    return new TypeOf(parent, Expression()) { SrcInfo = si };
                case "__FUNCTION__":
                    return new StringValue(parent.GetName());
                case "__FILE__":
                    return new StringValue(si.Source);
                case "__LINE__":
                    return new IntValue(si.Number);
                case "__LLPML__":
                    return new StringValue("LLPML ver." + Root.LLPMLVersion);
            }
            if (t != null) Rewind();
            return null;
        }

        private Function Lambda()
        {
            var t = Peek();
            if (t == null) return null;

            var type = "ラムダ式";
            var ret = new Function(this.parent);
            if (t == "(")
                ReadArgs(type, ret);
            else if (t != "=>")
            {
                Read();
                ret.Args.Add(new Arg(ret, t, TypeVar.Instance));
            }
            Check(type, "=>");

            if (Peek() == "{")
                ReadBlock(type, ret);
            else
            {
                var parent = this.parent;
                this.parent = ret;
                var ex = Expression();
                this.parent = parent;
                ret.Sentences.Add(new Return(ret, ex));
            }

            if (!ret.Parent.AddFunction(ret))
                throw Abort("{0}: {1}: 定義が重複しています。", type, ret.Name);
            return ret;
        }
    }
}
