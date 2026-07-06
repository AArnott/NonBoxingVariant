using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[Union]
public readonly record struct Variant : IUnion
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

    public Variant(Half value)
    {
        this.type = VariantType.Half;
        Write(ref this.data, value);
    }

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
        VariantType.Int128 => this.Read<Int128>(),
        VariantType.UInt128 => this.Read<UInt128>(),
        VariantType.Half => this.Read<Half>(),
        VariantType.Float => this.Read<float>(),
        VariantType.Double => this.Read<double>(),
        VariantType.Decimal => this.Read<decimal>(),
        VariantType.TimeSpan => this.Read<TimeSpan>(),
        VariantType.DateOnly => this.Read<DateOnly>(),
        VariantType.TimeOnly => this.Read<TimeOnly>(),
        VariantType.DateTime => this.Read<DateTime>(),
        VariantType.DateTimeOffset => this.Read<DateTimeOffset>(),
        VariantType.Guid => this.Read<Guid>(),
        _ => throw new InvalidOperationException(),
    };

    public bool TryGetValue(out bool value) => this.TryGetTypedValue(VariantType.Bool, out value);

    public bool TryGetValue(out char value) => this.TryGetTypedValue(VariantType.Char, out value);

    public bool TryGetValue(out sbyte value) => this.TryGetTypedValue(VariantType.Int8, out value);

    public bool TryGetValue(out byte value) => this.TryGetTypedValue(VariantType.UInt8, out value);

    public bool TryGetValue(out short value) => this.TryGetTypedValue(VariantType.Int16, out value);

    public bool TryGetValue(out ushort value) => this.TryGetTypedValue(VariantType.UInt16, out value);

    public bool TryGetValue(out int value) => this.TryGetTypedValue(VariantType.Int32, out value);

    public bool TryGetValue(out uint value) => this.TryGetTypedValue(VariantType.UInt32, out value);

    public bool TryGetValue(out long value) => this.TryGetTypedValue(VariantType.Int64, out value);

    public bool TryGetValue(out nint value) => this.TryGetTypedValue(VariantType.IntPtr, out value);

    public bool TryGetValue(out ulong value) => this.TryGetTypedValue(VariantType.UInt64, out value);

    public bool TryGetValue(out nuint value) => this.TryGetTypedValue(VariantType.UIntPtr, out value);

    public bool TryGetValue(out Int128 value) => this.TryGetTypedValue(VariantType.Int128, out value);

    public bool TryGetValue(out UInt128 value) => this.TryGetTypedValue(VariantType.UInt128, out value);

    public bool TryGetValue(out Half value) => this.TryGetTypedValue(VariantType.Half, out value);

    public bool TryGetValue(out float value) => this.TryGetTypedValue(VariantType.Float, out value);

    public bool TryGetValue(out double value) => this.TryGetTypedValue(VariantType.Double, out value);

    public bool TryGetValue(out decimal value) => this.TryGetTypedValue(VariantType.Decimal, out value);

    public bool TryGetValue(out TimeSpan value) => this.TryGetTypedValue(VariantType.TimeSpan, out value);

    public bool TryGetValue(out DateOnly value) => this.TryGetTypedValue(VariantType.DateOnly, out value);

    public bool TryGetValue(out TimeOnly value) => this.TryGetTypedValue(VariantType.TimeOnly, out value);

    public bool TryGetValue(out DateTime value) => this.TryGetTypedValue(VariantType.DateTime, out value);

    public bool TryGetValue(out DateTimeOffset value) => this.TryGetTypedValue(VariantType.DateTimeOffset, out value);

    public bool TryGetValue(out Guid value) => this.TryGetTypedValue(VariantType.Guid, out value);

    private static void Write<T>(ref InlineData data, T value)
        where T : unmanaged
        => MemoryMarshal.Write(data[..], in value);

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

    private readonly T Read<T>()
        where T : unmanaged
        => MemoryMarshal.Read<T>(this.data[..]);

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

    [InlineArray(16)] // large enough for a Guid
    private struct InlineData
    {
        private byte _element0;
    }
}