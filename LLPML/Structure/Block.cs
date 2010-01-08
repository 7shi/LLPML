using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;
using Girl.LLPML.Parsing;

namespace Girl.LLPML
{
    public class Block : BlockBase
    {
        public Block() { }
        public Block(BlockBase parent) : base(parent) { }
        public Block(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        public BlockBase Target { get; set; }

        protected virtual void ReadBlock(XmlTextReader xr)
        {
            var target = Target;
            if (target == null) target = this;
            ReadBlock(target, xr);
        }

        protected void ReadBlock(BlockBase target, XmlTextReader xr)
        {
            NodeBase nb = null;
            switch (xr.NodeType)
            {
                case XmlNodeType.Element:
                    switch (xr.Name)
                    {
                        case "int-declare":
                            target.ReadIntDefine(xr);
                            break;
                        case "string-declare":
                            target.ReadStringDefine(xr);
                            break;
                        case "function":
                            new Function(target, xr);
                            break;
                        case "extern":
                            new Extern(target, xr);
                            break;
                        case "return":
                            nb = new Return(target, xr);
                            return;
                        case "block":
                            nb = new Block(target, xr);
                            break;
                        case "call":
                            nb = new Call(target, xr);
                            break;
                        case "if":
                            nb = new If(target, xr);
                            break;
                        case "switch":
                            nb = new Switch(target, xr);
                            break;
                        case "for":
                            nb = new For(target, xr);
                            break;
                        case "do":
                            nb = new Do(target, xr);
                            break;
                        case "while":
                            nb = new While(target, xr);
                            break;
                        case "break":
                            nb = new Break(target, xr);
                            break;
                        case "continue":
                            nb = new Continue(target, xr);
                            break;
                        case "set":
                            nb = new Set(target, xr);
                            break;
                        case "var-declare":
                            nb = new Var.Declare(target, xr);
                            break;
                        case "expression":
                            nb = new Expression(target, xr);
                            break;
                        case "inc":
                            nb = new Inc(target, xr);
                            break;
                        case "dec":
                            nb = new Dec(target, xr);
                            break;
                        case "post-inc":
                            nb = new PostInc(target, xr);
                            break;
                        case "post-dec":
                            nb = new PostDec(target, xr);
                            break;
                        case "var-add":
                            nb = new Var.Add(target, xr);
                            break;
                        case "var-sub":
                            nb = new Var.Sub(target, xr);
                            break;
                        case "var-mul":
                            nb = new Var.Mul(target, xr);
                            break;
                        case "var-div":
                            nb = new Var.Div(target, xr);
                            break;
                        case "var-and":
                            nb = new Var.And(target, xr);
                            break;
                        case "var-or":
                            nb = new Var.Or(target, xr);
                            break;
                        case "var-shift-left":
                            nb = new Var.ShiftLeft(target, xr);
                            break;
                        case "var-shift-right":
                            nb = new Var.ShiftRight(target, xr);
                            break;
                        case "ptr-declare":
                            nb = new Var.Declare(target, xr);
                            break;
                        case "struct-define":
                            new Struct.Define(target, xr);
                            break;
                        case "class-define":
                            new Struct.Define(target, xr) { IsClass = true };
                            break;
                        case "struct-declare":
                            nb = new Struct.Declare(target, xr);
                            break;
                        case "new":
                            nb = new Struct.New(target, xr);
                            break;
                        default:
                            throw Abort(xr);
                    }
                    break;

                case XmlNodeType.Whitespace:
                case XmlNodeType.Comment:
                    break;

                case XmlNodeType.Text:
                    {
                        NodeBase[] sents = ReadText(target,
                            new Tokenizer(target.Root.Source, xr));
                        if (sents != null) AddSentences(sents);
                        break;
                    }

                case XmlNodeType.ProcessingInstruction:
                    if (xr.Name == "llp")
                    {
                        char[] data = new char[1024];
                        var rs = xr.ReadString();
                        var t = new Tokenizer(target.Root.Source,
                            xr.Value, xr.LineNumber + 1, 1);
                        var sents = Block.ReadText(target, t);
                        if (sents != null) AddSentences(sents);
                        break;
                    }
                    else
                        throw Abort(xr, "unknow instruction: " + xr.Name);

                default:
                    throw Abort(xr, "element required");
            }
            if (nb != null) AddSentence(nb);
        }

        public static NodeBase[] ReadText(BlockBase parent, Tokenizer token)
        {
            Parser parser = new Parser(token, parent);
            NodeBase[] ret = parser.Parse();
            if (token.CanRead) ret = null;
            return ret;
        }

        public override void Read(XmlTextReader xr)
        {
            base.Read(xr);
            Parse(xr, delegate
            {
                ReadBlock(xr);
            });
        }
    }
}
