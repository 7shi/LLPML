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

        public Call() { }
        public Call(Block parent, XmlTextReader xr) : base(parent, xr) { }

        public override void Read(XmlTextReader xr)
        {
            target = xr["target"];
            args = new List<object>();
            Parse(xr, delegate
            {
                if (xr.NodeType == XmlNodeType.Element)
                {
                    object arg = null;
                    string name = xr["name"];
                    switch (xr.Name)
                    {
                        case "int":
                            arg = parent.ReadInt(xr);
                            break;
                        case "string":
                            arg = parent.ReadString(xr);
                            break;
                        case "var-int":
                            arg = parent.ReadVarInt(xr);
                            break;
                        case "ptr":
                            arg = parent.ReadPointer(xr);
                            break;
                        default:
                            throw Abort(xr);
                    }
                    if (arg == null)
                    {
                        string msg = "invalid argument";
                        if (name != null) msg += ": " + name;
                        throw Abort(xr, msg);
                    }
                    args.Add(arg);
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
                else if (arg is string)
                {
                    codes.Add(I386.Push(m.GetString(arg as string)));
                }
                else if (arg is VarInt)
                {
                    (arg as VarInt).WriteArg(codes, parent.Level);
                }
                else if (arg is Pointer)
                {
                    Pointer p = arg as Pointer;
                    int lv = p.Parent.Level;
                    if (p.Parent == parent || p.Address.IsAddress)
                    {
                        codes.Add(I386.Lea(Reg32.EAX, p.Address));
                        codes.Add(I386.Push(Reg32.EAX));
                    }
                    else if (0 < lv && lv < parent.Level)
                    {
                        codes.Add(I386.Mov(Reg32.EAX, new Addr32(Reg32.EBP, -lv * 4)));
                        codes.Add(I386.Sub(Reg32.EAX, (uint)-p.Address.Disp));
                        codes.Add(I386.Push(Reg32.EAX));
                    }
                    else
                    {
                        throw new Exception("Invalid variable scope.");
                    }
                }
                else
                {
                    throw new Exception("Unknown argument.");
                }
            }
            Function f = parent.GetFunction(target);
            if (f == null) throw new Exception("undefined function: " + target);
            codes.Add(I386.Call(f.Address));
            if (f.Type == CallType.CDecl && args.Length > 0)
            {
                codes.Add(I386.Add(Reg32.ESP, (byte)(args.Length * 4)));
            }
        }
    }
}
