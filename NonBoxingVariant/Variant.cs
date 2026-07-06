using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NET
using IntegerSignedMagnitude = System.Int128;
using IntegerMagnitude = System.UInt128;
#else
using IntegerSignedMagnitude = System.Int64;
using IntegerMagnitude = System.UInt64;
#endif

namespace NonBoxingVariant;

/// <summary>
/// A value type that can hold a value of one of several specified types, without boxing.
/// </summary>
/// <remarks>
/// See also <see href="https://github.com/JeremyKuhne/touki/blob/main/touki/Touki/Value.cs">this alternative implementation</see>.
/// </remarks>
[Union]
public readonly struct Variant : IEquatable<Variant>
{
    private readonly VariantType type;
    private readonly InlineData data;

    public Variant(bool value)
    {
        this.type = VariantType.Bool;
        Write(ref this.data, value);
    }

    public Variant(char value)
    {
        this.type = VariantType.Char;
        Write(ref this.data, value);
    }

    public Variant(sbyte value)
    {
        this.type = VariantType.Int8;
        Write(ref this.data, value);
    }

    public Variant(byte value)
    {
        this.type = VariantType.UInt8;
        Write(ref this.data, value);
    }

    public Variant(short value)
    {
        this.type = VariantType.Int16;
        Write(ref this.data, value);
    }

    public Variant(ushort value)
    {
        this.type = VariantType.UInt16;
        Write(ref this.data, value);
    }

    public Variant(int value)
    {
        this.type = VariantType.Int32;
        Write(ref this.data, value);
    }

    public Variant(uint value)
    {
        this.type = VariantType.UInt32;
        Write(ref this.data, value);
    }

    public Variant(long value)
    {
        this.type = VariantType.Int64;
        Write(ref this.data, value);
    }

    public Variant(nint value)
    {
        this.type = VariantType.IntPtr;
        Write(ref this.data, value);
    }

    public Variant(ulong value)
    {
        this.type = VariantType.UInt64;
        Write(ref this.data, value);
    }

    public Variant(nuint value)
    {
        this.type = VariantType.UIntPtr;
        Write(ref this.data, value);
    }

#if NET
    public Variant(Int128 value)
    {
        this.type = VariantType.Int128;
        Write(ref this.data, value);
    }

    public Variant(UInt128 value)
    {
        this.type = VariantType.UInt128;
        Write(ref this.data, value);
    }

    public Variant(DateOnly value)
    {
        this.type = VariantType.DateOnly;
        Write(ref this.data, value);
    }

    public Variant(TimeOnly value)
    {
        this.type = VariantType.TimeOnly;
        Write(ref this.data, value);
    }

    public Variant(Half value)
    {
        this.type = VariantType.Half;
        Write(ref this.data, value);
    }
#endif

    public Variant(float value)
    {
        this.type = VariantType.Float;
        Write(ref this.data, value);
    }

    public Variant(double value)
    {
        this.type = VariantType.Double;
        Write(ref this.data, value);
    }

    public Variant(decimal value)
    {
        this.type = VariantType.Decimal;
        Write(ref this.data, value);
    }

    public Variant(TimeSpan value)
    {
        this.type = VariantType.TimeSpan;
        Write(ref this.data, value);
    }

    public Variant(DateTime value)
    {
        this.type = VariantType.DateTime;
        Write(ref this.data, value);
    }

    public Variant(DateTimeOffset value)
    {
        this.type = VariantType.DateTimeOffset;
        Write(ref this.data, value);
    }

    public Variant(Guid value)
    {
        this.type = VariantType.Guid;
        Write(ref this.data, value);
    }

    public static explicit operator bool(Variant value) => value.GetTypedValue<bool>(VariantType.Bool);

    public static explicit operator char(Variant value) => value.GetTypedValue<char>(VariantType.Char);

    public static explicit operator sbyte(Variant value) => value.GetTypedValue<sbyte>(VariantType.Int8);

    public static explicit operator byte(Variant value) => value.GetTypedValue<byte>(VariantType.UInt8);

    public static explicit operator short(Variant value) => value.GetInt16Value();

    public static explicit operator ushort(Variant value) => value.GetUInt16Value();

    public static explicit operator int(Variant value) => value.GetInt32Value();

    public static explicit operator uint(Variant value) => value.GetUInt32Value();

    public static explicit operator long(Variant value) => value.GetInt64Value();

    public static explicit operator nint(Variant value) => value.GetIntPtrValue();

    public static explicit operator ulong(Variant value) => value.GetUInt64Value();

    public static explicit operator nuint(Variant value) => value.GetUIntPtrValue();

#if NET
    public static explicit operator Int128(Variant value) => value.GetInt128Value();

    public static explicit operator UInt128(Variant value) => value.GetUInt128Value();

    public static explicit operator Half(Variant value) => value.GetTypedValue<Half>(VariantType.Half);

    public static explicit operator DateOnly(Variant value) => value.GetTypedValue<DateOnly>(VariantType.DateOnly);

    public static explicit operator TimeOnly(Variant value) => value.GetTypedValue<TimeOnly>(VariantType.TimeOnly);
#endif

    public static explicit operator float(Variant value) => value.GetTypedValue<float>(VariantType.Float);

    public static explicit operator double(Variant value) => value.GetTypedValue<double>(VariantType.Double);

    public static explicit operator decimal(Variant value) => value.GetTypedValue<decimal>(VariantType.Decimal);

    public static explicit operator TimeSpan(Variant value) => value.GetTypedValue<TimeSpan>(VariantType.TimeSpan);

    public static explicit operator DateTime(Variant value) => value.GetTypedValue<DateTime>(VariantType.DateTime);

    public static explicit operator DateTimeOffset(Variant value) => value.GetTypedValue<DateTimeOffset>(VariantType.DateTimeOffset);

    public static explicit operator Guid(Variant value) => value.GetTypedValue<Guid>(VariantType.Guid);

    public static bool operator ==(Variant left, Variant right) => left.Equals(right);

    public static bool operator !=(Variant left, Variant right) => !left.Equals(right);

    public readonly bool Equals(Variant other)
    {
        if (this.TryGetIntegerEqualityValue(out IntegerMagnitude thisMagnitude, out bool thisIsNegative)
            && other.TryGetIntegerEqualityValue(out IntegerMagnitude otherMagnitude, out bool otherIsNegative))
        {
            return thisIsNegative == otherIsNegative && thisMagnitude == otherMagnitude;
        }

        if (this.type != other.type)
        {
            return false;
        }

        return this.type switch
        {
            VariantType.None => true,
            VariantType.Bool => this.Read<bool>().Equals(other.Read<bool>()),
            VariantType.Char => this.Read<char>().Equals(other.Read<char>()),
            VariantType.Int8 => this.Read<sbyte>().Equals(other.Read<sbyte>()),
            VariantType.UInt8 => this.Read<byte>().Equals(other.Read<byte>()),
            VariantType.Int16 => this.Read<short>().Equals(other.Read<short>()),
            VariantType.UInt16 => this.Read<ushort>().Equals(other.Read<ushort>()),
            VariantType.Int32 => this.Read<int>().Equals(other.Read<int>()),
            VariantType.UInt32 => this.Read<uint>().Equals(other.Read<uint>()),
            VariantType.Int64 => this.Read<long>().Equals(other.Read<long>()),
            VariantType.IntPtr => this.Read<nint>().Equals(other.Read<nint>()),
            VariantType.UInt64 => this.Read<ulong>().Equals(other.Read<ulong>()),
            VariantType.UIntPtr => this.Read<nuint>().Equals(other.Read<nuint>()),
#if NET
            VariantType.Int128 => this.Read<Int128>().Equals(other.Read<Int128>()),
            VariantType.UInt128 => this.Read<UInt128>().Equals(other.Read<UInt128>()),
            VariantType.Half => this.Read<Half>().Equals(other.Read<Half>()),
            VariantType.DateOnly => this.Read<DateOnly>().Equals(other.Read<DateOnly>()),
            VariantType.TimeOnly => this.Read<TimeOnly>().Equals(other.Read<TimeOnly>()),
#endif
            VariantType.Float => this.Read<float>().Equals(other.Read<float>()),
            VariantType.Double => this.Read<double>().Equals(other.Read<double>()),
            VariantType.Decimal => this.Read<decimal>().Equals(other.Read<decimal>()),
            VariantType.TimeSpan => this.Read<TimeSpan>().Equals(other.Read<TimeSpan>()),
            VariantType.DateTime => this.Read<DateTime>().Equals(other.Read<DateTime>()),
            VariantType.DateTimeOffset => this.Read<DateTimeOffset>().Equals(other.Read<DateTimeOffset>()),
            VariantType.Guid => this.Read<Guid>().Equals(other.Read<Guid>()),
            _ => false,
        };
    }

    public override readonly bool Equals(object? obj) => obj is Variant other && this.Equals(other);

    public override readonly int GetHashCode()
    {
        if (this.TryGetIntegerEqualityValue(out IntegerMagnitude magnitude, out bool isNegative))
        {
            return GetIntegerHashCode(magnitude, isNegative);
        }

        return this.type switch
        {
            VariantType.None => 0,
            VariantType.Bool => HashCode.Combine(this.type, this.Read<bool>()),
            VariantType.Char => HashCode.Combine(this.type, this.Read<char>()),
#if NET
            VariantType.Half => HashCode.Combine(this.type, this.Read<Half>()),
            VariantType.DateOnly => HashCode.Combine(this.type, this.Read<DateOnly>()),
            VariantType.TimeOnly => HashCode.Combine(this.type, this.Read<TimeOnly>()),
#endif
            VariantType.Float => HashCode.Combine(this.type, this.Read<float>()),
            VariantType.Double => HashCode.Combine(this.type, this.Read<double>()),
            VariantType.Decimal => HashCode.Combine(this.type, this.Read<decimal>()),
            VariantType.TimeSpan => HashCode.Combine(this.type, this.Read<TimeSpan>()),
            VariantType.DateTime => HashCode.Combine(this.type, this.Read<DateTime>()),
            VariantType.DateTimeOffset => HashCode.Combine(this.type, this.Read<DateTimeOffset>()),
            VariantType.Guid => HashCode.Combine(this.type, this.Read<Guid>()),
            _ => throw new InvalidOperationException(),
        };
    }

    public object Value => this.type switch
    {
        VariantType.Bool => this.Read<bool>(),
        VariantType.Char => this.Read<char>(),
        VariantType.Int8 => this.Read<sbyte>(),
        VariantType.UInt8 => this.Read<byte>(),
        VariantType.Int16 => this.Read<short>(),
        VariantType.UInt16 => this.Read<ushort>(),
        VariantType.Int32 => this.Read<int>(),
        VariantType.UInt32 => this.Read<uint>(),
        VariantType.Int64 => this.Read<long>(),
        VariantType.IntPtr => this.Read<nint>(),
        VariantType.UInt64 => this.Read<ulong>(),
        VariantType.UIntPtr => this.Read<nuint>(),
#if NET
        VariantType.Int128 => this.Read<Int128>(),
        VariantType.UInt128 => this.Read<UInt128>(),
        VariantType.Half => this.Read<Half>(),
        VariantType.DateOnly => this.Read<DateOnly>(),
        VariantType.TimeOnly => this.Read<TimeOnly>(),
#endif
        VariantType.Float => this.Read<float>(),
        VariantType.Double => this.Read<double>(),
        VariantType.Decimal => this.Read<decimal>(),
        VariantType.TimeSpan => this.Read<TimeSpan>(),
        VariantType.DateTime => this.Read<DateTime>(),
        VariantType.DateTimeOffset => this.Read<DateTimeOffset>(),
        VariantType.Guid => this.Read<Guid>(),
        _ => throw new InvalidOperationException(),
    };

    public bool TryGetValue(out bool value) => this.TryGetTypedValue(VariantType.Bool, out value);

    public bool TryGetValue(out char value) => this.TryGetTypedValue(VariantType.Char, out value);

    public bool TryGetValue(out sbyte value) => this.TryGetTypedValue(VariantType.Int8, out value);

    public bool TryGetValue(out byte value) => this.TryGetTypedValue(VariantType.UInt8, out value);

    public bool TryGetValue(out short value) => this.TryGetInt16Value(out value);

    public bool TryGetValue(out ushort value) => this.TryGetUInt16Value(out value);

    public bool TryGetValue(out int value) => this.TryGetInt32Value(out value);

    public bool TryGetValue(out uint value) => this.TryGetUInt32Value(out value);

    public bool TryGetValue(out long value) => this.TryGetInt64Value(out value);

    public bool TryGetValue(out nint value) => this.TryGetIntPtrValue(out value);

    public bool TryGetValue(out ulong value) => this.TryGetUInt64Value(out value);

    public bool TryGetValue(out nuint value) => this.TryGetUIntPtrValue(out value);

#if NET
    public bool TryGetValue(out Int128 value) => this.TryGetInt128Value(out value);

    public bool TryGetValue(out UInt128 value) => this.TryGetUInt128Value(out value);

    public bool TryGetValue(out Half value) => this.TryGetTypedValue(VariantType.Half, out value);

    public bool TryGetValue(out DateOnly value) => this.TryGetTypedValue(VariantType.DateOnly, out value);

    public bool TryGetValue(out TimeOnly value) => this.TryGetTypedValue(VariantType.TimeOnly, out value);
#endif

    public bool TryGetValue(out float value) => this.TryGetTypedValue(VariantType.Float, out value);

    public bool TryGetValue(out double value) => this.TryGetTypedValue(VariantType.Double, out value);

    public bool TryGetValue(out decimal value) => this.TryGetTypedValue(VariantType.Decimal, out value);

    public bool TryGetValue(out TimeSpan value) => this.TryGetTypedValue(VariantType.TimeSpan, out value);

    public bool TryGetValue(out DateTime value) => this.TryGetTypedValue(VariantType.DateTime, out value);

    public bool TryGetValue(out DateTimeOffset value) => this.TryGetTypedValue(VariantType.DateTimeOffset, out value);

    public bool TryGetValue(out Guid value) => this.TryGetTypedValue(VariantType.Guid, out value);

#if NET
    private static void Write<T>(ref InlineData data, T value)
        where T : unmanaged
        => MemoryMarshal.Write(data[..], in value);
#else
    private static unsafe void Write<T>(ref InlineData data, T value)
        where T : unmanaged
    {
        fixed (byte* bytes = data.buffer)
        {
            Buffer.MemoryCopy(&value, bytes, 16, sizeof(T));
        }
    }
#endif

    private readonly T GetTypedValue<T>(VariantType expectedType)
        where T : unmanaged
    {
        if (this.type == expectedType)
        {
            return this.Read<T>();
        }

        throw this.CreateInvalidCastException(expectedType.ToString());
    }

    private readonly short GetInt16Value()
        => this.TryGetInt16Value(out short value) ? value : throw this.CreateInvalidCastException(nameof(Int16));

    private readonly ushort GetUInt16Value()
        => this.TryGetUInt16Value(out ushort value) ? value : throw this.CreateInvalidCastException(nameof(UInt16));

    private readonly int GetInt32Value()
        => this.TryGetInt32Value(out int value) ? value : throw this.CreateInvalidCastException(nameof(Int32));

    private readonly uint GetUInt32Value()
        => this.TryGetUInt32Value(out uint value) ? value : throw this.CreateInvalidCastException(nameof(UInt32));

    private readonly long GetInt64Value()
        => this.TryGetInt64Value(out long value) ? value : throw this.CreateInvalidCastException(nameof(Int64));

    private readonly nint GetIntPtrValue()
        => this.TryGetIntPtrValue(out nint value) ? value : throw this.CreateInvalidCastException(nameof(IntPtr));

    private readonly ulong GetUInt64Value()
        => this.TryGetUInt64Value(out ulong value) ? value : throw this.CreateInvalidCastException(nameof(UInt64));

    private readonly nuint GetUIntPtrValue()
        => this.TryGetUIntPtrValue(out nuint value) ? value : throw this.CreateInvalidCastException(nameof(UIntPtr));

#if NET
    private readonly Int128 GetInt128Value()
        => this.TryGetInt128Value(out Int128 value) ? value : throw this.CreateInvalidCastException(nameof(Int128));

    private readonly UInt128 GetUInt128Value()
        => this.TryGetUInt128Value(out UInt128 value) ? value : throw this.CreateInvalidCastException(nameof(UInt128));
#endif

    private readonly InvalidCastException CreateInvalidCastException(string expectedTypeName)
        => new(this.type == VariantType.None
            ? "Variant does not contain a value."
            : $"Variant contains a {this.type} value, which cannot be retrieved as a {expectedTypeName} value.");

    private readonly bool TryGetTypedValue<T>(VariantType expectedType, out T value)
        where T : unmanaged
    {
        if (this.type == expectedType)
        {
            value = this.Read<T>();
            return true;
        }

        value = default;
        return false;
    }

    private readonly bool TryGetInt16Value(out short value)
    {
        switch (this.type)
        {
            case VariantType.Int8:
                value = this.Read<sbyte>();
                return true;
            case VariantType.UInt8:
                value = this.Read<byte>();
                return true;
            case VariantType.Int16:
                value = this.Read<short>();
                return true;
            default:
                value = default;
                return false;
        }
    }

    private readonly bool TryGetUInt16Value(out ushort value)
    {
        switch (this.type)
        {
            case VariantType.UInt8:
                value = this.Read<byte>();
                return true;
            case VariantType.UInt16:
                value = this.Read<ushort>();
                return true;
            default:
                value = default;
                return false;
        }
    }

    private readonly bool TryGetInt32Value(out int value)
    {
        switch (this.type)
        {
            case VariantType.Int8:
                value = this.Read<sbyte>();
                return true;
            case VariantType.UInt8:
                value = this.Read<byte>();
                return true;
            case VariantType.Int16:
                value = this.Read<short>();
                return true;
            case VariantType.UInt16:
                value = this.Read<ushort>();
                return true;
            case VariantType.Int32:
                value = this.Read<int>();
                return true;
            default:
                value = default;
                return false;
        }
    }

    private readonly bool TryGetUInt32Value(out uint value)
    {
        switch (this.type)
        {
            case VariantType.UInt8:
                value = this.Read<byte>();
                return true;
            case VariantType.UInt16:
                value = this.Read<ushort>();
                return true;
            case VariantType.UInt32:
                value = this.Read<uint>();
                return true;
            default:
                value = default;
                return false;
        }
    }

    private readonly bool TryGetInt64Value(out long value)
    {
        switch (this.type)
        {
            case VariantType.Int8:
                value = this.Read<sbyte>();
                return true;
            case VariantType.UInt8:
                value = this.Read<byte>();
                return true;
            case VariantType.Int16:
                value = this.Read<short>();
                return true;
            case VariantType.UInt16:
                value = this.Read<ushort>();
                return true;
            case VariantType.Int32:
                value = this.Read<int>();
                return true;
            case VariantType.UInt32:
                value = this.Read<uint>();
                return true;
            case VariantType.Int64:
                value = this.Read<long>();
                return true;
            case VariantType.IntPtr:
                value = this.Read<nint>();
                return true;
            default:
                value = default;
                return false;
        }
    }

    private readonly bool TryGetIntPtrValue(out nint value)
    {
        switch (this.type)
        {
            case VariantType.Int8:
                value = this.Read<sbyte>();
                return true;
            case VariantType.UInt8:
                value = this.Read<byte>();
                return true;
            case VariantType.Int16:
                value = this.Read<short>();
                return true;
            case VariantType.UInt16:
                value = this.Read<ushort>();
                return true;
            case VariantType.Int32:
                value = this.Read<int>();
                return true;
            case VariantType.UInt32 when IntPtr.Size == 8:
                value = (nint)this.Read<uint>();
                return true;
            case VariantType.Int64 when IntPtr.Size == 8:
                value = (nint)this.Read<long>();
                return true;
            case VariantType.IntPtr:
                value = this.Read<nint>();
                return true;
            default:
                value = default;
                return false;
        }
    }

    private readonly bool TryGetUInt64Value(out ulong value)
    {
        switch (this.type)
        {
            case VariantType.UInt8:
                value = this.Read<byte>();
                return true;
            case VariantType.UInt16:
                value = this.Read<ushort>();
                return true;
            case VariantType.UInt32:
                value = this.Read<uint>();
                return true;
            case VariantType.UInt64:
                value = this.Read<ulong>();
                return true;
            case VariantType.UIntPtr:
                value = this.Read<nuint>();
                return true;
            default:
                value = default;
                return false;
        }
    }

    private readonly bool TryGetUIntPtrValue(out nuint value)
    {
        switch (this.type)
        {
            case VariantType.UInt8:
                value = this.Read<byte>();
                return true;
            case VariantType.UInt16:
                value = this.Read<ushort>();
                return true;
            case VariantType.UInt32:
                value = this.Read<uint>();
                return true;
            case VariantType.UInt64 when IntPtr.Size == 8:
                value = (nuint)this.Read<ulong>();
                return true;
            case VariantType.UIntPtr:
                value = this.Read<nuint>();
                return true;
            default:
                value = default;
                return false;
        }
    }

#if NET
    private readonly bool TryGetInt128Value(out Int128 value)
    {
        switch (this.type)
        {
            case VariantType.Int8:
                value = this.Read<sbyte>();
                return true;
            case VariantType.UInt8:
                value = this.Read<byte>();
                return true;
            case VariantType.Int16:
                value = this.Read<short>();
                return true;
            case VariantType.UInt16:
                value = this.Read<ushort>();
                return true;
            case VariantType.Int32:
                value = this.Read<int>();
                return true;
            case VariantType.UInt32:
                value = this.Read<uint>();
                return true;
            case VariantType.Int64:
                value = this.Read<long>();
                return true;
            case VariantType.IntPtr:
                value = (Int128)this.Read<nint>();
                return true;
            case VariantType.UInt64:
                value = this.Read<ulong>();
                return true;
            case VariantType.UIntPtr:
                value = (Int128)this.Read<nuint>();
                return true;
            case VariantType.Int128:
                value = this.Read<Int128>();
                return true;
            default:
                value = default;
                return false;
        }
    }

    private readonly bool TryGetUInt128Value(out UInt128 value)
    {
        switch (this.type)
        {
            case VariantType.UInt8:
                value = this.Read<byte>();
                return true;
            case VariantType.UInt16:
                value = this.Read<ushort>();
                return true;
            case VariantType.UInt32:
                value = this.Read<uint>();
                return true;
            case VariantType.UInt64:
                value = this.Read<ulong>();
                return true;
            case VariantType.UIntPtr:
                value = (UInt128)this.Read<nuint>();
                return true;
            case VariantType.UInt128:
                value = this.Read<UInt128>();
                return true;
            default:
                value = default;
                return false;
        }
    }
#endif

    private readonly bool TryGetIntegerEqualityValue(out IntegerMagnitude magnitude, out bool isNegative)
    {
        switch (this.type)
        {
            case VariantType.Int8:
                CreateSignedIntegerEqualityValue(this.Read<sbyte>(), out magnitude, out isNegative);
                return true;
            case VariantType.UInt8:
                CreateUnsignedIntegerEqualityValue(this.Read<byte>(), out magnitude, out isNegative);
                return true;
            case VariantType.Int16:
                CreateSignedIntegerEqualityValue(this.Read<short>(), out magnitude, out isNegative);
                return true;
            case VariantType.UInt16:
                CreateUnsignedIntegerEqualityValue(this.Read<ushort>(), out magnitude, out isNegative);
                return true;
            case VariantType.Int32:
                CreateSignedIntegerEqualityValue(this.Read<int>(), out magnitude, out isNegative);
                return true;
            case VariantType.UInt32:
                CreateUnsignedIntegerEqualityValue(this.Read<uint>(), out magnitude, out isNegative);
                return true;
            case VariantType.Int64:
                CreateSignedIntegerEqualityValue(this.Read<long>(), out magnitude, out isNegative);
                return true;
            case VariantType.IntPtr:
                CreateSignedIntegerEqualityValue(this.Read<nint>(), out magnitude, out isNegative);
                return true;
            case VariantType.UInt64:
                CreateUnsignedIntegerEqualityValue(this.Read<ulong>(), out magnitude, out isNegative);
                return true;
            case VariantType.UIntPtr:
                CreateUnsignedIntegerEqualityValue(this.Read<nuint>(), out magnitude, out isNegative);
                return true;
#if NET
            case VariantType.Int128:
                CreateSignedIntegerEqualityValue(this.Read<Int128>(), out magnitude, out isNegative);
                return true;
            case VariantType.UInt128:
                CreateUnsignedIntegerEqualityValue(this.Read<UInt128>(), out magnitude, out isNegative);
                return true;
#endif
            default:
                magnitude = default;
                isNegative = false;
                return false;
        }
    }

    private static void CreateSignedIntegerEqualityValue(IntegerSignedMagnitude value, out IntegerMagnitude magnitude, out bool isNegative)
    {
        isNegative = value < 0;
#if NET
        magnitude = isNegative ? (UInt128)(-(value + 1)) + 1 : (UInt128)value;
#else
        magnitude = isNegative ? unchecked((ulong)(-(value + 1))) + 1UL : unchecked((ulong)value);
#endif
    }

    private static void CreateUnsignedIntegerEqualityValue(IntegerMagnitude value, out IntegerMagnitude magnitude, out bool isNegative)
    {
        magnitude = value;
        isNegative = false;
    }

    private static int GetIntegerHashCode(IntegerMagnitude magnitude, bool isNegative)
    {
#if NET
        return HashCode.Combine(isNegative, (ulong)(magnitude >> 64), (ulong)magnitude);
#else
        return HashCode.Combine(isNegative, magnitude);
#endif
    }

#if NET
    private readonly T Read<T>()
        where T : unmanaged
        => MemoryMarshal.Read<T>(this.data[..]);
#else
    private readonly unsafe T Read<T>()
        where T : unmanaged
    {
        InlineData data = this.data;
        byte* bytes = data.buffer;
        T value = default;
        Buffer.MemoryCopy(bytes, &value, sizeof(T), sizeof(T));
        return value;
    }
#endif

    private enum VariantType : byte
    {
        None,
        Bool,
        Char,
        Int8,
        UInt8,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Int64,
        IntPtr,
        UInt64,
        UIntPtr,
        Int128,
        UInt128,
        Half,
        Float,
        Double,
        Decimal,
        TimeSpan,
        DateOnly,
        TimeOnly,
        DateTime,
        DateTimeOffset,
        Guid,
    }

#if NET
    [InlineArray(16)] // large enough for a Guid
    private struct InlineData
    {
        private byte _element0;
    }
#else
    private unsafe struct InlineData
    {
        public fixed byte buffer[16];
    }
#endif
}
