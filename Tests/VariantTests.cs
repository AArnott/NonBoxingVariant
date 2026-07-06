public class VariantTests
{
    [Test]
    public async Task BooleanValue()
    {
        await AssertRoundTrip(new Variant(true), true, static variant =>
        {
            var success = variant.TryGetValue(out bool value);
            return (success, value);
        });
    }

    [Test]
    public async Task CharacterValue()
    {
        await AssertRoundTrip(new Variant('Z'), 'Z', static variant =>
        {
            var success = variant.TryGetValue(out char value);
            return (success, value);
        });
    }

    [Test]
    public async Task SignedIntegralValues()
    {
        await AssertRoundTrip(new Variant((sbyte)-12), (sbyte)-12, static variant =>
        {
            var success = variant.TryGetValue(out sbyte value);
            return (success, value);
        });

        await AssertRoundTrip(new Variant((short)-32000), (short)-32000, static variant =>
        {
            var success = variant.TryGetValue(out short value);
            return (success, value);
        });

        await AssertRoundTrip(new Variant(-123456789), -123456789, static variant =>
        {
            var success = variant.TryGetValue(out int value);
            return (success, value);
        });

        await AssertRoundTrip(new Variant(-9_876_543_210L), -9_876_543_210L, static variant =>
        {
            var success = variant.TryGetValue(out long value);
            return (success, value);
        });

        await AssertRoundTrip(new Variant((nint)(-321_123)), (nint)(-321_123), static variant =>
        {
            var success = variant.TryGetValue(out nint value);
            return (success, value);
        });

        await AssertRoundTrip(new Variant(Int128.Parse("-17014118346046923173168730371588410572")), Int128.Parse("-17014118346046923173168730371588410572"), static variant =>
        {
            var success = variant.TryGetValue(out Int128 value);
            return (success, value);
        });
    }

    [Test]
    public async Task UnsignedIntegralValues()
    {
        await AssertRoundTrip(new Variant((byte)250), (byte)250, static variant =>
        {
            var success = variant.TryGetValue(out byte value);
            return (success, value);
        });

        await AssertRoundTrip(new Variant((ushort)65000), (ushort)65000, static variant =>
        {
            var success = variant.TryGetValue(out ushort value);
            return (success, value);
        });

        await AssertRoundTrip(new Variant(3_456_789_012U), 3_456_789_012U, static variant =>
        {
            var success = variant.TryGetValue(out uint value);
            return (success, value);
        });

        await AssertRoundTrip(new Variant(12_345_678_901_234_567_890UL), 12_345_678_901_234_567_890UL, static variant =>
        {
            var success = variant.TryGetValue(out ulong value);
            return (success, value);
        });

        await AssertRoundTrip(new Variant((nuint)654_321), (nuint)654_321, static variant =>
        {
            var success = variant.TryGetValue(out nuint value);
            return (success, value);
        });

        await AssertRoundTrip(new Variant(UInt128.Parse("34028236692093846346337460743176821145")), UInt128.Parse("34028236692093846346337460743176821145"), static variant =>
        {
            var success = variant.TryGetValue(out UInt128 value);
            return (success, value);
        });
    }

    [Test]
    public async Task FloatingPointValues()
    {
        await AssertRoundTrip(new Variant((Half)1.5f), (Half)1.5f, static variant =>
        {
            var success = variant.TryGetValue(out Half value);
            return (success, value);
        });

        await AssertRoundTrip(new Variant(123.5f), 123.5f, static variant =>
        {
            var success = variant.TryGetValue(out float value);
            return (success, value);
        });

        await AssertRoundTrip(new Variant(456.25d), 456.25d, static variant =>
        {
            var success = variant.TryGetValue(out double value);
            return (success, value);
        });
    }

    [Test]
    public async Task DecimalValue()
    {
        await AssertRoundTrip(new Variant(7922816251426433759354395.0335m), 7922816251426433759354395.0335m, static variant =>
        {
            var success = variant.TryGetValue(out decimal value);
            return (success, value);
        });
    }

    [Test]
    public async Task TemporalValues()
    {
        await AssertRoundTrip(new Variant(TimeSpan.FromTicks(9876543210)), TimeSpan.FromTicks(9876543210), static variant =>
        {
            var success = variant.TryGetValue(out TimeSpan value);
            return (success, value);
        });

        await AssertRoundTrip(new Variant(new DateOnly(2026, 7, 6)), new DateOnly(2026, 7, 6), static variant =>
        {
            var success = variant.TryGetValue(out DateOnly value);
            return (success, value);
        });

        await AssertRoundTrip(new Variant(new TimeOnly(9, 50, 46, 123)), new TimeOnly(9, 50, 46, 123), static variant =>
        {
            var success = variant.TryGetValue(out TimeOnly value);
            return (success, value);
        });

        var dateTime = new DateTime(2026, 7, 6, 9, 50, 46, 123, DateTimeKind.Utc);
        await AssertRoundTrip(new Variant(dateTime), dateTime, static variant =>
        {
            var success = variant.TryGetValue(out DateTime value);
            return (success, value);
        });
        await Assert.That(((DateTime)new Variant(dateTime).Value).Kind).IsEqualTo(DateTimeKind.Utc);

        var dateTimeOffset = new DateTimeOffset(2026, 7, 6, 9, 50, 46, 123, TimeSpan.FromHours(-6));
        var dateTimeOffsetVariant = new Variant(dateTimeOffset);
        await AssertRoundTrip(dateTimeOffsetVariant, dateTimeOffset, static variant =>
        {
            var success = variant.TryGetValue(out DateTimeOffset value);
            return (success, value);
        });
        await Assert.That(((DateTimeOffset)dateTimeOffsetVariant.Value).Offset).IsEqualTo(dateTimeOffset.Offset);
    }

    [Test]
    public async Task GuidValue()
    {
        var guid = Guid.Parse("e63bcfc0-c8a3-4d11-9958-7f042e6cbef0");
        await AssertRoundTrip(new Variant(guid), guid, static variant =>
        {
            var success = variant.TryGetValue(out Guid value);
            return (success, value);
        });
    }

    [Test]
    public async Task MismatchedTypeReturnsFalseAndDefaultValue()
    {
        var intVariant = new Variant(42);
        await Assert.That(intVariant.TryGetValue(out Guid guidValue)).IsFalse();
        await Assert.That(guidValue).IsEqualTo(Guid.Empty);

        var guidVariant = new Variant(Guid.Parse("570e19b1-c9f9-42a9-ac0d-1a2f4024e629"));
        await Assert.That(guidVariant.TryGetValue(out bool boolValue)).IsFalse();
        await Assert.That(boolValue).IsFalse();

        var dateVariant = new Variant(new DateOnly(2026, 7, 6));
        await Assert.That(dateVariant.TryGetValue(out TimeSpan timeSpanValue)).IsFalse();
        await Assert.That(timeSpanValue).IsEqualTo(TimeSpan.Zero);
    }

    [Test]
    public async Task DefaultVariantHasNoTypedValue()
    {
        Variant variant = default;

        await Assert.That(variant.TryGetValue(out int intValue)).IsFalse();
        await Assert.That(intValue).IsEqualTo(0);

        await Assert.That(variant.TryGetValue(out Guid guidValue)).IsFalse();
        await Assert.That(guidValue).IsEqualTo(Guid.Empty);
    }

    private static async Task AssertRoundTrip<T>(Variant variant, T expected, Func<Variant, (bool Success, T Value)> tryGet)
    {
        var (success, value) = tryGet(variant);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo(expected);
        await Assert.That((T)variant.Value).IsEqualTo(expected);
    }
}
