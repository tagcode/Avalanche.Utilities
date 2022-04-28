// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary>Status Code Ids</summary>
public static class CodeIds
{
    /// <summary>Avalanche libraries</summary>
    public const int Avalanche = 0x20AA0000 | StatusCodes.ThirdPartyMask;
    /// <summary>Avalanche.Core.dll repack</summary>
    public const int CorePack = Avalanche | 0x0000_C000;
    /// <summary>Avalanche.Core.dll repack</summary>
    public const int Reserved0 = Avalanche | 0x00;
    /// <summary>Avalanche.Identity</summary>
    public const int Identity = CorePack | 0x10;
    /// <summary>Avalanche.Serialization</summary>
    public const int Serialization = CorePack | 0x20;
    /// <summary>Avalanche.Localization</summary>
    public const int Localization = CorePack | 0x30;
    /// <summary></summary>
    public const int Reserved4 = CorePack | 0x40;
    /// <summary>Avalanche.Service</summary>
    public const int Service = CorePack | 0x50;
    /// <summary>Avalanche.Service</summary>
    public const int Service2 = CorePack | 0x60;
    /// <summary></summary>
    public const int Reserved7 = CorePack | 0x70;
    /// <summary></summary>
    public const int Core = CorePack | 0x80;
    /// <summary>Avalanche.RMI</summary>
    public const int Reserved9 = CorePack | 0x90;
    /// <summary>Avalanche.Accessor</summary>
    public const int Accessor = CorePack | 0xA0;
    /// <summary>Avalanche.Binding</summary>
    public const int Binding = CorePack | 0xB0;
    /// <summary>Avalanche.Converter</summary>
    public const int Converter = CorePack | 0xC0;
    /// <summary>Avalanche.DataType</summary>
    public const int DataType = CorePack | 0xD0;
    /// <summary>Avalanche.Writer</summary>
    public const int Writer = CorePack | 0xE0;
    /// <summary>Avalanche.FileSystem</summary>
    public const int FileSystem = CorePack | 0xF0;

    /// <summary>Avalanche.OpcUa</summary>
    public const int OpcUa = Avalanche | 0x4A00;
}

