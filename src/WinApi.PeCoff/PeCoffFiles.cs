// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace WinApi.PeCoff
{
    public static class PeCoffFiles
    {
        public static IMAGE_FILE_HEADER GetDllMachineType(string dllPath)
        {
            // See http://www.microsoft.com/whdc/system/platform/firmware/PECOFF.mspx Offset to PE
            // header is always at 0x3C. The PE header starts with "PE\0\0" = 0x50 0x45 0x00 0x00,
            // followed by a 2-byte machine type field (see the document above for the enum).
            var result = IMAGE_FILE_HEADER.Empty;

            FileStream stream = null;
            try
            {
                stream = new FileStream(dllPath, FileMode.Open, FileAccess.Read);
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new BinaryReader(stream))
                {
                    stream = null;

                    // PE Header starts @ 0x3C (60). Its a 4 byte header.
                    reader.BaseStream.Seek(0x3c, SeekOrigin.Begin);
                    var peHeaderOffset = reader.ReadInt32();
                    if (peHeaderOffset == 0)
                    {
                        peHeaderOffset = 0x80;
                    }

                    // Ensure there is at least enough room for the following structures: 24 byte PE
                    // Signature & Header 28 byte Standard Fields (24 bytes for PE32+) 68 byte NT Fields
                    // (88 bytes for PE32+) >= 128 byte Data Dictionary Table
                    if (peHeaderOffset > reader.BaseStream.Length - 256)
                    {
                        throw new BadImageFormatException("File either is not a PE/COFF file or is corrupted.", dllPath);
                    }

                    // Moving to PE Header start location...
                    reader.BaseStream.Seek(peHeaderOffset, SeekOrigin.Begin);
                    var peHeaderSignature = reader.ReadUInt32();

                    // Check the PE signature. Should equal 'PE\0\0'.
                    if (peHeaderSignature != 0x00004550)
                    {
                        throw new BadImageFormatException("Can't find PE header", dllPath);
                    }

                    result.Machine = (MachineType)reader.ReadUInt16();
                    result.NumberOfSections = reader.ReadUInt16();
                    var seconds = reader.ReadUInt32();
                    result.TimeDateStamp = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(seconds);
                    result.PointerToSymbolTable = reader.ReadUInt32();
                    result.NumberOfSymbols = reader.ReadUInt32();
                    result.SizeOfOptionalHeader = reader.ReadUInt16();
                    result.Characteristics = (Characteristics)reader.ReadUInt16();

                    // Now we are at the end of the PE Header and from here, the PE Optional Headers
                    // starts... Read PE magic number from Standard Fields to determine format.
                    result.PEFormat = (PEType)reader.ReadUInt16();
                    if (result.PEFormat != PEType.PE32 && result.PEFormat != PEType.PE32Plus)
                    {
                        throw new BadImageFormatException("Found neither PE nor PE+ magic numbers", dllPath);
                    }

                    // we'll increase the stream's current position to with 96 for PE headers and 112 for
                    // PE+ headers we want to skip these structures: 28 byte Standard Fields (24 bytes
                    // for PE32+) 68 byte NT Fields (88 bytes for PE32+)
                    var dataDictionaryStart = peHeaderOffset + (result.PEFormat == PEType.PE32 ? 96 : 112);

                    // To go directly to the datadictionary
                    reader.BaseStream.Position += dataDictionaryStart;

                    // DataDictionay has 16 directories in total, doing simple maths 128/16 = 8. So each
                    // directory is of 8 bytes. In this 8 bytes, 4 bytes is of RVA and 4 bytes of Size.
                    for (int i = 0; i < 15; i++)
                    {
                        result.DataDictionaryRVA[i] = reader.ReadUInt32();
                        result.DataDictionarySize[i] = reader.ReadUInt32();
                    }
                }
            }
            catch
            {
            }
            finally
            {
                stream?.Dispose();
            }

            return result;
        }

        // Returns true if the dll is Managed, false if it is native, and null if unknown
        public static bool? Is32BitManagedDll(string dllPath)
        {
            try
            {
                var pe = GetDllMachineType(dllPath);

                // The 15th directory consist of CLR header! if its 0, its not a CLR file :)
                if (pe.DataDictionaryRVA[14] == 0)
                {
                    return false;
                }

                return pe.PEFormat == PEType.PE32 && !pe.Characteristics.HasFlag(Characteristics.IMAGE_FILE_32BIT_MACHINE);
            }
            catch
            {
            }

            return null;
        }

        // Returns true if the dll is Managed, false if it is native, and null if unknown
        public static bool? Is64BitManagedDll(string dllPath)
        {
            try
            {
                var pe = GetDllMachineType(dllPath);

                // The 15th directory consist of CLR header! if its 0, its not a CLR file :)
                if (pe.DataDictionaryRVA[14] == 0)
                {
                    return false;
                }
                return pe.PEFormat == PEType.PE32Plus && !pe.Characteristics.HasFlag(Characteristics.IMAGE_FILE_32BIT_MACHINE);
            }
            catch
            {
            }

            return null;
        }

        // Returns true if the dll is Managed, false if it is native, and null if unknown
        public static bool? IsAnyCPUManagedDll(string dllPath)
        {
            try
            {
                var pe = GetDllMachineType(dllPath);

                // The 15th directory consist of CLR header! if its 0, its not a CLR file :)
                if (pe.DataDictionaryRVA[14] == 0)
                {
                    return false;
                }
                return pe.PEFormat == PEType.PE32 && !pe.Characteristics.HasFlag(Characteristics.IMAGE_FILE_32BIT_MACHINE);
            }
            catch
            {
            }

            return null;
        }

        // Returns true if the dll is Managed, false if it is native, and null if unknown
        public static bool? IsManagedDll(string dllPath)
        {
            try
            {
                var pe = GetDllMachineType(dllPath);

                // The 15th directory consist of CLR header! if its 0, its not a CLR file :)
                return pe.DataDictionaryRVA[14] != 0;
            }
            catch
            {
            }

            return null;
        }

        // Returns true if the dll is 64-bit, false if 32-bit, and null if unknown
        public static bool? IsUnmanagedDll64Bit(string dllPath)
        {
            try
            {
                var pe = GetDllMachineType(dllPath);
                switch (pe.Machine)
                {
                    case MachineType.IMAGE_FILE_MACHINE_AMD64:
                    case MachineType.IMAGE_FILE_MACHINE_IA64:
                        return true;

                    case MachineType.IMAGE_FILE_MACHINE_I386:
                        return false;

                    default:
                        return null;
                }
            }
            catch
            {
            }

            return null;
        }

        public static bool? IsDebugDLL(string dllPath, out bool pdbOnly)
        {
            pdbOnly = false;

            if (IsManagedDll(dllPath) ?? false)
            {
                try
                {
                    var result = false;
                    var assembly = Assembly.ReflectionOnlyLoadFrom(dllPath);
                    var attribs = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false);
                    bool IsJITOptimized = true;

                    // If the 'DebuggableAttribute' is not found then it is definitely an OPTIMIZED build
                    if (attribs.Length > 0)
                    {
                        // Just because the 'DebuggableAttribute' is found doesn't necessarily mean
                        // it's a DEBUG build; we have to check the JIT Optimization flag
                        // i.e. it could have the "generate PDB" checked but have JIT Optimization enabled
                        var debuggableAttribute = attribs[0] as DebuggableAttribute;
                        if (debuggableAttribute != null)
                        {
                            result = true;
                            IsJITOptimized = !debuggableAttribute.IsJITOptimizerDisabled;
                            result &= debuggableAttribute.IsJITOptimizerDisabled;

                            // check for Debug Output "full" or "pdb-only"
                            pdbOnly = !debuggableAttribute.DebuggingFlags.HasFlag(DebuggableAttribute.DebuggingModes.Default);
                        }
                    }
                }
                catch
                {
                }
            }

            return null;
        }
    }
}