using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class Declare : VarDeclare
    {
        private ArrayList values = new ArrayList();
        public ArrayList Values { get { return values; } }

        public override bool NeedsInit
        {
            get
            {
                if (values != null) return true;
                var st = GetStruct();
                return st != null && st.NeedsInit;
            }
        }

        public override bool NeedsCtor
        {
            get
            {
                var st = GetStruct();
                return st != null && st.NeedsCtor;
            }
        }

        public override bool NeedsDtor
        {
            get
            {
                var st = GetStruct();
                return st != null && st.NeedsDtor;
            }
        }

        public static Declare NewDecl(Declare parent)
        {
            var ret = new Declare();
            ret.Parent = parent.Parent;
            return ret;
        }

        public static Declare New(BlockBase parent, string name, string type)
        {
            var ret = new Declare();
            ret.init1(parent, name, null);
            ret.type = Types.GetType(parent, type) as TypeStruct;
            if (ret.type == null) throw ret.Abort("type required");
            return ret;
        }

        public override void AddCodes(OpModule codes)
        {
            var st = GetStruct();
            if (!st.NeedsInit && values.Count == 0 && !st.NeedsCtor)
                return;
            codes.Add(I386.Lea(Reg32.EAX, GetAddress(codes, Parent)));
            codes.Add(I386.Push(Reg32.EAX));
            var ad = Addr32.New(Reg32.ESP);
            st.AddInit(codes, ad);
            AddInitValues(codes, st);
            st.AddConstructor(codes, ad);
            codes.Add(I386.AddR(Reg32.ESP, Val32.New(4)));
        }

        private bool AddInitValues(OpModule codes, Define st)
        {
            if (values.Count == 0) return false;

            var members = st.GetMemberDecls();
            if (members.Length != values.Count)
                throw Abort("initializers mismatched: " + st.Name);

            var ad = Addr32.New(Reg32.ESP);
            codes.Add(I386.PushA(ad));
            for (int i = 0; i < values.Count; i++)
            {
                VarDeclare mem = members[i];
                object obj = values[i];
                if (obj is Declare)
                {
                    Define memst = Types.GetStruct(mem.Type);
                    if (!(mem is Declare) || memst == null)
                        throw Abort("struct required: " + mem.Name);
                    (obj as Declare).AddInitValues(codes, memst);
                }
                else if (obj is NodeBase)
                {
                    if (!(mem is VarDeclare))
                        throw Abort("value required: " + mem.Name);
                    (obj as NodeBase).AddCodesV(codes, "mov", null);
                    codes.Add(I386.MovRA(Var.DestRegister, ad));
                    mem.Type.AddSetCodes(codes, Addr32.New(Var.DestRegister));
                }
                else
                    throw Abort("invalid parameter: " + mem.Name);
                codes.Add(I386.AddA(ad, Val32.NewI(mem.Type.Size)));
            }
            codes.Add(I386.AddR(Reg32.ESP, Val32.New(4)));
            return true;
        }

        public void CheckField(Define st1, Define st2)
        {
            st2.MakeUp();
            if (st1 == st2)
                throw Abort(
                    "can not define recursive field: {0}",
                    st1.GetFullName(name));
            var b = st2.GetBaseStruct();
            if (b != null) CheckField(st1, b);
        }
    }
}
