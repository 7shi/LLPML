using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Parser
    {
        private string output = "output.exe";
        public string Output { get { return output; } }

        private Module module = new Module();
        public Module Module { get { return module; } }

        private List<OpCode> codes = new List<OpCode>();
        private XmlTextReader xml;
        private Dictionary<string, Function> functions = new Dictionary<string, Function>();
        private Dictionary<string, Addr32> ints = new Dictionary<string, Addr32>();
        private Dictionary<string, string> strs = new Dictionary<string, string>();
        private Dictionary<string, Ref<uint>> ptrs = new Dictionary<string, Ref<uint>>();

        public Parser(string src)
        {
            StringReader sr = new StringReader(src);
            xml = new XmlTextReader(sr);
            while (xml.Read())
            {
                if (xml.Name == "llpml" && xml.NodeType == XmlNodeType.Element)
                {
                    llpml();
                }
            }
            xml.Close();
            sr.Close();

            Function ExitProcess = module.GetFunction(CallType.Std, "kernel32.dll", "ExitProcess");
            codes.AddRange(ExitProcess.Invoke(0));
            module.Text.OpCodes = codes.ToArray();
        }

        public delegate void VoidDelegate();
        private void Parse(VoidDelegate delg)
        {
            string self = xml.Name;
            bool empty = xml.IsEmptyElement;
            while (!empty && xml.Read())
            {
                if (xml.Name == self && xml.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
                delg();
            }
        }

        private void ParseSentence()
        {
            if (xml.NodeType == XmlNodeType.Element)
            {
                switch (xml.Name)
                {
                    case "extern":
                        @extern();
                        break;
                    case "call":
                        call();
                        break;
                    case "var-int":
                        var_int();
                        break;
                    case "string":
                        @string();
                        break;
                    case "ptr":
                        ptr();
                        break;
                    case "loop":
                        loop();
                        break;
                    default:
                        throw new Exception(string.Format("[{0}:{1}] invalid element: {2}",
                            xml.LineNumber, xml.LinePosition, xml.Name));
                }
            }
            else if (xml.NodeType != XmlNodeType.Whitespace)
            {
                throw new Exception(string.Format("[{0}:{1}] element required",
                    xml.LineNumber, xml.LinePosition));
            }
        }

        private void llpml()
        {
            string output = xml["output"];
            if (output != null) this.output = output;
            if (xml["subsystem"] == "WINDOWS_GUI")
            {
                module.Specific.SubSystem = IMAGE_SUBSYSTEM.WINDOWS_GUI;
            }
            Parse(ParseSentence);
        }

        private void loop()
        {
            int c = 0;
            string count = xml["count"];
            if (count != null)
            {
                c = int.Parse(xml["count"]);
                codes.Add(I386.Push((uint)c));
            }
            OpCode start = new OpCode();
            codes.Add(start);
            Parse(ParseSentence);
            if (count != null)
            {
                codes.Add(I386.Dec(new Addr32(Reg32.ESP)));
                codes.Add(I386.Jnz(start.Address));
                codes.Add(I386.Add(Reg32.ESP, 4));
            }
            else
            {
                codes.Add(I386.Jmp(start.Address));
            }
        }

        private void call()
        {
            Function target = functions[xml["target"]];
            List<object> args = new List<object>();
            Parse(delegate
            {
                if (xml.NodeType == XmlNodeType.Element)
                {
                    switch (xml.Name)
                    {
                        case "int":
                            args.Add(@int());
                            break;
                        case "string":
                            args.Add(@string());
                            break;
                        case "var-int":
                            args.Add(var_int());
                            break;
                        case "ptr":
                            args.Add(ptr());
                            break;
                        default:
                            throw new Exception(string.Format("[{0}:{1}] invalid element: {2}",
                                xml.LineNumber, xml.LinePosition, xml.Name));
                    }
                }
            });
            codes.AddRange(target.Invoke(args.ToArray()));
        }

        private Addr32 var_int()
        {
            Addr32 ret = null;
            string name = xml["name"];
            if (ints.ContainsKey(name))
            {
                ret = ints[name];
            }
            else
            {
                ints[name] = ret = new Addr32(module.GetInt32(name));
            }
            int? v = null;
            bool let = false;
            Parse(delegate
            {
                if (xml.NodeType == XmlNodeType.Text)
                {
                    v = int.Parse(xml.Value);
                    let = true;
                }
                else if (xml.NodeType == XmlNodeType.Element)
                {
                    switch (xml.Name)
                    {
                        case "call":
                            call();
                            let = true;
                            break;
                        default:
                            throw new Exception(string.Format("[{0}:{1}] invalid element: {2}",
                                xml.LineNumber, xml.LinePosition, xml.Name));
                    }
                }
            });
            if (let)
            {
                if (v != null) codes.Add(I386.Mov(Reg32.EAX, (uint)v));
                codes.Add(I386.Mov(ret, Reg32.EAX));
            }
            return ret;
        }

        private int @int()
        {
            int ret = 0;
            string len = xml["len"];
            if (len != null) ret = strs[len].Length;
            Parse(delegate
            {
                if (xml.NodeType == XmlNodeType.Text)
                {
                    ret = int.Parse(xml.Value);
                }
            });
            return ret;
        }

        private string @string()
        {
            string ret = null;
            string name = xml["name"];
            Parse(delegate
            {
                if (xml.NodeType == XmlNodeType.Text)
                {
                    ret = xml.Value;
                }
            });
            if (name != null)
            {
                if (ret == null)
                {
                    ret = strs[name];
                }
                else
                {
                    strs[name] = ret;
                }
            }
            return ret;
        }

        private Ref<uint> ptr()
        {
            Ref<uint> ret = null;
            string name = xml["name"];
            Parse(delegate
            {
                if (xml.NodeType == XmlNodeType.Element)
                {
                    int c = 1;
                    string count = xml["count"];
                    if (count != null) c = int.Parse(count);
                    switch (xml.Name)
                    {
                        case "int":
                            ret = module.GetInt32(name);
                            break;
                        case "byte":
                            ret = module.GetBuffer(name, c);
                            break;
                        default:
                            throw new Exception(string.Format("[{0}:{1}] invalid element: {2}",
                                xml.LineNumber, xml.LinePosition, xml.Name));
                    }
                }
            });
            if (name != null)
            {
                if (ret == null)
                {
                    ret = ptrs[name];
                }
                else
                {
                    ptrs[name] = ret;
                }
            }
            return ret;
        }

        private void @extern()
        {
            CallType type = CallType.CDecl;
            if (xml["type"] == "std") type = CallType.Std;

            string name = xml["name"];
            string alias = xml["alias"];
            if (alias == null) alias = name;
            functions[alias] = module.GetFunction(type, xml["module"], name);
        }
    }
}
