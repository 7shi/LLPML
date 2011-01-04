using System;
using Girl.Binary;

namespace Girl.PE
{
    public class IMAGE_SUBSYSTEM
    {
        public const ushort WINDOWS_GUI = 2;
        public const ushort WINDOWS_CUI = 3;
        public const ushort WINCE_GUI = 9;
    }

    public class IMAGE_SCN
    {
        public const uint CNT_CODE = 0x00000020;
        public const uint CNT_INITIALIZED_DATA = 0x00000040;
        public const uint CNT_UNINITIALIZED_DATA = 0x00000080;
        public const uint MEM_EXECUTE = 0x20000000;
        public const uint MEM_READ = 0x40000000;
        public const uint MEM_WRITE = 0x80000000;
    }

    public class PEFileHeader : HeaderBase
    {
        public ushort Machine = 0x14c;
        public ushort NumberOfSections;
        public uint TimeDateStamp = 0;
        public uint PointerToSymbolTable = 0;
        public uint NumberOfSymbols = 0;
        public ushort OptionalHeaderSize = 0xe0;
        public ushort Characteristics = 0x10e;

        public override void WriteBlock(Block32 block)
        {
            block.AddUShort(Machine);
            block.AddUShort(NumberOfSections);
            block.AddUInt(TimeDateStamp);
            block.AddUInt(PointerToSymbolTable);
            block.AddUInt(NumberOfSymbols);
            block.AddUShort(OptionalHeaderSize);
            block.AddUShort(Characteristics);
        }
    }

    public class PEHeaderStandardFields : HeaderBase
    {
        public ushort Magic = 0x10b;
        public byte LMajor = 6;
        public byte LMinor = 0;
        public uint CodeSize;
        public uint InitializedDataSize;
        public uint UninitializedDataSize;
        public uint EntryPoint;
        public uint BaseOfCode;
        public uint BaseOfData;

        public override void WriteBlock(Block32 block)
        {
            block.AddUShort(Magic);
            block.AddByte(LMajor);
            block.AddByte(LMinor);
            block.AddUInt(CodeSize);
            block.AddUInt(InitializedDataSize);
            block.AddUInt(UninitializedDataSize);
            block.AddUInt(EntryPoint);
            block.AddUInt(BaseOfCode);
            block.AddUInt(BaseOfData);
        }
    }

    public class PEHeaderWindowsNTSpecificFields : HeaderBase
    {
        public uint ImageBase = 0x400000;
        public uint SectionAlignment = 0x2000;
        public uint FileAlignment = 0x200;
        public ushort OSMajor = 4;
        public ushort OSMinor = 0;
        public ushort UserMajor = 0;
        public ushort UserMinor = 0;
        public ushort SubSysMajor = 4;
        public ushort SubSysMinor = 0;
        public uint Reserved = 0;
        public uint ImageSize;
        public uint HeaderSize = 0x400;
        public uint FileChecksum = 0;
        public ushort SubSystem = 3;
        public ushort DLLFlags = 0;
        public uint StackReserveSize = 0x100000;
        public uint StackCommitSize = 0x1000;
        public uint HeapReserveSize = 0x100000;
        public uint HeapCommitSize = 0x1000;
        public uint LoaderFlags = 0;
        public uint NumberOfDataDirectories = 0x10;

        public override void WriteBlock(Block32 block)
        {
            block.AddUInt(ImageBase);
            block.AddUInt(SectionAlignment);
            block.AddUInt(FileAlignment);
            block.AddUShort(OSMajor);
            block.AddUShort(OSMinor);
            block.AddUShort(UserMajor);
            block.AddUShort(UserMinor);
            block.AddUShort(SubSysMajor);
            block.AddUShort(SubSysMinor);
            block.AddUInt(Reserved);
            block.AddUInt(ImageSize);
            block.AddUInt(HeaderSize);
            block.AddUInt(FileChecksum);
            block.AddUShort(SubSystem);
            block.AddUShort(DLLFlags);
            block.AddUInt(StackReserveSize);
            block.AddUInt(StackCommitSize);
            block.AddUInt(HeapReserveSize);
            block.AddUInt(HeapCommitSize);
            block.AddUInt(LoaderFlags);
            block.AddUInt(NumberOfDataDirectories);
        }
    }

    public class PEHeaderDataDirectories : HeaderBase
    {
        public Table ExportTable;
        public Table ImportTable;
        public Table ResourceTable;
        public Table ExceptionTable;
        public Table CertificateTable;
        public Table BaseRelocationTable;
        public Table Debug;
        public Table Copyright;
        public Table GlobalPtr;
        public Table TLSTable;
        public Table LoadConfigTable;
        public Table BoundImport;
        public Table IAT;
        public Table DelayImportDescriptor;
        public Table CLIHeader;
        public Table Reserved;

        public override void WriteBlock(Block32 block)
        {
            ExportTable.WriteBlock(block);
            ImportTable.WriteBlock(block);
            ResourceTable.WriteBlock(block);
            ExceptionTable.WriteBlock(block);
            CertificateTable.WriteBlock(block);
            BaseRelocationTable.WriteBlock(block);
            Debug.WriteBlock(block);
            Copyright.WriteBlock(block);
            GlobalPtr.WriteBlock(block);
            TLSTable.WriteBlock(block);
            LoadConfigTable.WriteBlock(block);
            BoundImport.WriteBlock(block);
            IAT.WriteBlock(block);
            DelayImportDescriptor.WriteBlock(block);
            CLIHeader.WriteBlock(block);
            Reserved.WriteBlock(block);
        }
    }

    public class SectionHeader : HeaderBase
    {
        private string name = "";
        public string Name
        {
            get { return Trim(name); }
            set { name = Pad(8, value); }
        }

        public uint VirtualSize;
        public uint VirtualAddress;
        public uint SizeOfRawData = 0;
        public uint PointerToRawData = 0;
        public uint PointerToRelocations = 0;
        public uint PointerToLinenumbers = 0;
        public ushort NumberOfRelocations = 0;
        public ushort NumberOfLinenumbers = 0;
        public uint Characteristics;

        public override void WriteBlock(Block32 block)
        {
            block.AddString(name);
            block.AddUInt(VirtualSize);
            block.AddUInt(VirtualAddress);
            block.AddUInt(SizeOfRawData);
            block.AddUInt(PointerToRawData);
            block.AddUInt(PointerToRelocations);
            block.AddUInt(PointerToLinenumbers);
            block.AddUShort(NumberOfRelocations);
            block.AddUShort(NumberOfLinenumbers);
            block.AddUInt(Characteristics);
        }
    }

    public class ImportTable : HeaderBase
    {
        public uint ImportLookupTable = 0;
        public uint DateTimeStamp = 0;
        public uint ForwarderChain = 0;
        public uint Name = 0;
        public uint ImportAddressTable = 0;

        public override void WriteBlock(Block32 block)
        {
            block.AddUInt(ImportLookupTable);
            block.AddUInt(DateTimeStamp);
            block.AddUInt(ForwarderChain);
            block.AddUInt(Name);
            block.AddUInt(ImportAddressTable);
        }
    }
}
