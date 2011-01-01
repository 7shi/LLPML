using System;
using Girl.Binary;
using Girl.X86;

namespace Girl.PE
{
    public class DOSHeader : HeaderBase
    {
        public string signature = "MZ";
        public ushort bytes_in_last_block = 0x90;
        public ushort blocks_in_file = 3;
        public ushort num_relocs = 0;
        public ushort header_paragraphs = 4;
        public ushort min_extra_paragraphs = 0;
        public ushort max_extra_paragraphs = 0xffff;
        public ushort ss = 0;
        public ushort sp = 0xb8;
        public ushort checksum = 0;
        public ushort ip = 0;
        public ushort cs = 0;
        public ushort reloc_table_offset = 0x40;
        public ushort overlay_number = 0;

        public override void WriteBlock(Block block)
        {
            block.AddString(signature);
            block.AddUShort(bytes_in_last_block);
            block.AddUShort(blocks_in_file);
            block.AddUShort(num_relocs);
            block.AddUShort(header_paragraphs);
            block.AddUShort(min_extra_paragraphs);
            block.AddUShort(max_extra_paragraphs);
            block.AddUShort(ss);
            block.AddUShort(sp);
            block.AddUShort(checksum);
            block.AddUShort(ip);
            block.AddUShort(cs);
            block.AddUShort(reloc_table_offset);
            block.AddUShort(overlay_number);
        }

        public static OpCode[] Stub
        {
            get
            {
                var ret = new OpCode[8];
                ret[0] = I8086.PushS(SegReg.CS);
                ret[1] = I8086.PopS(SegReg.DS);
                ret[2] = I8086.Mov(Reg16.DX, 0x000e);
                ret[3] = I8086.MovB(Reg8.AH, 0x09);
                ret[4] = I8086.Int(0x21);
                ret[5] = I8086.Mov(Reg16.AX, 0x4c01);
                ret[6] = I8086.Int(0x21);
                ret[7] = OpCode.NewString("This program cannot be run in DOS mode.\r\n$");
                return ret;
            }
        }
    }
}
