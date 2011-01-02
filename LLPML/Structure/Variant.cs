using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Variant : NodeBase
    {
        private Function func;
        private Val32 address;

        public Variant(BlockBase parent, string name) : base(parent, name) { }
        public Variant(Val32 address) { this.address = address; }
        public Variant(Function func) { this.func = func; }

        public override TypeBase Type
        {
            get
            {
                var ret = GetVariantType();
                if (ret != null) return ret;

                throw Abort("undefined symbol: {0}", name);
            }
        }

        public TypeBase GetVariantType()
        {
            var v = GetVar();
            if (v != null) return v.Type;

            var c = GetConst();
            if (c != null) return c.Type;

            var f = GetFunction();
            if (f != null) return f.Type;

            var g = GetGetter();
            if (g != null) return g.ReturnType ?? TypeVar.Instance;

            var s = GetSetter();
            if (s != null) return s.Args[1].Type;

            return null;
        }

        public override void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            Val32 v;
            var m = codes.Module;
            if (address != null)
                v = Val32.New2(Val32.New(m.Specific.ImageBase), address);
            else if (func != null)
                v = func.GetAddress(m);
            else
            {
                var vv = GetVar();
                if (vv != null)
                {
                    vv.AddCodes(codes, op, dest);
                    return;
                }
                var c = GetConst();
                if (c != null)
                {
                    c.AddCodes(codes, op, dest);
                    return;
                }
                var f = GetFunction();
                if (f == null)
                {
                    var g = GetGetter();
                    if (g != null)
                    {
                        new Call(Parent, g.Name).AddCodes(codes, op, dest);
                        return;
                    }
                    throw Abort("undefined symbol: " + name);
                }
                v = f.GetAddress(m);
            }
            codes.AddCodesV(op, dest, v);
        }

        public Function GetFunction()
        {
            if (func != null) return func;
            return Parent.GetFunction(name);
        }

        public Function GetGetter()
        {
            if (GetFunction() != null) return null;
            return Parent.GetFunction("get_" + name);
        }

        public Function GetSetter()
        {
            if (GetFunction() != null) return null;
            return Parent.GetFunction("set_" + name);
        }

        public Var GetVar()
        {
            if (GetFunction() != null) return null;
            var v = Parent.GetVar(name);
            if (v != null && v.Parent is Struct.Define)
                return new Var(Parent, v) { SrcInfo = SrcInfo };
            return null;
        }

        public bool IsGetter
        {
            get { return GetGetter() != null; }
        }

        public bool IsSetter
        {
            get { return GetSetter() != null; }
        }

        public NodeBase GetConst()
        {
            var ret = GetTarget(Parent, name);
            if (ret is NodeBase) (ret as NodeBase).SrcInfo = SrcInfo;
            return ret;
        }

        public static NodeBase GetTarget(BlockBase parent, string name)
        {
            if (parent == null || name == null)
                return null;

            var i = parent.GetInt(name);
            if (i != null && i.Parent.Parent == null) return i;

            var s = parent.GetString(name);
            if (s != null && s.Parent.Parent == null) return s;

            if (parent.Parent == null) return null;

            var v = parent.GetVar(name);
            if (v != null && (v.Parent is Struct.Define || v.Parent.Parent == null))
                return new Var(parent, v);

            return null;
        }
    }
}
