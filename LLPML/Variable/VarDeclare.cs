using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.LLPML.Struct;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class VarDeclare : NodeBase
    {
        public Addr32 Address { get; set; }
        public bool IsMember { get; protected set; }
        public NodeBase Value { get; set; }
        public bool IsStatic { get; set; }

        public virtual bool NeedsInit { get { return Value != null; } }
        public virtual bool NeedsCtor { get { return type.NeedsCtor; } }
        public virtual bool NeedsDtor { get { return type.NeedsDtor; } }

        protected VarDeclare() { }

        public VarDeclare(BlockBase parent, string name)
        {
            Parent = parent;
            this.name = name;
            Init();
            AddToParent();
        }

        public VarDeclare(BlockBase parent, string name, TypeBase type)
            : this(parent, name)
        {
            if (type != null)
            {
                doneInferType = true;
                this.type = type;
            }
        }

        public VarDeclare(BlockBase parent, string name, TypeBase type, NodeBase value)
            : this(parent, name, type)
        {
            Value = value;
        }

        public static VarDeclare Array(BlockBase parent, string name, TypeBase type, NodeBase count)
        {
            var ret = new VarDeclare(parent, name);
            ret.doneInferType = true;
            ret.type = TypeArray.New(type, count);
            return ret;
        }

        protected virtual void Init()
        {
            if (type == null) type = TypeVar.Instance;
            IsMember = Parent is Define;
            if (Parent.Parent == null) IsStatic = true;
        }

        public Define GetStruct()
        {
            var ret = Types.GetStruct(Type);
            if (ret != null) return ret;
            throw Abort("undefined struct: " + Type.Name);
        }

        public Addr32 GetAddress(OpModule codes, BlockBase scope)
        {
            if (IsMember && !IsStatic)
            {
                var thisptr = This.New(scope);
                codes.Add(I386.MovRA(Var.DestRegister, thisptr.GetAddress(codes)));
                return Addr32.NewAd(Address);
            }

            int plv = scope.Level, lv = Parent.Level;
            if (plv == lv || Address.IsAddress)
                return Addr32.NewAd(Address);
            if (lv <= 0 || lv >= plv)
                throw Abort("Invalid variable scope: " + Name);
            codes.Add(I386.MovRA(Var.DestRegister, Addr32.NewRO(Reg32.EBP, -lv * 4)));
            return Addr32.NewRO(Var.DestRegister, Address.Disp);
        }

        protected virtual void AddToParent()
        {
            if (!Parent.AddVar(this))
                throw Abort("multiple definitions: " + name);
        }

        private void AddConstructor(OpModule codes)
        {
            if (!type.NeedsCtor) return;
            var pst = Parent as Define;
            if (pst != null && pst.IsClass)
            {
                var tr = type as TypeReference;
                if (tr != null && tr.UseGC) return;
            }
            type.AddConstructor(codes, GetAddress(codes, Parent));
        }

        public override void AddCodes(OpModule codes)
        {
            try
            {
                AddConstructor(codes);
            }
            catch
            {
                throw Abort("undefined type: {0}: {1}", name, type.Name);
            }

            if (Value != null)
            {
                var s = Set.New(Parent, Var.New(Parent, this), Value);
                s.AddCodes(codes);
            }
        }

        protected TypeBase type;
        protected bool doneInferType = false;

        public override TypeBase Type
        {
            get
            {
                if (doneInferType || !root.IsCompiling)
                    return type;

                doneInferType = true;
                if (Value != null)
                    type = Types.ToVarType(Value.Type);
                return type;
            }
        }

        public string FullName
        {
            get { return Parent.GetFullName(name); }
        }

        public void CheckClass()
        {
            var t = Type;
            TypeStruct ts = null;
            if (Type is TypeStruct)
                ts = t as TypeStruct;
            else if (Type is TypeArray)
                ts = t.Type as TypeStruct;
            if (ts == null) return;

            var st = ts.GetStruct();
            st.MakeUp();
            if (!st.IsClass) return;

            throw Abort(
                "can not declare class as automatic variable: {0}",
                st.FullName);
        }
    }
}
