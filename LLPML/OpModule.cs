﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Girl.Binary;
using Girl.LLPML.Struct;
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

        public static OpModule Create(Module m)
        {
            var ret = new OpModule();
            ret.Module = m;
            return ret;
        }

        private ListDictionary strings = new ListDictionary();

        public Val32 GetString(string s)
        {
            if (strings.ContainsKey(s))
                return strings.Get(s) as Val32;

            var block = new Block32();
            block.AddBytes(Module.EncodeString(s));
            var type = Val32.NewB(0, true);
            var ret = AddData("string_constant", s, type, 2, s.Length, block);
            strings.Add(s, ret);
            type.Reference = GetTypeObjectD(Root.GetStruct("string"));
            return ret;
        }

        private ListDictionary types = new ListDictionary();

        public Val32 GetTypeObjectD(Define st)
        {
            if (st == null) return Val32.New(0);

            var name = st.FullName;
            if (types.ContainsKey(name))
                return types.Get(name) as Val32;

            return GetTypeObjectV(
                name, st.GetFunction(Define.Destructor),
                st.GetSize(), GetTypeObjectD(st.GetBaseStruct()));
        }

        public Val32 GetTypeObjectV(string name, Function dtor, int size, Val32 baseType)
        {
            if (types.ContainsKey(name))
                return types.Get(name) as Val32;

            var block = new Block32();
            var namev = Val32.NewB(0, true);
            block.AddVal32(namev);
            if (dtor == null || name == "string" || name == "Type")
                block.AddInt(0);
            else
                block.AddVal32(GetAddress(dtor));
            block.AddInt(size);
            block.AddVal32(baseType);
            var type = Val32.NewB(0, true);
            var tsz = (int)block.Length;
            var ret = AddData("type_object", name, type, tsz, -1, block);
            types.Add(name, ret);
            namev.Reference = GetString(name);
            type.Reference = GetTypeObjectD(Root.GetStruct("Type"));
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
                Val32 targetType = Val32.New(0);
                if (tt is TypeReference)
                {
                    dtor = OpModule.Root.GetFunction(New.DereferencePtr);
                    var at = tt.Type as TypeStruct;
                    if (at != null)
                    {
                        name = at.Name;
                        targetType = GetTypeObjectD(at.GetStruct());
                    }
                }
                else if (tts != null)
                {
                    name = tts.Name;
                    targetType = GetTypeObjectD(tts.GetStruct());
                }
                return GetTypeObjectV(name + "[]", dtor, tt.Size, targetType);
            }

            var ts = type as TypeStruct;
            if (ts != null)
            {
                var st = ts.GetStruct();
                return GetTypeObjectD(st);
            }

            return GetTypeObjectV(type.Name, null, type.Size, Val32.New(0));
        }

        public Val32 AddData(string category, string name, Val32 type, int size, int len, Block32 data)
        {
            var db = new DataBlock();
            db.Block.AddVal32(type);
            db.Block.AddInt(1);
            db.Block.AddInt(size);
            db.Block.AddInt(len);
            var offset = db.Block.Length;
            db.Block.AddBlock(data);
            Module.Data.AddDataBlock(category, name, db);
            return Val32.New2(db.Address, Val32.New(offset));
        }

        public Val32 GetAddress(Function f)
        {
            if (f == null) return Val32.New(0);
            return f.GetAddress(Module);
        }

        public static bool NeedsDtor(NodeBase v)
        {
            while (v is Cast) v = (v as Cast).Source;
            var vt = v.Type;
            if (vt == null) return false;

            var vsm = v as Member;
            if (vsm != null && vt is TypeDelegate && vsm.GetDelegate() != null)
                return true;

            var mem = v as Member;
            return vt.NeedsDtor && !(v is As)
                && (v is Call || v is New || v is DelgFunc || v is Operator
                || (mem != null && mem.IsGetter));
        }

        public void AddDtorCodes(TypeBase t)
        {
            Add(I386.Push(Reg32.ESP));
            t.AddDestructor(this);
            Add(I386.AddR(Reg32.ESP, Val32.New(8)));
        }

        public void AddOperatorCodes(TypeBase tb, string op, Addr32 dest, NodeBase arg, bool pushf)
        {
            arg.AddCodesV(this, "mov", null);
            var cleanup = NeedsDtor(arg);
            if (cleanup)
            {
                Add(I386.Push(Reg32.EAX));
                if (dest.Register == Reg32.ESP)
                    dest = Addr32.NewRO(dest.Register, dest.Disp + 4);
            }
            tb.AddOpCodes(op, this, dest);
            if (cleanup)
            {
                if (pushf)
                {
                    Add(I386.Pop(Reg32.EAX));
                    Add(I386.Pushf());
                    Add(I386.Push(Reg32.EAX));
                }
                AddDtorCodes(arg.Type);
                if (pushf) Add(I386.Popf());
            }
        }

        public OpCode GetCall(string tag, string name)
        {
            var f = Root.GetFunction(name);
            if (f == null)
                throw root.Abort("{0}: can not find: {1}", tag, name);
            return I386.CallD(f.First);
        }
    }
}
