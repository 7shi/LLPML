using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Block : BlockBase
    {
        public Block() { }
        public Block(BlockBase parent) : base(parent) { }
        public Block(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

        protected virtual void ReadBlock(XmlTextReader xr)
        {
            switch (xr.NodeType)
            {
                case XmlNodeType.Element:
                    switch (xr.Name)
                    {
                        case "int-declare":
                            ReadIntDefine(xr);
                            break;
                        case "string-declare":
                            ReadStringDefine(xr);
                            break;
                        case "function":
                            new Function(this, xr);
                            break;
                        case "extern":
                            new Extern(this, xr);
                            break;
                        case "return":
                            sentences.Add(new Return(this, xr));
                            return;
                        case "block":
                            sentences.Add(new Block(this, xr));
                            break;
                        case "call":
                            sentences.Add(new Call(this, xr));
                            break;
                        case "invoke":
                            sentences.Add(new Struct.Invoke(this, xr));
                            break;
                        case "if":
                            sentences.Add(new If(this, xr));
                            break;
                        case "switch":
                            sentences.Add(new Switch(this, xr));
                            break;
                        case "for":
                            sentences.Add(new For(this, xr));
                            break;
                        case "do":
                            sentences.Add(new Do(this, xr));
                            break;
                        case "while":
                            sentences.Add(new While(this, xr));
                            break;
                        case "break":
                            sentences.Add(new Break(this, xr));
                            break;
                        case "continue":
                            sentences.Add(new Continue(this, xr));
                            break;
                        case "let":
                            sentences.Add(new Let(this, xr));
                            break;
                        case "var-declare":
                            sentences.Add(new Var.Declare(this, xr));
                            break;
                        case "inc":
                            sentences.Add(new Inc(this, xr));
                            break;
                        case "dec":
                            sentences.Add(new Dec(this, xr));
                            break;
                        case "post-inc":
                            sentences.Add(new PostInc(this, xr));
                            break;
                        case "post-dec":
                            sentences.Add(new PostDec(this, xr));
                            break;
                        case "var-add":
                            sentences.Add(new Var.Add(this, xr));
                            break;
                        case "var-sub":
                            sentences.Add(new Var.Sub(this, xr));
                            break;
                        case "var-mul":
                            sentences.Add(new Var.Mul(this, xr));
                            break;
                        case "var-unsigned-mul":
                            sentences.Add(new Var.UnsignedMul(this, xr));
                            break;
                        case "var-div":
                            sentences.Add(new Var.Div(this, xr));
                            break;
                        case "var-unsigned-div":
                            sentences.Add(new Var.UnsignedDiv(this, xr));
                            break;
                        case "var-and":
                            sentences.Add(new Var.And(this, xr));
                            break;
                        case "var-or":
                            sentences.Add(new Var.Or(this, xr));
                            break;
                        case "var-shift-left":
                            sentences.Add(new Var.ShiftLeft(this, xr));
                            break;
                        case "var-shift-right":
                            sentences.Add(new Var.ShiftRight(this, xr));
                            break;
                        case "var-unsigned-shift-left":
                            sentences.Add(new Var.UnsignedShiftLeft(this, xr));
                            break;
                        case "var-unsigned-shift-right":
                            sentences.Add(new Var.UnsignedShiftRight(this, xr));
                            break;
                        case "ptr-declare":
                            sentences.Add(new Pointer.Declare(this, xr));
                            break;
                        case "struct-define":
                            new Struct.Define(this, xr);
                            break;
                        case "struct-declare":
                            sentences.Add(new Struct.Declare(this, xr));
                            break;
                        default:
                            throw Abort(xr);
                    }
                    break;

                case XmlNodeType.Whitespace:
                case XmlNodeType.Comment:
                    break;

                default:
                    throw Abort(xr, "element required");
            }
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
