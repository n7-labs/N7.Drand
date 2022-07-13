using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace System;

public static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Boolean StartsWithHexPrefix(this String hex) => hex != null && hex.StartsWith("0x");


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static String TrimHexPrefix(this String hex) => hex.StartsWithHexPrefix() ? hex[2..] : hex;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger FromHexToUnsignedBigInteger(this String hex)
    {
        var hexWithourPrefix      = hex.TrimHexPrefix();
        var hexWithUnsignedPrefix = $"0{hexWithourPrefix}";

        return BigInteger.Parse(
            value    : hexWithUnsignedPrefix,
            style    : NumberStyles.AllowHexSpecifier,
            provider : null);
    }
}
