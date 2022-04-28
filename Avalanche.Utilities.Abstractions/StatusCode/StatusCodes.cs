// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>Status code bit helper</summary>
public static class StatusCodes
{
    /// <summary>(0-15) Code mask</summary>
    public const int CodeMask = 0x0000FFFF;
    /// <summary>(16-26) Facility/Namespace mask</summary>
    public const int FacilityMask = 0x07FF0000;
    /// <summary>(27) Is display text, 0=StatusCode, 1=Display text</summary>
    public const int DisplayTextMask = 0x08000000;
    /// <summary>(28) Reserved</summary>
    public const int Reserved28 = 0x10000000;
    /// <summary>(29) Indicates whether code is 0=standard code (Microsoft), 1=3rd party defined</summary>
    public const int ThirdPartyMask = 0x20000000;
    /// <summary>(30-31) Severity mask</summary>
    public const int SeverityMask = unchecked((int)0xC0000000U);

    /// <summary>Severity: Operation was successful and results may be used.</summary>
    public const int Good = 0x00000000;
    /// <summary>Severity: Operation was partially successful and that associated results may be suitable.</summary>
    public const int Uncertain = 0x40000000;
    /// <summary>Severity: Operation failed and associated results cannot be used.</summary>
    public const int Bad = unchecked((int)0x80000000U);
    /// <summary>Severity: Severe failure.</summary>
    public const int Severe = unchecked((int)0xC0000000U);

    /// <summary>Is severity good</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGood(int statuscode) => (statuscode & SeverityMask) == Good;
    /// <summary>Is severity not good</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotGood(int statuscode) => (statuscode & SeverityMask) != Good;
    /// <summary>Is severity bad (bit 31)</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBad(int statuscode) => (statuscode & Bad) == Bad;
    /// <summary>Is severity not bad (bit 31)</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotBad(int statuscode) => (statuscode & Bad) != Bad;
    /// <summary>Is severity uncertain</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUncertain(int statuscode) => (statuscode & SeverityMask) == Uncertain;
    /// <summary>Is severity not uncertain</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotUncertain(int statuscode) => (statuscode & SeverityMask) != Uncertain;
    /// <summary>Is severity severe failure</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSevere(int statuscode) => (statuscode & SeverityMask) == Severe;
    /// <summary>Is not severe failure</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotSevere(int statuscode) => (statuscode & SeverityMask) != Severe;

    /// <summary>Get severity</summary>
    /// <returns>0=unassigned, 1=good, 2=uncertain, 3=bad, 4=severe/critical</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden]
    public static int GetSeverityLevel(int statuscode)
    {
        if ((statuscode & StatusCodes.SeverityMask) == StatusCodes.Severe) return 4;
        if ((statuscode & StatusCodes.Bad) == StatusCodes.Bad) return 3;
        if ((statuscode & StatusCodes.SeverityMask) == StatusCodes.Uncertain) return 2;
        return 1;
    }

}


/* Bits on OPC-UA
 *   0- 7  Info bits, additional information
 *   8- 9  Type of information 
 *  10-13  Reserved
 *     14  DataValue semantics changed
 *     15  DataValue structure changed
 *  16-27  Event Condition
 *  28-29  Reserved
 *  30-31  Severity
 */

/* Bits on HResult
 * (0-15) Code mask
 * (16-26) Facility/Namespace mask
 * (27) Displaytext: 0=StatusCode, 1=Display text
 * (28) Reserved
 * (29) Thirdparty: 0=Standard Microsoft, 1=3rd party defined
 * (30) NT Severity, 1=Severe
 * (31) Severity 0=Good, 1=Bad
 */
