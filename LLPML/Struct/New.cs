using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Girl.Binary;
using Girl.X86;

namespace Girl.LLPML.Struct
{
    public class New : NodeBase
    {
        public const string Function = "__operator_new";
        public const string DereferencePtr = "__dereference_ptr";

        protected TypeBase type;
        public override TypeBase Type { get { return type; } }
        public NodeBase Length { get; protected set; }
        public bool IsArray { get { return !(Length is IntValue && (Length as IntValue).Value == -1); } }

        public New(BlockBase parent, string type)
        {
            Parent = parent;
            this.type = Types.GetVarType(parent, type);
            Length = new IntValue(-1);
        }

        public New(BlockBase parent, string type, NodeBase length)
        {
            Parent = parent;
            this.type = new TypeReference(Types.GetType(parent, type), true);
            Length = length;
        }

        public override void AddCodes(OpModule codes)
        {
            AddCodesValue(codes, "mov", null);
        }

        public override void AddCodesValue(OpModule codes, string op, Addr32 dest)
        {
            var tt = Type.Type;
            var tts = tt as TypeStruct;
            if (!IsArray && (tts == null || !tts.IsClass))
                throw Abort("new: is not class: {0}", tts.Name);
            var f = Parent.GetFunction(Function);
            if (f == null)
                throw Abort("new: undefined function: {0}", Function);
            Val32 type = codes.GetTypeObject(Type), izer = Val32.New(0), ctor = Val32.New(0), init = null;
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
            codes.Add(I386.PushD(ctor));
            codes.Add(I386.PushD(izer));
            Length.AddCodesValue(codes, "push", null);
            codes.Add(I386.PushD(Val32.NewI(tt.Size)));
            codes.Add(I386.PushD(type));
            codes.Add(I386.CallD(f.First));
            codes.Add(I386.AddR(Reg32.ESP, Val32.New(16)));
            if (init != null)
            {
                codes.Add(I386.Push(Reg32.EAX));
                codes.Add(I386.CallD(init));
                codes.Add(I386.Pop(Reg32.EAX));
            }
            codes.AddCodes(op, dest);
        }
    }
}
