namespace Decryptcode.Assessment.Service.Domain.Guards;

using System.Runtime.CompilerServices;

public static class Guard
{
    public static void Null<T>(T value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value is null)
            throw new ArgumentNullException(parameterName);
    }

    public static void NullOrEmpty(string? value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Value cannot be null or empty.", parameterName);
    }

    public static void NullOrWhiteSpace(string? value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(parameterName, "Value cannot be null or whitespace.");
    }

    public static string NullOrWhiteSpace(string? value, string? defaultValue, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(parameterName, "Value cannot be null or whitespace.");
        return value;
    }

    public static void NullOrEmpty<T>(IEnumerable<T>? sequence, [CallerArgumentExpression(nameof(sequence))] string? parameterName = null)
    {
        if (sequence is null)
            throw new ArgumentNullException(parameterName);

        if (!sequence.Any())
            throw new ArgumentException("Collection cannot be empty.", parameterName);
    }

    public static void DefaultGuid(Guid value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Guid cannot be empty.", parameterName);
    }

    public static void OutOfRange<T>(T value, T min, T max, [CallerArgumentExpression(nameof(value))] string? parameterName = null) where T : IComparable<T>
    {
        if (min.CompareTo(max) > 0)
            throw new ArgumentException("min must be less than or equal to max.", nameof(min));

        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be in range [{min}..{max}].");
    }

    public static void InvalidEnum<TEnum>(TEnum value, [CallerArgumentExpression(nameof(value))] string? parameterName = null) where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
            throw new ArgumentException("Invalid enum value.", parameterName);
    }
}
