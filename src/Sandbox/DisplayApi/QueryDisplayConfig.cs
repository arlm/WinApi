// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace Sandbox
{
    public enum D3DKMDT_VIDEO_SIGNAL_STANDARD
    {
        // D3DKMDT_VSS_UNINITIALIZED -> 0
        D3DKMDT_VSS_UNINITIALIZED = 0,

        // D3DKMDT_VSS_VESA_DMT -> 1
        D3DKMDT_VSS_VESA_DMT = 1,

        // D3DKMDT_VSS_VESA_GTF -> 2
        D3DKMDT_VSS_VESA_GTF = 2,

        // D3DKMDT_VSS_VESA_CVT -> 3
        D3DKMDT_VSS_VESA_CVT = 3,

        // D3DKMDT_VSS_IBM -> 4
        D3DKMDT_VSS_IBM = 4,

        // D3DKMDT_VSS_APPLE -> 5
        D3DKMDT_VSS_APPLE = 5,

        // D3DKMDT_VSS_NTSC_M -> 6
        D3DKMDT_VSS_NTSC_M = 6,

        // D3DKMDT_VSS_NTSC_J -> 7
        D3DKMDT_VSS_NTSC_J = 7,

        // D3DKMDT_VSS_NTSC_443 -> 8
        D3DKMDT_VSS_NTSC_443 = 8,

        // D3DKMDT_VSS_PAL_B -> 9
        D3DKMDT_VSS_PAL_B = 9,

        // D3DKMDT_VSS_PAL_B1 -> 10
        D3DKMDT_VSS_PAL_B1 = 10,

        // D3DKMDT_VSS_PAL_G -> 11
        D3DKMDT_VSS_PAL_G = 11,

        // D3DKMDT_VSS_PAL_H -> 12
        D3DKMDT_VSS_PAL_H = 12,

        // D3DKMDT_VSS_PAL_I -> 13
        D3DKMDT_VSS_PAL_I = 13,

        // D3DKMDT_VSS_PAL_D -> 14
        D3DKMDT_VSS_PAL_D = 14,

        // D3DKMDT_VSS_PAL_N -> 15
        D3DKMDT_VSS_PAL_N = 15,

        // D3DKMDT_VSS_PAL_NC -> 16
        D3DKMDT_VSS_PAL_NC = 16,

        // D3DKMDT_VSS_SECAM_B -> 17
        D3DKMDT_VSS_SECAM_B = 17,

        // D3DKMDT_VSS_SECAM_D -> 18
        D3DKMDT_VSS_SECAM_D = 18,

        // D3DKMDT_VSS_SECAM_G -> 19
        D3DKMDT_VSS_SECAM_G = 19,

        // D3DKMDT_VSS_SECAM_H -> 20
        D3DKMDT_VSS_SECAM_H = 20,

        // D3DKMDT_VSS_SECAM_K -> 21
        D3DKMDT_VSS_SECAM_K = 21,

        // D3DKMDT_VSS_SECAM_K1 -> 22
        D3DKMDT_VSS_SECAM_K1 = 22,

        // D3DKMDT_VSS_SECAM_L -> 23
        D3DKMDT_VSS_SECAM_L = 23,

        // D3DKMDT_VSS_SECAM_L1 -> 24
        D3DKMDT_VSS_SECAM_L1 = 24,

        // D3DKMDT_VSS_EIA_861 -> 25
        D3DKMDT_VSS_EIA_861 = 25,

        // D3DKMDT_VSS_EIA_861A -> 26
        D3DKMDT_VSS_EIA_861A = 26,

        // D3DKMDT_VSS_EIA_861B -> 27
        D3DKMDT_VSS_EIA_861B = 27,

        // D3DKMDT_VSS_PAL_K -> 28
        D3DKMDT_VSS_PAL_K = 28,

        // D3DKMDT_VSS_PAL_K1 -> 29
        D3DKMDT_VSS_PAL_K1 = 29,

        // D3DKMDT_VSS_PAL_L -> 30
        D3DKMDT_VSS_PAL_L = 30,

        // D3DKMDT_VSS_PAL_M -> 31
        D3DKMDT_VSS_PAL_M = 31,

        // D3DKMDT_VSS_OTHER -> 255
        D3DKMDT_VSS_OTHER = 255,
    }

    public enum DISPLAYCONFIG_MODE_INFO_TYPE
    {
        // DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE -> 1
        DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,

        // DISPLAYCONFIG_MODE_INFO_TYPE_TARGET -> 2
        DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,

        // DISPLAYCONFIG_MODE_INFO_TYPE_DESKTOP_IMAGE -> 3
        DISPLAYCONFIG_MODE_INFO_TYPE_DESKTOP_IMAGE = 3,

        // DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32 -> 0xFFFFFFFF
        DISPLAYCONFIG_MODE_INFO_TYPE_FORCE_UINT32 = -1,
    }

    public enum DISPLAYCONFIG_PIXELFORMAT
    {
        // DISPLAYCONFIG_PIXELFORMAT_8BPP -> 1
        DISPLAYCONFIG_PIXELFORMAT_8BPP = 1,

        // DISPLAYCONFIG_PIXELFORMAT_16BPP -> 2
        DISPLAYCONFIG_PIXELFORMAT_16BPP = 2,

        // DISPLAYCONFIG_PIXELFORMAT_24BPP -> 3
        DISPLAYCONFIG_PIXELFORMAT_24BPP = 3,

        // DISPLAYCONFIG_PIXELFORMAT_32BPP -> 4
        DISPLAYCONFIG_PIXELFORMAT_32BPP = 4,

        // DISPLAYCONFIG_PIXELFORMAT_NONGDI -> 5
        DISPLAYCONFIG_PIXELFORMAT_NONGDI = 5,

        // DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32 -> 0xffffffff
        DISPLAYCONFIG_PIXELFORMAT_FORCE_UINT32 = -1,
    }

    public enum DISPLAYCONFIG_ROTATION
    {
        // DISPLAYCONFIG_ROTATION_IDENTITY -> 1
        DISPLAYCONFIG_ROTATION_IDENTITY = 1,

        // DISPLAYCONFIG_ROTATION_ROTATE90 -> 2
        DISPLAYCONFIG_ROTATION_ROTATE90 = 2,

        // DISPLAYCONFIG_ROTATION_ROTATE180 -> 3
        DISPLAYCONFIG_ROTATION_ROTATE180 = 3,

        // DISPLAYCONFIG_ROTATION_ROTATE270 -> 4
        DISPLAYCONFIG_ROTATION_ROTATE270 = 4,

        // DISPLAYCONFIG_ROTATION_FORCE_UINT32 -> 0xFFFFFFFF
        DISPLAYCONFIG_ROTATION_FORCE_UINT32 = -1,
    }

    public enum DISPLAYCONFIG_SCALING
    {
        // DISPLAYCONFIG_SCALING_IDENTITY -> 1
        DISPLAYCONFIG_SCALING_IDENTITY = 1,

        // DISPLAYCONFIG_SCALING_CENTERED -> 2
        DISPLAYCONFIG_SCALING_CENTERED = 2,

        // DISPLAYCONFIG_SCALING_STRETCHED -> 3
        DISPLAYCONFIG_SCALING_STRETCHED = 3,

        // DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX -> 4
        DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,

        // DISPLAYCONFIG_SCALING_CUSTOM -> 5
        DISPLAYCONFIG_SCALING_CUSTOM = 5,

        // DISPLAYCONFIG_SCALING_PREFERRED -> 128
        DISPLAYCONFIG_SCALING_PREFERRED = 128,

        // DISPLAYCONFIG_SCALING_FORCE_UINT32 -> 0xFFFFFFFF
        DISPLAYCONFIG_SCALING_FORCE_UINT32 = -1,
    }

    public enum DISPLAYCONFIG_SCANLINE_ORDERING
    {
        // DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED -> 0
        DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0,

        // DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE -> 1
        DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1,

        // DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED -> 2
        DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2,

        // DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST -> DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED
        DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED,

        // DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST -> 3
        DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3,

        // DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32 -> 0xFFFFFFFF
        DISPLAYCONFIG_SCANLINE_ORDERING_FORCE_UINT32 = -1,
    }

    public enum DISPLAYCONFIG_TOPOLOGY_ID
    {
        // DISPLAYCONFIG_TOPOLOGY_INTERNAL -> 0x00000001
        DISPLAYCONFIG_TOPOLOGY_INTERNAL = 1,

        // DISPLAYCONFIG_TOPOLOGY_CLONE -> 0x00000002
        DISPLAYCONFIG_TOPOLOGY_CLONE = 2,

        // DISPLAYCONFIG_TOPOLOGY_EXTEND -> 0x00000004
        DISPLAYCONFIG_TOPOLOGY_EXTEND = 4,

        // DISPLAYCONFIG_TOPOLOGY_EXTERNAL -> 0x00000008
        DISPLAYCONFIG_TOPOLOGY_EXTERNAL = 8,

        // DISPLAYCONFIG_TOPOLOGY_FORCE_UINT32 -> 0xFFFFFFFF
        DISPLAYCONFIG_TOPOLOGY_FORCE_UINT32 = -1,
    }

    public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY
    {
        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER -> -1
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = -1,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 -> 0
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO -> 1
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO -> 2
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO -> 3
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI -> 4
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI -> 5
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS -> 6
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN -> 8
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI -> 9
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL -> 10
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED -> 11
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL -> 12
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED -> 13
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE -> 14
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST -> 15
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL -> 0x80000000
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INTERNAL = -2147483648,

        // DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32 -> 0xFFFFFFFF
        DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCE_UINT32 = -1,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Anonymous_17ea295e_ab4c_4636_977f_5f219c859b79
    {
        // Anonymous_638f97c6_55d4_4922_9bd6_5275b62d0caf
        [FieldOffset(0)]
        public Anonymous_638f97c6_55d4_4922_9bd6_5275b62d0caf AdditionalSignalInfo;

        // UINT32->unsigned int
        [FieldOffset(0)]
        public uint videoStandard;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Anonymous_4be23a3d_1961_462d_ab0c_22a3cfa7a5d3
    {
        // UINT32->unsigned int
        [FieldOffset(0)]
        public uint modeInfoIdx;

        // Anonymous_e1c1a519_feb7_4276_90e6_279499df2bb4
        [FieldOffset(0)]
        public Anonymous_e1c1a519_feb7_4276_90e6_279499df2bb4 DUMMYSTRUCTNAME;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Anonymous_638f97c6_55d4_4922_9bd6_5275b62d0caf
    {
        // videoStandard : 16
        //vSyncFreqDivider : 6
        //reserved : 10
        public uint bitvector1;

        public uint videoStandard
        {
            get
            {
                return this.bitvector1 & 65535u;
            }

            set
            {
                this.bitvector1 = value | this.bitvector1;
            }
        }

        public uint vSyncFreqDivider
        {
            get
            {
                return (this.bitvector1 & 4128768u) / 65536;
            }

            set
            {
                this.bitvector1 = (value * 65536) | this.bitvector1;
            }
        }

        public uint reserved
        {
            get
            {
                return (this.bitvector1 & 4290772992u) / 4194304;
            }

            set
            {
                this.bitvector1 = (value * 4194304) | this.bitvector1;
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Anonymous_9a90a686_b6cf_4bda_bc00_c40cd73b5a54
    {
        // UINT32->unsigned int
        [FieldOffset(0)]
        public uint modeInfoIdx;

        // Anonymous_cd659137_af82_4d73_a4b4_7537126520b6
        [FieldOffset(0)]
        public Anonymous_cd659137_af82_4d73_a4b4_7537126520b6 Struct1;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Anonymous_cd659137_af82_4d73_a4b4_7537126520b6
    {
        // desktopModeInfoIdx : 16
        //targetModeInfoIdx : 16
        public uint bitvector1;

        public uint desktopModeInfoIdx
        {
            get
            {
                return this.bitvector1 & 65535u;
            }

            set
            {
                this.bitvector1 = value | this.bitvector1;
            }
        }

        public uint targetModeInfoIdx
        {
            get
            {
                return (this.bitvector1 & 4294901760u) / 65536;
            }

            set
            {
                this.bitvector1 = (value * 65536) | this.bitvector1;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Anonymous_e1c1a519_feb7_4276_90e6_279499df2bb4
    {
        // cloneGroupId : 16
        //sourceModeInfoIdx : 16
        public uint bitvector1;

        public uint cloneGroupId
        {
            get
            {
                return this.bitvector1 & 65535u;
            }

            set
            {
                this.bitvector1 = value | this.bitvector1;
            }
        }

        public uint sourceModeInfoIdx
        {
            get
            {
                return (this.bitvector1 & 4294901760u) / 65536;
            }

            set
            {
                this.bitvector1 = (value * 65536) | this.bitvector1;
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Anonymous_fa905f1f_70a2_4b5b_8df3_0074ac1037b8
    {
        // DISPLAYCONFIG_TARGET_MODE
        [FieldOffset(0)]
        public DISPLAYCONFIG_TARGET_MODE targetMode;

        // DISPLAYCONFIG_SOURCE_MODE
        [FieldOffset(0)]
        public DISPLAYCONFIG_SOURCE_MODE sourceMode;

        // DISPLAYCONFIG_DESKTOP_IMAGE_INFO
        [FieldOffset(0)]
        public DISPLAYCONFIG_DESKTOP_IMAGE_INFO desktopImageInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_2DREGION
    {
        // UINT32->unsigned int
        public uint cx;

        // UINT32->unsigned int
        public uint cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_DESKTOP_IMAGE_INFO
    {
        // POINTL->_POINTL
        public POINTL PathSourceSize;

        // RECTL->_RECTL
        public RECTL DesktopImageRegion;

        // RECTL->_RECTL
        public RECTL DesktopImageClip;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_MODE_INFO
    {
        // DISPLAYCONFIG_MODE_INFO_TYPE->Anonymous_e328c430_ee09_4e5a_93e4_f407dbea5d4f
        public DISPLAYCONFIG_MODE_INFO_TYPE infoType;

        // UINT32->unsigned int
        public uint id;

        // LUID->_LUID
        public LUID adapterId;

        // Anonymous_fa905f1f_70a2_4b5b_8df3_0074ac1037b8
        public Anonymous_fa905f1f_70a2_4b5b_8df3_0074ac1037b8 Union1;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_PATH_INFO
    {
        // DISPLAYCONFIG_PATH_SOURCE_INFO
        public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;

        // DISPLAYCONFIG_PATH_TARGET_INFO
        public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;

        // UINT32->unsigned int
        public uint flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_PATH_SOURCE_INFO
    {
        // LUID->_LUID
        public LUID adapterId;

        // UINT32->unsigned int
        public uint id;

        // Anonymous_4be23a3d_1961_462d_ab0c_22a3cfa7a5d3
        public Anonymous_4be23a3d_1961_462d_ab0c_22a3cfa7a5d3 Union1;

        // UINT32->unsigned int
        public uint statusFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_PATH_TARGET_INFO
    {
        // LUID->_LUID
        public LUID adapterId;

        // UINT32->unsigned int
        public uint id;

        // Anonymous_9a90a686_b6cf_4bda_bc00_c40cd73b5a54
        public Anonymous_9a90a686_b6cf_4bda_bc00_c40cd73b5a54 Union1;

        // DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY->Anonymous_a6d6e24b_1c95_4d48_af17_2864806d51b6
        public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;

        // DISPLAYCONFIG_ROTATION->Anonymous_6080c5ef_6cd8_4ac0_904d_64b5f16cab32
        public DISPLAYCONFIG_ROTATION rotation;

        // DISPLAYCONFIG_SCALING->Anonymous_f69e85df_1da3_4b57_a4a6_0fa524266036
        public DISPLAYCONFIG_SCALING scaling;

        // DISPLAYCONFIG_RATIONAL
        public DISPLAYCONFIG_RATIONAL refreshRate;

        // DISPLAYCONFIG_SCANLINE_ORDERING->Anonymous_5ff6ec6a_03af_4513_9a0a_a6cba858ee8d
        public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;

        // BOOL->int
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public bool targetAvailable;

        // UINT32->unsigned int
        public uint statusFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_RATIONAL
    {
        // UINT32->unsigned int
        public uint Numerator;

        // UINT32->unsigned int
        public uint Denominator;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_SOURCE_MODE
    {
        // UINT32->unsigned int
        public uint width;

        // UINT32->unsigned int
        public uint height;

        // DISPLAYCONFIG_PIXELFORMAT->Anonymous_295899c1_a2a4_4629_98e8_5faabefa65e0
        public DISPLAYCONFIG_PIXELFORMAT pixelFormat;

        // POINTL->_POINTL
        public POINTL position;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_TARGET_MODE
    {
        // DISPLAYCONFIG_VIDEO_SIGNAL_INFO
        public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
    {
        // UINT64->unsigned __int64
        public ulong pixelRate;

        // DISPLAYCONFIG_RATIONAL
        public DISPLAYCONFIG_RATIONAL hSyncFreq;

        // DISPLAYCONFIG_RATIONAL
        public DISPLAYCONFIG_RATIONAL vSyncFreq;

        // DISPLAYCONFIG_2DREGION
        public DISPLAYCONFIG_2DREGION activeSize;

        // DISPLAYCONFIG_2DREGION
        public DISPLAYCONFIG_2DREGION totalSize;

        // Anonymous_17ea295e_ab4c_4636_977f_5f219c859b79
        public Anonymous_17ea295e_ab4c_4636_977f_5f219c859b79 Union1;

        // DISPLAYCONFIG_SCANLINE_ORDERING->Anonymous_5ff6ec6a_03af_4513_9a0a_a6cba858ee8d
        public DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LUID
    {
        // DWORD->unsigned int
        public uint LowPart;

        // LONG->int
        public int HighPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECTL
    {
        // LONG->int
        public int left;

        // LONG->int
        public int top;

        // LONG->int
        public int right;

        // LONG->int
        public int bottom;
    }
}