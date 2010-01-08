﻿using System;
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

        private static Root root;
        public static Root Root
        {
            get { return root; }
            set
            {
                TypeString.Init();
                TypeType.Init();
                root = value;
            }
        }

        public OpModule(Module m)
        {
            Module = m;
        }

        private Dictionary<string, Val32> strings = new Dictionary<string, Val32>();

        public Val32 GetString(string s)
        {
            if (strings.ContainsKey(s)) return strings[s];

            var block = new Girl.Binary.Block();
            block.Add(Module.DefaultEncoding.GetBytes(s + "\0"));
            var type = new Val32(0, true);
            var ret = strings[s] = AddData("string_constant", s, type, 2, s.Length, block);
            type.Reference = GetTypeObject(Root.GetStruct("string"));
            return ret;
        }

        private Dictionary<string, Val32> types = new Dictionary<string, Val32>();

        public Val32 GetTypeObject(Struct.Define st)
        {
            if (st == null) return 0;

            var name = st.FullName;
            if (types.ContainsKey(name)) return types[name];

            return GetTypeObject(
                name, st.GetFunction(Struct.Define.Destructor),
                st.GetSize(), GetTypeObject(st.GetBaseStruct()));
        }

        public Val32 GetTypeObject(string name, Function dtor, int size, Val32 baseType)
        {
            if (types.ContainsKey(name)) return types[name];

            var block = new Girl.Binary.Block();
            var namev = new Val32(0, true);
            block.Add(namev);
            if (dtor == null || name == "string" || name == "Type")
                block.Add(0);
            else
                block.Add(GetAddress(dtor));
            block.Add(size);
            block.Add(baseType);
            var type = new Val32(0, true);
            var tsz = (int)block.Length;
            var ret = types[name] = AddData("type_object", name, type, tsz, -1, block);
            namev.Reference = GetString(name);
            type.Reference = GetTypeObject(Root.GetStruct("Type"));
            return ret;
        }

        public Val32 GetTypeObject(TypeBase type)
        {
            var tr = type as TypeReference;
            if (tr != null)
            {
                var tt = type.Type;
                if (!tr.IsArray) return GetTypeObject(tt);

                var tts = tt as TypeStruct;
                Function dtor = null;
                string name = tt.Name;
                Val32 targetType = 0;
                if (tt is TypeReference)
                {
                    dtor = OpModule.Root.GetFunction(Struct.New.DereferencePtr);
                    var at = tt.Type as TypeStruct;
                    if (at != null)
                    {
                        name = at.Name;
                        targetType = GetTypeObject(at.GetStruct());
                    }
                }
                else if (tts != null)
                {
                    name = tts.Name;
                    targetType = GetTypeObject(tts.GetStruct());
                }
                return GetTypeObject(name + "[]", dtor, tt.Size, targetType);
            }

            var ts = type as TypeStruct;
            if (ts != null)
            {
                var st = ts.GetStruct();
                return GetTypeObject(st);
            }

            return GetTypeObject(type.Name, null, type.Size, 0);
        }

        public Val32 AddData(string category, string name, Val32 type, int size, int len, Girl.Binary.Block data)
        {
            var db = new DataBlock();
            db.Block.Add(type);
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
            if (f == null) return 0;
            return f.GetAddress(Module);
        }

        public static bool NeedsDtor(IIntValue v)
        {
            while (v is Cast) v = (v as Cast).Source;
            var vt = v.Type;
            if (vt == null) return false;

            var vsm = v as Struct.Member;
            if (vsm != null && vt is TypeDelegate && vsm.GetDelegate() != null)
                return true;

            return vt.NeedsDtor && !(v is Struct.As)
                && (v is Call || v is Struct.New || v is Delegate || v is Operator);
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