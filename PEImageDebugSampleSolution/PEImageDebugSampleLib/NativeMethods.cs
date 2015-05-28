using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PEImageDebugSampleLib {
    internal static class NativeMethods {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("Imagehlp.dll", CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
        public static extern bool MapAndLoad(
            [MarshalAs(UnmanagedType.LPStr)]string ImageName,
            [MarshalAs(UnmanagedType.LPStr)]string DllPath,
            [MarshalAs(UnmanagedType.SysInt)]IntPtr LoadedImage,
            [MarshalAs(UnmanagedType.Bool)]bool DotDll,
            [MarshalAs(UnmanagedType.Bool)]bool ReadOnly
            );

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("Imagehlp.dll", CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
        public static extern bool UnMapAndLoad(
            [MarshalAs(UnmanagedType.SysInt)]IntPtr LoadedImage
            );

        [return: MarshalAs(UnmanagedType.SysInt)]
        [DllImport("Dbghelp.dll", CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
        public static extern IntPtr ImageRvaToVa(
            [MarshalAs(UnmanagedType.SysInt)]IntPtr NtHeaders,
            [MarshalAs(UnmanagedType.SysInt)]IntPtr Base,
            [MarshalAs(UnmanagedType.U4)]UInt32 Rva,
            [MarshalAs(UnmanagedType.SysInt)]IntPtr LastRvaSection
            );

        [return: MarshalAs(UnmanagedType.SysInt)]
        [DllImport("Dbghelp.dll", CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
        public static extern IntPtr ImageNtHeader(
            [MarshalAs(UnmanagedType.SysInt)]IntPtr ImageBase
            );

        [return: MarshalAs(UnmanagedType.SysInt)]
        [DllImport("Dbghelp.dll", CharSet = CharSet.Unicode, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
        public static extern IntPtr ImageDirectoryEntryToDataEx(
            [MarshalAs(UnmanagedType.SysInt)]IntPtr Base,
            [MarshalAs(UnmanagedType.Bool)]bool MappedAsImage,
            [MarshalAs(UnmanagedType.U4)]UInt32 DirectoryEntry,
            [MarshalAs(UnmanagedType.U4)]out UInt32 Size,
            [MarshalAs(UnmanagedType.SysInt)]out IntPtr FoundHeader
            );
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct LOADED_IMAGE {
        //[MarshalAs(UnmanagedType.LPTStr)]
        //public string ModuleName;
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr ModuleName;
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr hFile;
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr MappedAddress;
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr FileHeader;
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr LastRvaSection;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 NumberOfSections;
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr Sections;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Characteristics;
        [MarshalAs(UnmanagedType.U1)]
        public byte fSystemImage;
        [MarshalAs(UnmanagedType.U1)]
        public byte fDOSImage;
        [MarshalAs(UnmanagedType.U1)]
        public byte fReadOnly;
        [MarshalAs(UnmanagedType.U1)]
        public byte Version;
        [MarshalAs(UnmanagedType.Struct)]
        public LIST_ENTRY Links;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfImage;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct IMAGE_NT_HEADERS {
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Signature;
        [MarshalAs(UnmanagedType.Struct)]
        public IMAGE_FILE_HEADER FileHeader;
        [MarshalAs(UnmanagedType.Struct)]
        public IMAGE_OPTIONAL_HEADER OptionalHeader;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct IMAGE_NT_HEADERS64 {
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Signature;
        [MarshalAs(UnmanagedType.Struct)]
        public IMAGE_FILE_HEADER FileHeader;
        [MarshalAs(UnmanagedType.Struct)]
        public IMAGE_OPTIONAL_HEADER64 OptionalHeader;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct IMAGE_FILE_HEADER {
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 Machine;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberOfSections;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 TimeDateStamp;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 PointerToSymbolTable;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 NumberOfSymbols;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 SizeOfOptionalHeader;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 Characteristics;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct IMAGE_OPTIONAL_HEADER {
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 Magic;
        [MarshalAs(UnmanagedType.U1)]
        public byte MajorLinkerVersion;
        [MarshalAs(UnmanagedType.U1)]
        public byte MinorLinkerVersion;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfCode;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfInitializedData;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfUninitializedData;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 AddressOfEntryPoint;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 BaseOfCode;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 BaseOfData;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 ImageBase;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SectionAlignment;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 FileAlignment;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 MajorOperatingSystemVersion;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 MinorOperatingSystemVersion;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 MajorImageVersion;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 MinorImageVersion;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 MajorSubsystemVersion;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 MinorSubsystemVersion;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Win32VersionValue;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfImage;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfHeaders;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 CheckSum;
        [MarshalAs(UnmanagedType.U2)]
        public ImageSubsystem Subsystem;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 DllCharacteristics;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfStackReserve;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfStackCommit;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfHeapReserve;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfHeapCommit;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 LoaderFlags;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 NumberOfRvaAndSizes;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 16/*IMAGE_NUMBEROF_DIRECTORY_ENTRIES*/)]
        public IMAGE_DATA_DIRECTORY[] DataDirectory;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct IMAGE_OPTIONAL_HEADER64 {
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 Magic;
        [MarshalAs(UnmanagedType.U1)]
        public byte MajorLinkerVersion;
        [MarshalAs(UnmanagedType.U1)]
        public byte MinorLinkerVersion;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfCode;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfInitializedData;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfUninitializedData;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 AddressOfEntryPoint;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 BaseOfCode;
        [MarshalAs(UnmanagedType.U8)]
        public UInt64 ImageBase;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SectionAlignment;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 FileAlignment;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 MajorOperatingSystemVersion;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 MinorOperatingSystemVersion;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 MajorImageVersion;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 MinorImageVersion;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 MajorSubsystemVersion;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 MinorSubsystemVersion;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Win32VersionValue;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfImage;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 SizeOfHeaders;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 CheckSum;
        [MarshalAs(UnmanagedType.U2)]
        public ImageSubsystem Subsystem;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 DllCharacteristics;
        [MarshalAs(UnmanagedType.U8)]
        public UInt64 SizeOfStackReserve;
        [MarshalAs(UnmanagedType.U8)]
        public UInt64 SizeOfStackCommit;
        [MarshalAs(UnmanagedType.U8)]
        public UInt64 SizeOfHeapReserve;
        [MarshalAs(UnmanagedType.U8)]
        public UInt64 SizeOfHeapCommit;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 LoaderFlags;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 NumberOfRvaAndSizes;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 16/* IMAGE_NUMBEROF_DIRECTORY_ENTRIES */)]
        public IMAGE_DATA_DIRECTORY[] DataDirectory;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    internal struct IMAGE_SECTION_HEADER {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8/* IMAGE_SIZEOF_SHORT_NAME */)]
        [FieldOffset(0)]
        public byte[] Name;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(8)]
        public UInt32 PhysicalAddress;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(8)]
        public UInt32 VirtualSize;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(12)]
        public UInt32 VirtualAddress;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(16)]
        public UInt32 SizeOfRawData;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(20)]
        public UInt32 PointerToRawData;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(24)]
        public UInt32 PointerToRelocations;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(28)]
        public UInt32 PointerToLinenumbers;
        [MarshalAs(UnmanagedType.U2)]
        [FieldOffset(32)]
        public UInt16 NumberOfRelocations;
        [MarshalAs(UnmanagedType.U2)]
        [FieldOffset(34)]
        public UInt16 NumberOfLinenumbers;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(36)]
        public UInt32 Characteristics;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    internal struct IMAGE_COR20_HEADER {
        // Header versioning
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(0)]
        public UInt32 cb;
        [MarshalAs(UnmanagedType.U2)]
        [FieldOffset(4)]
        public UInt16 MajorRuntimeVersion;
        [MarshalAs(UnmanagedType.U2)]
        [FieldOffset(6)]
        public UInt16 MinorRuntimeVersion;

        // Symbol table and startup information
        [MarshalAs(UnmanagedType.Struct)]
        [FieldOffset(8)]
        public IMAGE_DATA_DIRECTORY MetaData;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(16)]
        public UInt32 Flags;

        // The main program if it is an EXE (not used if a DLL?)
        // If COMIMAGE_FLAGS_NATIVE_ENTRYPOINT is not set, EntryPointToken represents a managed entrypoint.
        // If COMIMAGE_FLAGS_NATIVE_ENTRYPOINT is set, EntryPointRVA represents an RVA to a native entrypoint
        // (depricated for DLLs, use modules constructors intead). 
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(20)]
        public UInt32 EntryPointToken;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(20)]
        public UInt32 EntryPointRVA;

        // This is the blob of managed resources. Fetched using code:AssemblyNative.GetResource and
        // code:PEFile.GetResource and accessible from managed code from
        // System.Assembly.GetManifestResourceStream.  The meta data has a table that maps names to offsets into
        // this blob, so logically the blob is a set of resources. 
        [MarshalAs(UnmanagedType.Struct)]
        [FieldOffset(24)]
        public IMAGE_DATA_DIRECTORY Resources;
        // IL assemblies can be signed with a public-private key to validate who created it.  The signature goes
        // here if this feature is used. 
        [MarshalAs(UnmanagedType.Struct)]
        [FieldOffset(32)]
        public IMAGE_DATA_DIRECTORY StrongNameSignature;

        [MarshalAs(UnmanagedType.Struct)]
        [FieldOffset(40)]
        public IMAGE_DATA_DIRECTORY CodeManagerTable;			// Depricated, not used 
        // Used for manged codee that has unmaanaged code inside it (or exports methods as unmanaged entry points)
        [MarshalAs(UnmanagedType.Struct)]
        [FieldOffset(48)]
        public IMAGE_DATA_DIRECTORY VTableFixups;
        [MarshalAs(UnmanagedType.Struct)]
        [FieldOffset(56)]
        public IMAGE_DATA_DIRECTORY ExportAddressTableJumps;

        // null for ordinary IL images.  NGEN images it points at a code:CORCOMPILE_HEADER structure
        [MarshalAs(UnmanagedType.Struct)]
        [FieldOffset(64)]
        public IMAGE_DATA_DIRECTORY ManagedNativeHeader;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct IMAGE_DATA_DIRECTORY {
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 VirtualAddress;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Size;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct LIST_ENTRY {
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr Flink;
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr Blink;
    }

    internal enum ImageSubsystem : ushort {
        IMAGE_SUBSYSTEM_UNKNOWN = 0,
        IMAGE_SUBSYSTEM_NATIVE = 1,
        IMAGE_SUBSYSTEM_WINDOWS_GUI = 2,
        IMAGE_SUBSYSTEM_WINDOWS_CUI = 3,
        IMAGE_SUBSYSTEM_OS2_CUI = 5,
        IMAGE_SUBSYSTEM_POSIX_CUI = 7,
        IMAGE_SUBSYSTEM_WINDOWS_CE_GUI = 9,
        IMAGE_SUBSYSTEM_EFI_APPLICATION = 10,
        IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER = 11,
        IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER = 12,
        IMAGE_SUBSYSTEM_EFI_ROM = 13,
        IMAGE_SUBSYSTEM_XBOX = 14,
        IMAGE_SUBSYSTEM_WINDOWS_BOOT_APPLICATION = 16,
    }
}
