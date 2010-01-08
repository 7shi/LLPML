using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Girl.Binary;
using Girl.PE;
using Girl.X86;

namespace Girl.LLPML
{
    public class OpModule : OpCodes
    {
        public Module Module { get; private set; }

        public OpModule(Module m)
        {
            Module = m;
        }

        private Dictionary<string, Val32> strings = new Dictionary<string, Val32>();

        public Val32 GetString(string s)
        {
            if (strings.ContainsKey(s)) return strings[s];

            var db = new Girl.Binary.Block();
            db.Add(Module.DefaultEncoding.GetBytes(s + "\0"));
            return strings[s] = AddData("string_constant", s, 0u, 2u, (uint)s.Length, db);
        }

        public Val32 AddData(string category, string name, Val32 dtor, Val32 size, Val32 len, Girl.Binary.Block data)
        {
            var db = new DataBlock();
            db.Block.Add(dtor);
            db.Block.Add(1);
            db.Block.Add(size);
            db.Block.Add(len);
            var offset = db.Block.Length;
            db.Block.Add(data);
            Module.Data.Add(category, name, db);
            return new Val32(db.Address, offset);
        }

        public Val32 GetAddress(Function f)
        {
            return f.GetAddress(Module);
        }

        public static bool NeedsCtor(IIntValue v)
        {
            if (!(v is Var)) return false;

            var vt = v.Type;
            return vt is TypeReference && vt.NeedsCtor;
        }

        public static bool NeedsDtor(IIntValue v)
        {
            var vt = v.Type;

            var vsm = v as Struct.Member;
            if (vsm != null && vt is TypeDelegate && vsm.GetDelegate() != null)
                return true;

            if (v is Var && (vt is TypeStruct || vt is TypeDelegate))
                return false;
            else if (v is Set || vt == null || !vt.NeedsDtor)
                return false;
            else
                return true;
        }

        public void AddCtorCodes()
        {
            var label = new OpCode();
            AddRange(new[]
            {
                I386.Test(Reg32.EAX, Reg32.EAX),
                I386.Jcc(Cc.Z, label.Address),
                I386.Inc(new Addr32(Reg32.EAX, -12)),
                label,
            });
        }

        public void AddDtorCodes(TypeBase t)
        {
            Add(I386.Push(Reg32.ESP));
            t.AddDestructor(this);
            Add(I386.Add(Reg32.ESP, 8));
        }

        public void AddOperatorCodes(TypeBase.Func f, Addr32 dest, IIntValue arg, bool pushf)
        {
            arg.AddCodes(this, "mov", null);
            var cleanup = NeedsDtor(arg);
            if (cleanup)
            {
                Add(I386.Push(Reg32.EAX));
                if (dest.Register == Reg32.ESP)
                    dest = new Addr32(dest.Register, dest.Disp + 4);
            }
            if (NeedsCtor(arg)) AddCtorCodes();
            f(this, dest);
            if (cleanup)
            {
                if (pushf)
                    AddRange(new[]
                    {
                        I386.Pop(Reg32.EAX),
                        I386.Pushf(),
                        I386.Push(Reg32.EAX),
                    });
                AddDtorCodes(arg.Type);
                if (pushf) Add(I386.Popf());
            }
        }
    }
}
