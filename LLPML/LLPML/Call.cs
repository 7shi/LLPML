using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Call : NodeBase
    {
        public List<object> args;

        private string target;
        public string Target { get { return target; } }

        private Extern function;
        public Extern Function { get { return function; } }

        public Call() { }
        public Call(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            target = xr["target"];
            function = root.GetFunction(target);
            args = new List<object>();
            Parse(xr, delegate
            {
                if (xr.NodeType == XmlNodeType.Element)
                {
                    switch (xr.Name)
                    {
                        case "int":
                            args.Add(parent.ReadInt(xr));
                            break;
                        case "string":
                            args.Add(parent.ReadString(xr));
                            break;
                        case "var-int":
                            args.Add(parent.ReadVarInt(xr));
                            break;
                        case "ptr":
                            args.Add(parent.ReadPointer(xr));
                            break;
                        default:
                            throw Abort(xr);
                    }
                }
            });
        }

        public override void AddCodes(List<OpCode> codes, Module m)
        {
            object[] args = this.args.ToArray();
            Array.Reverse(args);
            foreach (object arg in args)
            {
                if (arg is int)
                {
                    codes.Add(I386.Push((uint)(int)arg));
                }
                else if (arg is uint)
                {
                    codes.Add(I386.Push((uint)arg));
                }
                else if (arg is Ptr<uint>)
                {
                    codes.Add(I386.Push((Ptr<uint>)arg));
                }
                else if (arg is Addr32)
                {
                    codes.Add(I386.Push((Addr32)arg));
                }
                else if (arg is string)
                {
                    codes.Add(I386.Push(m.GetString(arg as string)));
                }
                else if (arg is Block.LocalVarInt)
                {
                    Block.LocalVarInt v = arg as Block.LocalVarInt;
                    int lv = v.scope.Level;
                    if (v.scope == parent || v.addr.IsAddress)
                    {
                        codes.Add(I386.Push(v.addr));
                    }
                    else if (0 < lv && lv < parent.Level)
                    {
                        codes.Add(I386.Mov(Reg32.EAX, new Addr32(Reg32.EBP, - lv * 4)));
                        codes.Add(I386.Push(new Addr32(Reg32.EAX, v.addr.Disp)));
                    }
                    else
                    {
                        throw new Exception("Invalid variable scope.");
                    }
                }
                else if (arg is Block.LocalPointer)
                {
                    Block.LocalPointer p = arg as Block.LocalPointer;
                    int lv = p.scope.Level;
                    if (p.ptr != null)
                    {
                        codes.Add(I386.Push(p.ptr));
                    }
                    else
                    {
                        if (p.scope == parent || p.addr.IsAddress)
                        {
                            codes.Add(I386.Lea(Reg32.EAX, p.addr));
                            codes.Add(I386.Push(Reg32.EAX));
                        }
                        else if (0 < lv && lv < parent.Level)
                        {
                            codes.Add(I386.Mov(Reg32.EAX, new Addr32(Reg32.EBP, - lv * 4)));
                            codes.Add(I386.Sub(Reg32.EAX, (uint)-p.addr.Disp));
                            codes.Add(I386.Push(Reg32.EAX));
                        }
                        else
                        {
                            throw new Exception("Invalid variable scope.");
                        }
                    }
                }
                else
                {
                    throw new Exception("Unknown argument.");
                }
            }
            Function f = function.GetFunction(m);
            codes.Add(I386.Call(f.Address));
            if (f.CallType == CallType.CDecl)
            {
                codes.Add(I386.Add(Reg32.ESP, (byte)(args.Length * 4)));
            }
        }
    }
}
