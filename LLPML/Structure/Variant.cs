using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.LLPML.Struct;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class Variant : NodeBase
    {
        private Function func;

        public static Variant NewName(BlockBase parent, string name)
        {
            var ret = new Variant();
            ret.Parent = parent;
            ret.name = name;
            return ret;
        }

        public static Variant New(Function func)
        {
            var ret = new Variant();
            ret.func = func;
            return ret;
        }

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
            if (g != null)
            {
                var rt = g.ReturnType;
                if (rt != null) return rt;
                return TypeVar.Instance;
            }

            var s = GetSetter();
            if (s != null) return (s.Args[1] as VarDeclare).Type;

            return null;
        }

        public override void AddCodesV(OpModule codes, string op, Addr32 dest)
        {
            Val32 v;
            var m = codes.Module;
            if (func != null)
                v = func.GetAddress(m);
            else
            {
                var vv = GetVar();
                if (vv != null)
                {
                    vv.AddCodesV(codes, op, dest);
                    return;
                }
                var c = GetConst();
                if (c != null)
                {
                    c.AddCodesV(codes, op, dest);
                    return;
                }
                var f = GetFunction();
                if (f == null)
                {
                    var g = GetGetter();
                    if (g != null)
                    {
                        Call.NewName(Parent, g.Name).AddCodesV(codes, op, dest);
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
            if (v != null && v.Parent is Define)
            {
                var ret = Var.New(Parent, v);
                ret.SrcInfo = SrcInfo;
                return ret;
            }
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
            if (v != null && (v.Parent is Define || v.Parent.Parent == null))
                return Var.New(parent, v);

            return null;
        }
    }
}
