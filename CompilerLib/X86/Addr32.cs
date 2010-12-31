using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public class Addr32
    {
        private bool isInitialized;
        private Reg32 reg;
        private int disp;
        private Val32 address;
        private byte middleBits;

        public bool IsInitialized { get { return isInitialized; } }
        public Reg32 Register { get { return reg; } }
        public int Disp { get { return disp; } }
        public Val32 Address { get { return address; } }
        public bool IsAddress { get { return address != null; } }

        public static Addr32 New(Reg32 r) { var ret = new Addr32(); ret.isInitialized = true; ret.reg = r; return ret; }
        public static Addr32 NewRO(Reg32 r, int offset) { var ret = new Addr32(); ret.isInitialized = true; ret.reg = r; ret.disp = offset; return ret; }
        public static Addr32 NewD(Val32 ad) { var ret = new Addr32(); ret.isInitialized = true; ret.address = ad; return ret; }
        public static Addr32 NewUInt(uint ad) { return NewD(Val32.New(ad)); }
        public static Addr32 NewAd(Addr32 src) { var ret = new Addr32(); ret.isInitialized = true; ret.Set(src); return ret; }
        public static Addr32 NewAdM(Addr32 src, byte middleBits)
        {
            var ret = NewAd(src);
            ret.middleBits = middleBits;
            return ret;
        }

        public void Set(Addr32 src)
        {
            isInitialized = src.isInitialized;
            reg = src.reg;
            disp = src.disp;
            address = src.address;
            middleBits = src.middleBits;
        }

        private byte[] GetModRM()
        {
            if (address != null)
                return Util.AddUIntToBytes(Util.GetBytes1(0x05), address.Value);

            sbyte sbdisp = (sbyte)disp;
            if (reg == Reg32.ESP)
            {
                if (disp == 0)
                    return Util.GetBytes2(0x04, 0x24);
                else if (disp == sbdisp)
                    return Util.GetBytes3(0x44, 0x24, (byte)sbdisp);
                else
                    return Util.AddUIntToBytes(Util.GetBytes2(0x84, 0x24), (uint)disp);
            }
            else if (reg == Reg32.EBP || disp != 0)
            {
                if (disp == sbdisp)
                    return Util.GetBytes2((byte)(0x40 + (int)reg), (byte)sbdisp);
                else
                    return Util.AddUIntToBytes(Util.GetBytes1((byte)(0x80 + (int)reg)), (uint)disp);
            }
            else
            {
                return Util.GetBytes1((byte)reg);
            }
        }

        public byte[] GetCodes()
        {
            byte[] ret = GetModRM();
            ret[0] += (byte)(middleBits << 3);
            return ret;
        }

        public void Write(Block block)
        {
            if (address != null)
            {
                block.AddByte((byte)(0x05 + (middleBits << 3)));
                block.AddVal32(address);
            }
            else
            {
                block.AddBytes(GetCodes());
            }
        }

        public void Add(int n)
        {
            if (n == 0) return;

            if (address != null)
            {
                address = Val32.New2(address, Val32.NewI(n));
            }
            else
            {
                disp += n;
            }
        }
    }
}
