using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PEImageDebugSampleLib {
    public class MapAndLoadedImage : IDisposable {
        private const uint IMAGE_NT_SIGNATURE = 0x00004550; // signature == "PE\0\0"
        private const uint IMAGE_FILE_MACHINE_I386 = 0x014c;
        private const uint IMAGE_FILE_MACHINE_AMD64 = 0x8664;
        private const uint IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10b;
        private const uint IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x20b;

        private readonly IntPtr _pLoadedImage;
        private readonly LOADED_IMAGE _loadedImage;
        private readonly IntPtr _pImageNtHeaders;
        private readonly IMAGE_NT_HEADERS _imageNtHeaders;
        private readonly IMAGE_NT_HEADERS64 _imageNtHeaders64;
        private readonly int _lastRvaSectionOffset;
        private readonly bool _isAmd64Image;

        public bool IsAmd64Image {
            get { return _isAmd64Image; }
        }

        private bool _isDisposed = false;

        public MapAndLoadedImage(string fileName) {
            _pLoadedImage = IntPtr.Zero;
            _loadedImage = default(LOADED_IMAGE);
            _imageNtHeaders = default(IMAGE_NT_HEADERS);
            _imageNtHeaders64 = default(IMAGE_NT_HEADERS64);
            _isAmd64Image = false;

            _lastRvaSectionOffset = Marshal.OffsetOf(typeof(LOADED_IMAGE), "LastRvaSection").ToInt32();

            IntPtr pLoadedImage = IntPtr.Zero;
            LOADED_IMAGE loadedImage = default(LOADED_IMAGE);
            IntPtr pImageNtHeaders = IntPtr.Zero;
            IMAGE_NT_HEADERS imageNtHeaders = default(IMAGE_NT_HEADERS);
            IMAGE_NT_HEADERS64 imageNtHeaders64 = default(IMAGE_NT_HEADERS64);
            try {
                pLoadedImage = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LOADED_IMAGE)));
                bool fr;
                fr = NativeMethods.MapAndLoad(fileName, null, pLoadedImage, false, true);
                if (!fr) {
                    int error = Marshal.GetLastWin32Error();
                    Marshal.FreeHGlobal(pLoadedImage);
                    pLoadedImage = IntPtr.Zero;
                    throw new Win32Exception(error);
                }
                loadedImage = (LOADED_IMAGE)Marshal.PtrToStructure(_pLoadedImage, typeof(LOADED_IMAGE));

                IntPtr pNtHeaders = NativeMethods.ImageNtHeader(loadedImage.MappedAddress);
                IMAGE_NT_HEADERS ntHeaders = (IMAGE_NT_HEADERS)Marshal.PtrToStructure(pNtHeaders, typeof(IMAGE_NT_HEADERS));
                if (ntHeaders.Signature != IMAGE_NT_SIGNATURE) { // signature == "PE\0\0"
                    throw new ArgumentException();
                }
                if (ntHeaders.FileHeader.Machine == IMAGE_FILE_MACHINE_I386) {
                    _isAmd64Image = false;
                    imageNtHeaders = ntHeaders;
                    imageNtHeaders64 = default(IMAGE_NT_HEADERS64);
                    if (imageNtHeaders.OptionalHeader.Magic != IMAGE_NT_OPTIONAL_HDR32_MAGIC) {
                        throw new ArgumentException();
                    }
                } else if (ntHeaders.FileHeader.Machine == IMAGE_FILE_MACHINE_I386) {
                    _isAmd64Image = true;
                    imageNtHeaders = default(IMAGE_NT_HEADERS);
                    imageNtHeaders64 = (IMAGE_NT_HEADERS64)Marshal.PtrToStructure(pNtHeaders, typeof(IMAGE_NT_HEADERS64));
                    if (imageNtHeaders64.OptionalHeader.Magic != IMAGE_NT_OPTIONAL_HDR64_MAGIC) {
                        throw new ArgumentException();
                    }
                } else {
                    throw new ArgumentException();
                }
            } catch {
                if (pLoadedImage != IntPtr.Zero) {
                    NativeMethods.UnMapAndLoad(pLoadedImage);
                    Marshal.FreeHGlobal(pLoadedImage);
                    pLoadedImage = IntPtr.Zero;
                }
                throw;
            }

            _pLoadedImage = pLoadedImage;
            _loadedImage = loadedImage;
            _pImageNtHeaders = pImageNtHeaders;
            _imageNtHeaders = imageNtHeaders;
            _imageNtHeaders64 = imageNtHeaders64;
        }

        public IntPtr ImageDirectoryEntryToDataEx(UInt16 DirectoryEntry, out UInt32 size, out IntPtr foundHeader) {
            IntPtr result = NativeMethods.ImageDirectoryEntryToDataEx(_loadedImage.MappedAddress, false, DirectoryEntry, out size, out foundHeader);
            if (result == IntPtr.Zero) {
            }
            return result;
        }

        public T? ImageDirectoryEntryToDataEx<T>(UInt16 DirectoryEntry, out UInt32 size, out IntPtr foundHeader) where T: struct {
            IntPtr pResult = ImageDirectoryEntryToDataEx(DirectoryEntry, out size, out foundHeader);
            if (pResult == IntPtr.Zero) {
                return null;
            }
            T result = (T)Marshal.PtrToStructure(pResult, typeof(T));
            return result;
        }

        public IntPtr RvaToVa(uint virtualAddress) {
            IntPtr ppLastRvaSection = _pLoadedImage;
            ppLastRvaSection += _lastRvaSectionOffset;

            IntPtr result = NativeMethods.ImageRvaToVa(_pImageNtHeaders, _loadedImage.MappedAddress, virtualAddress, ppLastRvaSection);
            if (result == IntPtr.Zero) {
                int error = Marshal.GetLastWin32Error();
                if (error == 0) {
                    return result;
                }
                throw new Win32Exception(error, "error ImageRvaToVa.");
            }
            return result;
        }

        public void Dispose() {
            if (_isDisposed) {
                return;
            }
            NativeMethods.UnMapAndLoad(_pLoadedImage);
            Marshal.FreeHGlobal(_pLoadedImage);
            _isDisposed = true;
        }
    }
}