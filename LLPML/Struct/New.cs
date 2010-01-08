﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class New : NodeBase, IIntValue
    {
        public const string Function = "__operator_new";
        public const string DereferencePtr = "__dereference_ptr";

        public IIntValue Length { get; protected set; }
        public bool IsArray { get { return arrayType != null; } }
        public bool NoSet { get; set; }

        public New(BlockBase parent, string type)
            : base(parent)
        {
            this.type = Types.GetVarType(parent, type);
            Length = new IntValue(-1);
        }

        public New(BlockBase parent, string type, IIntValue length)
            : base(parent)
        {
            arrayType = Types.GetType(parent, type);
            Length = length;
        }

        public New(BlockBase parent, XmlTextReader xr)
            : base(parent, xr)
        {
        }

        public override void Read(XmlTextReader xr)
        {
            NoChild(xr);

            var type = xr["type"];
            var length = xr["length"];
            int len;
            if (length != null && int.TryParse(length, out len))
            {
                arrayType = Types.GetType(parent, type);
                Length = new IntValue(len);
            }
            else
            {
                this.type = Types.GetVarType(parent, type);
                Length = new IntValue(-1);
            }
        }

        public override void AddCodes(OpCodes codes)
        {
            if (!NoSet)
                AddCodes(codes, "mov", null);
            else
            {
                AddCodes(codes, "push", null);
                codes.AddRange(new[]
                {
                    I386.Mov(Reg32.EAX, new Addr32(Reg32.ESP)),
                    I386.Inc(new Addr32(Reg32.EAX, -12)),
                    I386.Push(Reg32.ESP),
                });
                Type.AddDestructor(codes);
                codes.Add(I386.Add(Reg32.ESP, 8));
            }
        }

        public void AddCodes(OpCodes codes, string op, Addr32 dest)
        {
            var tt = Type.Type;
            var tts = tt as TypeStruct;
            if (!IsArray && tts != null && !tts.IsClass)
                throw Abort("new: is not class: {0}", tts.Name);
            var f = parent.GetFunction(Function);
            if (f == null)
                throw Abort("new: undefined function: {0}", Function);
            Val32 izer = 0, ctor = 0, dtor = 0;
            if (tts != null)
            {
                var st = tts.GetStruct();
                izer = st.GetFunction(Define.Initializer).GetAddress(codes.Module);
                ctor = st.GetFunction(Define.Constructor).GetAddress(codes.Module);
                dtor = st.GetFunction(Define.Destructor).GetAddress(codes.Module);
            }
            else if (IsArray)
            {
                dtor = parent.GetFunction(DereferencePtr).GetAddress(codes.Module);
            }
            codes.AddRange(new[]
            {
                I386.Push(dtor),
                I386.Push(ctor),
                I386.Push(izer),
                I386.Push((uint)tt.Size),
            });
            Length.AddCodes(codes, "push", null);
            codes.AddRange(new[]
            {
                I386.Call(f.First),
                I386.Add(Reg32.ESP, 16),
            });
            codes.AddCodes(op, dest);
        }

        private TypeBase type, arrayType;

        public TypeBase Type
        {
            get
            {
                if (type != null) return type;

                TypeBase t = arrayType;
                if (t is TypeStruct && (t as TypeStruct).IsClass)
                    t = new TypeReference(parent, t);
                type = new TypeReference(parent, t, true);
                return type;
            }
        }
    }
}
