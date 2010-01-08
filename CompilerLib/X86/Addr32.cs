﻿using System;
using System.Collections.Generic;
using System.Text;
using Girl.Binary;

namespace Girl.X86
{
    public class Addr32
    {
        private Reg32 reg;
        public Reg32 Register { get { return reg; } }

        private int disp = 0;
        public int Disp { get { return disp; } }

        private Ref<uint> address;
        public Ref<uint> Address { get { return address; } }
        public bool IsAddress { get { return address != null; } }

        public byte MiddleBits = 0;

        public Addr32(Reg32 r) { reg = r; }
        public Addr32(Reg32 r, int os) { reg = r; disp = os; }
        public Addr32(Ref<uint> ad) { address = ad; }

        private byte[] GetModRM()
        {
            if (address != null)
                return Util.GetBytes(new byte[] { 0x05 }, address.Value);

            sbyte sbdisp = (sbyte)disp;
            if (reg == Reg32.ESP)
            {
                if (disp == 0)
                    return new byte[] { 0x04, 0x24 };
                else if (disp == sbdisp)
                    return new byte[] { 0x44, 0x24, (byte)sbdisp };
                else
                    return Util.GetBytes(new byte[] { 0x84, 0x24 }, (uint)disp);
            }
            else if (reg == Reg32.EBP || disp != 0)
            {
                if (disp == sbdisp)
                    return new byte[] { (byte)(0x40 + (int)reg), (byte)sbdisp };
                else
                    return Util.GetBytes(new byte[] { (byte)(0x80 + (int)reg) }, (uint)disp);
            }
            else
            {
                return new byte[] { (byte)reg };
            }
        }

        public byte[] GetCodes()
        {
            byte[] ret = GetModRM();
            ret[0] += (byte)(MiddleBits << 3);
            return ret;
        }

        public void Write(Block block)
        {
            if (address != null)
            {
                block.Add((byte)(0x05 + (MiddleBits << 3)));
                block.Add(address);
            }
            else
            {
                block.Add(GetCodes());
            }
        }
    }
}
