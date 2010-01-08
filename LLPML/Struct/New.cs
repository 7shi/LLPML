using System;
using System.Collections.Generic;
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
        public bool IsArray { get { return !(Length is IntValue && (Length as IntValue).Value == -1); } }
        public TypeBase Type { get; protected set; }

        public New(BlockBase parent, string type)
            : base(parent)
        {
            Type = Types.GetVarType(parent, type);
            Length = new IntValue(-1);
        }

        public New(BlockBase parent, string type, IIntValue length)
            : base(parent)
        {
            Type = new TypeReference(Types.GetType(parent, type), true);
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
                Type = new TypeReference(Types.GetType(Parent, type), true);
                Length = new IntValue(len);
            }
            else
            {
                Type = Types.GetVarType(Parent, type);
                Length = new IntValue(-1);
            }
        }

        public override void AddCodes(OpModule codes)
        {
            AddCodes(codes, "mov", null);
        }

        public void AddCodes(OpModule codes, string op, Addr32 dest)
        {
            var tt = Type.Type;
            var tts = tt as TypeStruct;
            if (!IsArray && (tts == null || !tts.IsClass))
                throw Abort("new: is not class: {0}", tts.Name);
            var f = Parent.GetFunction(Function);
            if (f == null)
                throw Abort("new: undefined function: {0}", Function);
            Val32 type = codes.GetTypeObject(Type), izer = 0, ctor = 0, init = null;
            if (!IsArray)
            {
                var st = tts.GetStruct();
                if (st.IsEmpty)
                {
                    init = st.First;
                    st = st.GetBaseStruct();
                    type = codes.GetTypeObject(st);
                }
                izer = codes.GetAddress(st.GetFunction(Define.Initializer));
                ctor = codes.GetAddress(st.GetFunction(Define.Constructor));
            }
            codes.AddRange(new[]
            {
                I386.Push(ctor),
                I386.Push(izer),
            });
            Length.AddCodes(codes, "push", null);
            codes.AddRange(new[]
            {
                I386.Push((uint)tt.Size),
                I386.Push(type),
                I386.Call(f.First),
                I386.Add(Reg32.ESP, 16),
            });
            if (init != null)
                codes.AddRange(new[]
                {
                    I386.Push(Reg32.EAX),
                    I386.Call(init),
                    I386.Pop(Reg32.EAX),
                });
            codes.AddCodes(op, dest);
        }
    }
}
