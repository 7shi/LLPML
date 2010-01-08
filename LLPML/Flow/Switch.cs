using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Switch : BlockBase
    {
        private class Expression : NodeBase
        {
            private IIntValue value;

            public Expression(BlockBase parent, IIntValue value)
                : base(parent)
            {
                this.value = value;
            }

            public Expression(BlockBase parent, XmlTextReader xr)
                : base(parent, xr)
            {
            }

            public override void Read(XmlTextReader xr)
            {
                Parse(xr, delegate
                {
                    IIntValue[] v = IntValue.Read(Parent, xr);
                    if (v != null)
                    {
                        if (v.Length > 1 || value != null)
                            throw Abort(xr, "multiple expressions");
                        else
                            value = v[0];
                    }
                });
                if (value == null)
                    throw Abort(xr, "expression required");
            }

            public override void AddCodes(OpModule codes)
            {
                value.AddCodes(codes, "mov", null);
                codes.Add(I386.Mov(Reg32.EDX, Reg32.EAX));
            }
        }

        public class Case : NodeBase
        {
            private List<IIntValue> values = new List<IIntValue>();
            public List<IIntValue> Values { get { return values; } }

            public Block Block;
            public bool IsLast;

            public Case(BlockBase parent) : base(parent) { }
            public Case(BlockBase parent, XmlTextReader xr) : base(parent, xr) { }

            public override void Read(XmlTextReader xr)
            {
                Parse(xr, delegate
                {
                    var vs = IntValue.Read(Parent, xr);
                    if (vs == null) return;
                    foreach (IIntValue v in vs)
                    {
                        if (!(v is IntValue))
                            throw Abort(xr, "constant required");
                        values.Add(v);
                    }
                });
                if (values.Count == 0)
                    throw Abort(xr, "value(s) required");
            }

            public override void AddCodes(OpModule codes)
            {
                int len = values.Count;
                if (len == 0)
                {
                    codes.Add(I386.Jmp(Block.First));
                    return;
                }

                foreach (var v in values)
                {
                    codes.Add(I386.Push(Reg32.EDX));
                    if (v.Type is TypeString)
                    {
                        v.AddCodes(codes, "push", null);
                        codes.AddRange(new[]
                        {
                            codes.GetCall("case", TypeString.Equal),
                            I386.Add(Reg32.ESP, 8),
                            I386.Test(Reg32.EAX, Reg32.EAX),
                            I386.Jcc(Cc.NZ, Block.First),
                        });
                    }
                    else
                    {
                        v.AddCodes(codes, "mov", null);
                        codes.AddRange(new[]
                        {
                            I386.Pop(Reg32.EDX),
                            I386.Cmp(Reg32.EDX, Reg32.EAX),
                            I386.Jcc(Cc.E, Block.First),
                        });
                    }
                }
                if (IsLast) codes.Add(I386.Jmp(Parent.Destruct));
            }
        }

        public class CaseBlock
        {
            public Case Case { get; set; }
            public Block Block { get; set; }
        }

        private Expression expr;

        private List<CaseBlock> blocks = new List<CaseBlock>();
        public List<CaseBlock> Blocks { get { return blocks; } }

        public override bool AcceptsBreak { get { return true; } }

        public Switch(BlockBase parent, IIntValue expr)
            : base(parent)
        {
            this.expr = new Expression(this, expr);
        }

        public Switch(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            base.Read(xr);
            CaseBlock cb = null;
            bool stop = false;
            Parse(xr, delegate
            {
                switch (xr.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xr.Name)
                        {
                            case "expr":
                                {
                                    expr = new Expression(this, xr);
                                    break;
                                }
                            case "case":
                                if (expr == null)
                                    throw Abort(xr, "expression required before case");
                                else if (stop)
                                    throw Abort(xr, "block terminated");
                                else if (cb != null)
                                    throw Abort(xr, "multiple cases");
                                cb = new CaseBlock { Case = new Case(this, xr) };
                                break;
                            case "block":
                                if (expr == null)
                                    throw Abort(xr, "expression required before block");
                                else if (stop)
                                    throw Abort(xr, "block terminated");
                                else if (cb == null)
                                {
                                    blocks.Add(new CaseBlock
                                    {
                                        Case = new Case(this),
                                        Block = new Block(this, xr)
                                    });
                                    stop = true;
                                }
                                else
                                {
                                    cb.Block = new Block(this, xr);
                                    blocks.Add(cb);
                                    cb = null;
                                }
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
            });
            if (expr == null)
                throw Abort(xr, "expression required");
            else if (cb != null)
                throw Abort(xr, "block required");
            else if (blocks.Count == 0)
                throw Abort(xr, "case and block required");
        }

        public override void AddCodes(OpModule codes)
        {
            sentences.Clear();
            AddSentence(expr);

            CaseBlock def = null;
            var list = new List<CaseBlock>();
            foreach (var cb in blocks)
            {
                if (cb.Case.Values.Count == 0)
                {
                    if (def != null)
                        throw cb.Case.Abort("switch: multiple defaults");
                    else
                        def = cb;
                }
                else
                    list.Add(cb);
            }
            if (def != null) list.Add(def);

            int len = list.Count;
            for (int i = 0; i < len; i++)
            {
                var cb = list[i];
                cb.Case.Block = cb.Block;
                cb.Case.IsLast = i == len - 1;
                AddSentence(cb.Case);
            }
            foreach (var cb in list)
                AddSentence(cb.Block);
            base.AddCodes(codes);
        }
    }
}
