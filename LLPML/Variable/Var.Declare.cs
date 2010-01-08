using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public partial class Var
    {
        public class Declare : NodeBase
        {
            public Addr32 Address { get; set; }
            public bool IsMember { get; protected set; }
            public IIntValue Value { get; set; }
            public bool IsStatic { get; set; }

            public virtual bool NeedsInit { get { return Value != null; } }
            public virtual bool NeedsCtor { get { return type.NeedsCtor; } }
            public virtual bool NeedsDtor { get { return type.NeedsDtor; } }

            public Declare() { }

            public Declare(BlockBase parent, string name)
                : base(parent, name)
            {
                Init();
                AddToParent();
            }

            public Declare(BlockBase parent, string name, TypeBase type)
                : this(parent, name)
            {
                if (type != null)
                {
                    doneInferType = true;
                    this.type = type;
                }
            }

            public Declare(BlockBase parent, string name, TypeBase type, IIntValue value)
                : this(parent, name, type)
            {
                Value = value;
            }

            public Declare(BlockBase parent, string name, TypeBase type, int count)
                : this(parent, name)
            {
                doneInferType = true;
                this.type = new TypeArray(type, count);
            }

            public Declare(BlockBase parent, XmlTextReader xr)
                : base(parent, xr)
            {
                Init();
            }

            protected virtual void Init()
            {
                if (type == null) type = TypeVar.Instance;
                IsMember = parent is Struct.Define;
                if (parent.Parent == null) IsStatic = true;
            }

            public override void Read(XmlTextReader xr)
            {
                RequiresName(xr);

                var t = Types.GetType(parent, xr["type"]);
                if (xr["static"] == "1") IsStatic = true;

                string slen = xr["length"];
                if (slen != null)
                {
                    var c = IntValue.Parse(slen);
                    if (t != null) type = new TypeArray(t, c);
                }
                else
                {
                    if (t != null) type = t;
                    Parse(xr, delegate
                    {
                        IIntValue[] v = IntValue.Read(parent, xr);
                        if (v != null)
                        {
                            if (v.Length > 1 || Value != null)
                                throw Abort(xr, "multiple values");
                            Value = v[0];
                        }
                    });
                }

                AddToParent();
            }

            public Struct.Define GetStruct()
            {
                var ret = Types.GetStruct(Type);
                if (ret != null) return ret;
                throw Abort("undefined struct: " + Type.Name);
            }

            public Addr32 GetAddress(OpCodes codes, BlockBase scope)
            {
                if (IsMember && !IsStatic)
                {
                    var thisptr = new Struct.This(scope);
                    codes.Add(I386.Mov(Var.DestRegister, thisptr.GetAddress(codes)));
                    return new Addr32(Address);
                }

                int plv = scope.Level, lv = parent.Level;
                if (plv == lv || Address.IsAddress)
                    return new Addr32(Address);
                if (lv <= 0 || lv >= plv)
                    throw Abort("Invalid variable scope: " + Name);
                codes.Add(I386.Mov(Var.DestRegister, new Addr32(Reg32.EBP, -lv * 4)));
                return new Addr32(Var.DestRegister, Address.Disp);
            }

            protected virtual void AddToParent()
            {
                if (!parent.AddVar(this))
                    throw Abort("multiple definitions: " + name);
            }

            public override void AddCodes(OpCodes codes)
            {
                if (type.NeedsCtor)
                    type.AddConstructor(codes, GetAddress(codes, parent));

                if (Value != null)
                {
                    var s = new Set(parent, new Var(parent, this), Value);
                    s.AddCodes(codes);
                }
            }

            protected TypeBase type;
            protected bool doneInferType = false;

            public TypeBase Type
            {
                get
                {
                    if (doneInferType || !root.IsCompiling)
                        return type;

                    doneInferType = true;
                    if (Value != null)
                        type = Types.ConvertVarType(parent, Value.Type);
                    return type;
                }
            }

            public string FullName
            {
                get { return parent.GetFullName(name); }
            }
        }
    }
}
