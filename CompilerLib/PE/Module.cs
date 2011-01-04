using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Girl.Binary;
using Girl.X86;

namespace Girl.PE
{
    public class Module
    {
        public DOSHeader DOSHeader = new DOSHeader();

        public PEFileHeader PEHeader = new PEFileHeader();
        public PEHeaderStandardFields Standard = new PEHeaderStandardFields();
        public PEHeaderWindowsNTSpecificFields Specific = new PEHeaderWindowsNTSpecificFields();
        public PEHeaderDataDirectories DataDirs = new PEHeaderDataDirectories();

        private ArrayList Sections = new ArrayList();
        public TextSection Text = new TextSection();
        public DataSection Data = DataSection.New(".data");
        public DataSection RData = DataSection.New(".rdata");
        public DataSection BSS = DataSection.New(".bss");
        public ImportSection Import = new ImportSection();

        public static byte[] EncodeString(string s)
        {
            return Encoding.Unicode.GetBytes(s + "\0");
        }

        public Addr32 GetFunction(string lib, string sym)
        {
            return Addr32.NewD(Import.Add(lib, sym).ImportRef);
        }

        public Val32 GetString(string s)
        {
            return RData.AddString(s).Address;
        }

        public Val32 GetBuffer(string name, int size)
        {
            return BSS.AddBuffer(name, size).Address;
        }

        public Val32 GetInt32(string name)
        {
            return Data.AddBuffer(name, sizeof(int)).Address;
        }

        public SectionHeader GetSection(string name)
        {
            foreach (SectionBase sb in Sections)
                if (sb.Name == name) return sb.Header;
            return null;
        }

        public Block32 CreateBlock(SectionBase sb)
        {
            if (sb.Header != null)
                return sb.ToBlock(sb.Header.VirtualAddress);
            else
                return sb.ToBlock(GetNextVirtualAddress());
        }

        public SectionHeader AddSection(SectionBase sb)
        {
            if (Sections.Count == 16)
            {
                throw new Exception("Maximum sections.");
            }

            SectionHeader ret = new SectionHeader();
            ret.Name = sb.Name;
            ret.VirtualSize = CreateBlock(sb).Length;
            ret.VirtualAddress = GetNextVirtualAddress();
            ret.SizeOfRawData = Align(ret.VirtualSize, Specific.FileAlignment);
            ret.PointerToRawData = GetNextPointerToRawData();
            switch (ret.Name)
            {
                case ".text":
                    ret.Characteristics =
                        IMAGE_SCN.CNT_CODE | IMAGE_SCN.CNT_INITIALIZED_DATA |
                        IMAGE_SCN.MEM_EXECUTE | IMAGE_SCN.MEM_READ;
                    break;
                case ".rdata":
                    ret.Characteristics =
                        IMAGE_SCN.CNT_INITIALIZED_DATA |
                        IMAGE_SCN.MEM_READ;
                    break;
                case ".bss":
                    ret.Characteristics =
                        IMAGE_SCN.CNT_UNINITIALIZED_DATA |
                        IMAGE_SCN.MEM_READ | IMAGE_SCN.MEM_WRITE;
                    break;
                default:
                    ret.Characteristics =
                        IMAGE_SCN.CNT_INITIALIZED_DATA |
                        IMAGE_SCN.MEM_READ | IMAGE_SCN.MEM_WRITE;
                    break;
            }
            sb.Header = ret;
            Sections.Add(sb);
            return ret;
        }

        public uint GetNextVirtualAddress()
        {
            if (Sections.Count == 0) return Specific.SectionAlignment;
            var last = (Sections[Sections.Count - 1] as SectionBase).Header;
            return last.VirtualAddress + Align(last.VirtualSize, Specific.SectionAlignment);
        }

        public uint GetNextPointerToRawData()
        {
            if (Sections.Count == 0) return Specific.HeaderSize;
            var last = (Sections[Sections.Count - 1] as SectionBase).Header;
            return last.PointerToRawData + last.SizeOfRawData;
        }

