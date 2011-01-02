using System;
using System.Collections.Generic;
using System.Text;

namespace Girl.LLPML.Parsing
{
    public partial class Parser
    {
        private NodeBase Call(NodeBase target, int order)
        {
            var fn = "call";
            if (target is NodeBase)
            {
                var nb = target as NodeBase;
                fn = nb.Name;
            }
            var args = Arguments(",", ")", false);
            if (args == null)
                throw Abort("{0}: 引数が不完全です。", fn);
            if (target is Variant)
                return new Call(parent, fn, null, args);
            if (target is Struct.Member)
            {
                var mem = target as Struct.Member;
                return new Call(parent, "", mem, args);
            }
            return new Call(parent, target, null, args);
        }

        private NodeBase Member(NodeBase target, int order)
        {
            Struct.Member mem = target as Struct.Member;
            var t2 = Read();
            if (!Tokenizer.IsWord(t2))
            {
                //Rewind();
                throw Abort("名前が不適切です: {0}", t2);
            }
            var t2m = new Struct.Member(parent, t2);
            if (mem != null)
            {
                mem.Append(t2m);
                return mem;
            }
            if (target is Variant)
                t2m.TargetType = (target as Variant).Name;
            else
                t2m.Target = target;
            return t2m;
        }

        private NodeBase Index(NodeBase target, int order)
        {
            var t = Read();
            if (t == "]" && target is Variant)
                return new TypeOf(parent, new Variant(parent, (target as Variant).Name + "[]"));
            else if (t != null)
                Rewind();
            var ret = new Index(parent, target, Expression());
            Check("配列", "]");
            return ret;
        }

        private Call ConvertToCall(NodeBase v)
        {
            if (v is Call)
                return v as Call;
            if (v is Variant)
                return new Call(parent, (v as Variant).Name);
            if (!(v is Struct.Member))
                return new Call(parent, v, null);
            var mem = v as Struct.Member;
            return new Call(parent, mem.GetName(), mem.GetTarget());
        }

        private NodeBase PipeForward(NodeBase arg1, NodeBase arg2)
        {
            var ret = ConvertToCall(arg2);
            ret.PipeForward(arg1);
            return ret;
        }

        private NodeBase PipeBack(NodeBase arg1, NodeBase arg2)
        {
            var ret = ConvertToCall(arg2);
            ret.PipeBack(arg1);
            return ret;
        }
    }
}
