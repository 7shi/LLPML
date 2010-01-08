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

            public virtual bool NeedsInit { get { return Value != null; } }
            public virtual bool NeedsCtor { get { return false; } }
            public virtual bool NeedsDtor { get { return false; } }

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
                if (type == null) type = Types.GetValueType("var");
                IsMember = parent is Struct.Define;
            }

            public override void Read(XmlTextReader xr)
            {
                RequiresName(xr);

                var t = Types.GetType(parent, xr["type"]);

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
                if (IsMember)
                {
                    var thisptr = new Struct.This(scope);
                    codes.Add(I386.Mov(Var.DestRegister, thisptr.GetAddress(codes)));
                    return new Addr32(Address);
                }

                int plv = scope.Level, lv = parent.Level;
                if (plv == lv || Address.IsAddress)
                    return new Addr32(Address);
                if (lv <= 0 || lv >= plv)
                    throw Abort("Invalid variable scope: " + name);
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
                if (Value == null) return;

                Value.AddCodes(codes, "mov", null);
                var ad = Address;
                if (IsMember)
                    ad = GetAddress(codes, parent);
                Type.AddSetCodes(codes, ad);
            }

            protected TypeBase type;
            protected bool doneInferType = false;

            public virtual TypeBase Type
            {
                get
                {
                    if (doneInferType || !root.IsCompiling)
                        return type;

                    doneInferType = true;
                    if (Value != null)
                        type = Types.ConvertVarType(Value.Type);
                    return type;
                }
            }
        }
    }
}