        public void Link(string output)
        {
            var text = AddSection(Text);
            AddSection(Data);
            if (!RData.IsEmtpy) AddSection(RData);
            if (!BSS.IsEmtpy) AddSection(BSS);
            AddSection(Import);
            PEHeader.TimeDateStamp = (uint)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
            Standard.EntryPoint = text.VirtualAddress;

            var fs = new FileStream(output, FileMode.Create);
            var bw = new BinaryWriter(fs, Encoding.ASCII);
            Write(bw);
            bw.Close();
            fs.Close();
        }

        public void Write(BinaryWriter bw)
        {
            PEHeader.NumberOfSections = (ushort)Sections.Count;
            Standard.InitializedDataSize = 0;
            Standard.UninitializedDataSize = 0;
            for (int i = 0; i < Sections.Count; i++)
            {
                var sb = Sections[i] as SectionBase;
                var sh = sb.Header;
                if ((sh.Characteristics & IMAGE_SCN.CNT_INITIALIZED_DATA) != 0)
                    Standard.InitializedDataSize += sh.VirtualSize;
                else if ((sh.Characteristics & IMAGE_SCN.CNT_UNINITIALIZED_DATA) != 0)
                    Standard.UninitializedDataSize += sh.VirtualSize;
            }
            Specific.ImageSize = GetNextVirtualAddress();
            Standard.CodeSize = Text.Header.VirtualSize;
            Standard.BaseOfCode = Text.Header.VirtualAddress;
            Standard.BaseOfData = Data.Header.VirtualAddress;
            DataDirs.ImportTable.Address = Import.Header.VirtualAddress;
            DataDirs.ImportTable.Size = Import.Header.VirtualSize;

            // write headers
            DOSHeader.Write(bw);
            SetPosition(bw, 0x3c);
            const int peSignPos = 0x80;
            bw.Write(peSignPos);
            WriteCodes(bw, DOSHeader.Stub);
            SetPosition(bw, peSignPos);
            WriteString(bw, "PE\0\0");
            PEHeader.Write(bw);
            Standard.Write(bw);
            Specific.Write(bw);
            DataDirs.Write(bw);
            for (int i = 0; i < Sections.Count; i++)
            {
                var sb = Sections[i] as SectionBase;
                sb.Header.Write(bw);
            }

            // write sections
            for (int i = 0; i < Sections.Count; i++)
            {
                var sb = Sections[i] as SectionBase;
                SetPosition(bw, sb.Header.PointerToRawData);
                var block = CreateBlock(sb);
                var bytes = block.ToByteArray();
                for (int j = 0; j < block.Relocations.Length; j++)
                {
                    var reloc = block.Relocations[j];
                    uint v = BitConverter.ToUInt32(bytes, (int)reloc);
                    Util.SetUInt(bytes, (int)reloc, v + Specific.ImageBase);
                    //Console.WriteLine("reloc[{0:X16}]{1:X16}", reloc, v);
                }
                bw.Write(bytes);
            }
            SetPosition(bw, GetNextPointerToRawData());
        }

        public static void SetPosition(BinaryWriter bw, uint offset)
        {
            uint len = offset - (uint)bw.BaseStream.Position;
            if (len > 0) bw.Write(new byte[len]);
        }

        public static void WriteCodes(BinaryWriter bw, OpCode[] ops)
        {
            for (int i = 0; i < ops.Length; i++)
            {
                bw.Write(ops[i].GetCodes());
            }
        }

        public static void WriteString(BinaryWriter bw, string s)
        {
            bw.Write(s.ToCharArray());
        }

        public static void WritePadString(BinaryWriter bw, int pad, string s)
        {
            WriteString(bw, s);
            int len = pad - s.Length;
            if (len > 0) bw.Write(new byte[len]);
        }

        public static uint Align(uint size, uint align)
        {
            return (size + align - 1) / align * align;
        }
    }
}
