# Deprecated

> **This library is retired. What it did is built into .NET.**
>
> - `ToHex(prefix, uppercase)` becomes `Convert.ToHexString(bytes)` (upper case, .NET 5 and later) or `Convert.ToHexStringLower(bytes)` (.NET 9 and later), with `"0x" + ...` for the prefix. `Convert.FromHexString` goes the other way.
> - Comparing the contents of two arrays becomes `ReadOnlySpan<byte>.SequenceEqual`, or `CryptographicOperations.FixedTimeEquals` when the bytes are secret.
>
> It targeted .NET Framework 4.5 and .NET Standard 1.1, which Microsoft no longer supports.
>
> The NuGet package is deprecated (`NCode.ByteArrayExtensions`) and this repository is archived. The code stays available as it is.

# Overview
Provides extension methods for byte arrays such as converting to hexadecimal and comparing array contents.

Target Frameworks:
* net45
* netstandard1.1

```csharp
/// <summary>
/// Provides a set of extension methods for byte arrays.
/// </summary>
public static class ByteArrayExtensions
{
    private static IByteArrayExtensionsProvider _provider;

    /// <summary>
    /// Gets or sets the <see cref="IByteArrayExtensionsProvider"/> provider for this extension class.
    /// </summary>
    public static IByteArrayExtensionsProvider Provider
    {
        get => _provider ?? (_provider = new ByteArrayExtensionsProvider());
        set => _provider = value;
    }

    /// <summary>
    /// Converts the specified byte array into a hexadecimal string.
    /// </summary>
    /// <param name="array">The byte array to convert into a hexadecimal string.</param>
    /// <param name="prefix"><c>true</c> to prefix the string with <c>0x</c>.</param>
    /// <param name="uppercase"><c>true</c> to return all upper case characters.</param>
    /// <returns>The byte array converted into a hexadecimal string.</returns>
    public static string ToHex(this byte[] array, bool prefix, bool uppercase)
    {
        return Provider.ToHex(array, prefix, uppercase);
    }

    /// <summary>
    /// Returns <c>true</c> or <c>false</c> whether the two byte arrays are equal.
    /// </summary>
    public static bool Equals(this byte[] buffer1, byte[] buffer2)
    {
        return Provider.Equals(buffer1, buffer2);
    }
}

/// <summary>
/// Provides the implementation for the <see cref="ByteArrayExtensions"/> methods.
/// </summary>
public interface IByteArrayExtensionsProvider
{
    /// <summary>
    /// Converts the specified byte array into a hexadecimal string.
    /// </summary>
    /// <param name="array">The byte array to convert into a hexadecimal string.</param>
    /// <param name="prefix"><c>true</c> to prefix the string with <c>0x</c>.</param>
    /// <param name="uppercase"><c>true</c> to return all upper case characters.</param>
    /// <returns>The byte array converted into a hexadecimal string.</returns>
    string ToHex(byte[] array, bool prefix, bool uppercase);

    /// <summary>
    /// Returns <c>true</c> or <c>false</c> whether the two byte arrays are equal.
    /// </summary>
    bool Equals(byte[] buffer1, byte[] buffer2);
}
```
