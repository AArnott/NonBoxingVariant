public class VariantTests
{
    [Test]
    public async Task SyntaxSugar()
    {
        Variant v = 5;
        switch (v)
        {
            case int i:
                await Assert.That(i).IsEqualTo(5);
                break;
            default:
                Assert.Fail("Unrecognized type");
                break;
        }

        await Assert.That(v is int).IsTrue();
    }

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

#if NET
        await AssertRoundTrip(new Variant(Int128.Parse("-17014118346046923173168730371588410572")), Int128.Parse("-17014118346046923173168730371588410572"), static variant =>
        {
            var success = variant.TryGetValue(out Int128 value);
            return (success, value);
        });
#endif
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

#if NET
        await AssertRoundTrip(new Variant(UInt128.Parse("34028236692093846346337460743176821145")), UInt128.Parse("34028236692093846346337460743176821145"), static variant =>
        {
            var success = variant.TryGetValue(out UInt128 value);
            return (success, value);
        });
#endif
    }

    [Test]
    public async Task IntegerWideningConversionsSucceed()
    {
        await AssertWideningConversion((byte)250, (short)250, static value => value, static variant =>
        {
            var success = variant.TryGetValue(out short widened);
            return (success, widened);
        }, static variant => (short)variant);

        await AssertWideningConversion((ushort)65000, 65000, static value => value, static variant =>
        {
            var success = variant.TryGetValue(out int widened);
            return (success, widened);
        }, static variant => (int)variant);

        await AssertWideningConversion(3_456_789_012U, 3_456_789_012L, static value => value, static variant =>
        {
            var success = variant.TryGetValue(out long widened);
            return (success, widened);
        }, static variant => (long)variant);

#if NET
        await AssertWideningConversion(-9_876_543_210L, (Int128)(-9_876_543_210L), static value => value, static variant =>
        {
            var success = variant.TryGetValue(out Int128 widened);
            return (success, widened);
        }, static variant => (Int128)variant);

        await AssertWideningConversion(12_345_678_901_234_567_890UL, (UInt128)12_345_678_901_234_567_890UL, static value => value, static variant =>
        {
            var success = variant.TryGetValue(out UInt128 widened);
            return (success, widened);
        }, static variant => (UInt128)variant);
#endif

        await AssertWideningConversion((nuint)654_321, 654_321UL, static value => value, static variant =>
        {
            var success = variant.TryGetValue(out ulong widened);
            return (success, widened);
        }, static variant => (ulong)variant);

        if (IntPtr.Size == 8)
        {
            await AssertWideningConversion(654_321U, (nint)654_321, static value => value, static variant =>
            {
                var success = variant.TryGetValue(out nint widened);
                return (success, widened);
            }, static variant => (nint)variant);

            await AssertWideningConversion(654_321UL, (nuint)654_321, static value => value, static variant =>
            {
                var success = variant.TryGetValue(out nuint widened);
                return (success, widened);
            }, static variant => (nuint)variant);
        }
    }

    [Test]
    public async Task NonWideningIntegerConversionsFail()
    {
        await AssertUnavailableConversion((byte)42, static value => value, static variant =>
        {
            var success = variant.TryGetValue(out sbyte narrowed);
            return (success, narrowed);
        }, static variant => (sbyte)variant);

        await AssertUnavailableConversion((short)42, static value => value, static variant =>
        {
            var success = variant.TryGetValue(out ushort changedSign);
            return (success, changedSign);
        }, static variant => (ushort)variant);

        await AssertUnavailableConversion(42, static value => value, static variant =>
        {
            var success = variant.TryGetValue(out uint changedSign);
            return (success, changedSign);
        }, static variant => (uint)variant);

        await AssertUnavailableConversion(3_456_789_012U, static value => value, static variant =>
        {
            var success = variant.TryGetValue(out int narrowed);
            return (success, narrowed);
        }, static variant => (int)variant);

        await AssertUnavailableConversion(1UL, static value => value, static variant =>
        {
            var success = variant.TryGetValue(out long changedSign);
            return (success, changedSign);
        }, static variant => (long)variant);
    }

    [Test]
    public async Task FloatingPointValues()
    {
#if NET
        await AssertRoundTrip(new Variant((Half)1.5f), (Half)1.5f, static variant =>
        {
            var success = variant.TryGetValue(out Half value);
            return (success, value);
        });
#endif

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

#if NET
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
#endif

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

#if NET
        var dateVariant = new Variant(new DateOnly(2026, 7, 6));
        await Assert.That(dateVariant.TryGetValue(out TimeSpan timeSpanValue)).IsFalse();
        await Assert.That(timeSpanValue).IsEqualTo(TimeSpan.Zero);
#endif
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

    [Test]
    public async Task IntegerVariantsCompareEqualAcrossWidths()
    {
        await AssertVariantsEqual((byte)42, (short)42);
        await AssertVariantsEqual(42, 42U);
#if NET
        await AssertVariantsEqual(42L, (UInt128)42);
        await AssertVariantsEqual((Int128)42, (UInt128)42);
#endif

        if (IntPtr.Size == 8)
        {
            await AssertVariantsEqual((nint)42, (nuint)42);
            await AssertVariantsEqual(42U, (nint)42);
        }
    }

    [Test]
    public async Task VariantEqualityDistinguishesDifferentNumericValuesAndNonIntegerTypes()
    {
        await AssertVariantsNotEqual((short)-1, (ushort)65535);
        await AssertVariantsNotEqual(42, 43U);
        await AssertVariantsNotEqual('A', 65);
        await AssertVariantsNotEqual(true, 1);

        Variant left = default;
        Variant right = default;
        await Assert.That(left.Equals(right)).IsTrue();
        await Assert.That(left == right).IsTrue();
        await Assert.That(left.GetHashCode()).IsEqualTo(right.GetHashCode());
    }

    [Test]
    public async Task ImplicitAndExplicitOperatorsRoundTripValues()
    {
        await AssertImplicitAndExplicitRoundTrip(true, static value => value, static variant => (bool)variant);
        await AssertImplicitAndExplicitRoundTrip('Z', static value => value, static variant => (char)variant);
        await AssertImplicitAndExplicitRoundTrip((sbyte)-12, static value => value, static variant => (sbyte)variant);
        await AssertImplicitAndExplicitRoundTrip((byte)250, static value => value, static variant => (byte)variant);
        await AssertImplicitAndExplicitRoundTrip((short)-32000, static value => value, static variant => (short)variant);
        await AssertImplicitAndExplicitRoundTrip((ushort)65000, static value => value, static variant => (ushort)variant);
        await AssertImplicitAndExplicitRoundTrip(-123456789, static value => value, static variant => (int)variant);
        await AssertImplicitAndExplicitRoundTrip(3_456_789_012U, static value => value, static variant => (uint)variant);
        await AssertImplicitAndExplicitRoundTrip(-9_876_543_210L, static value => value, static variant => (long)variant);
        await AssertImplicitAndExplicitRoundTrip((nint)(-321_123), static value => value, static variant => (nint)variant);
        await AssertImplicitAndExplicitRoundTrip(12_345_678_901_234_567_890UL, static value => value, static variant => (ulong)variant);
        await AssertImplicitAndExplicitRoundTrip((nuint)654_321, static value => value, static variant => (nuint)variant);
#if NET
        await AssertImplicitAndExplicitRoundTrip(Int128.Parse("-17014118346046923173168730371588410572"), static value => value, static variant => (Int128)variant);
        await AssertImplicitAndExplicitRoundTrip(UInt128.Parse("34028236692093846346337460743176821145"), static value => value, static variant => (UInt128)variant);
        await AssertImplicitAndExplicitRoundTrip((Half)1.5f, static value => value, static variant => (Half)variant);
#endif
        await AssertImplicitAndExplicitRoundTrip(123.5f, static value => value, static variant => (float)variant);
        await AssertImplicitAndExplicitRoundTrip(456.25d, static value => value, static variant => (double)variant);
        await AssertImplicitAndExplicitRoundTrip(7922816251426433759354395.0335m, static value => value, static variant => (decimal)variant);
        await AssertImplicitAndExplicitRoundTrip(TimeSpan.FromTicks(9876543210), static value => value, static variant => (TimeSpan)variant);
#if NET
        await AssertImplicitAndExplicitRoundTrip(new DateOnly(2026, 7, 6), static value => value, static variant => (DateOnly)variant);
        await AssertImplicitAndExplicitRoundTrip(new TimeOnly(9, 50, 46, 123), static value => value, static variant => (TimeOnly)variant);
#endif
        await AssertImplicitAndExplicitRoundTrip(new DateTime(2026, 7, 6, 9, 50, 46, 123, DateTimeKind.Utc), static value => value, static variant => (DateTime)variant);
        await AssertImplicitAndExplicitRoundTrip(new DateTimeOffset(2026, 7, 6, 9, 50, 46, 123, TimeSpan.FromHours(-6)), static value => value, static variant => (DateTimeOffset)variant);
        await AssertImplicitAndExplicitRoundTrip(Guid.Parse("e63bcfc0-c8a3-4d11-9958-7f042e6cbef0"), static value => value, static variant => (Guid)variant);
    }

    [Test]
    public async Task ExplicitOperatorsThrowOnMismatchedOrMissingValue()
    {
        Variant intVariant = 42;
        await AssertInvalidCast(() => _ = (Guid)intVariant);

        Variant guidVariant = Guid.Parse("570e19b1-c9f9-42a9-ac0d-1a2f4024e629");
        await AssertInvalidCast(() => _ = (bool)guidVariant);

        Variant empty = default;
        await AssertInvalidCast(() => _ = (TimeSpan)empty);
    }

    private static async Task AssertRoundTrip<T>(Variant variant, T expected, Func<Variant, (bool Success, T Value)> tryGet)
    {
        var (success, value) = tryGet(variant);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo(expected);
        await Assert.That((T)variant.Value).IsEqualTo(expected);
    }

    private static async Task AssertImplicitAndExplicitRoundTrip<T>(T expected, Func<T, Variant> toVariant, Func<Variant, T> fromVariant)
    {
        Variant variant = toVariant(expected);

        await Assert.That((T)variant.Value).IsEqualTo(expected);
        await Assert.That(fromVariant(variant)).IsEqualTo(expected);
    }

    private static async Task AssertWideningConversion<TFrom, TTo>(TFrom expected, TTo widened, Func<TFrom, Variant> toVariant, Func<Variant, (bool Success, TTo Value)> tryGet, Func<Variant, TTo> fromVariant)
    {
        Variant variant = toVariant(expected);
        var (success, value) = tryGet(variant);

        await Assert.That(success).IsTrue();
        await Assert.That(value).IsEqualTo(widened);
        await Assert.That(fromVariant(variant)).IsEqualTo(widened);
    }

    private static async Task AssertUnavailableConversion<TFrom, TTo>(TFrom expected, Func<TFrom, Variant> toVariant, Func<Variant, (bool Success, TTo Value)> tryGet, Func<Variant, TTo> fromVariant)
    {
        Variant variant = toVariant(expected);
        var (success, value) = tryGet(variant);

        await Assert.That(success).IsFalse();
        await Assert.That(value).IsEqualTo(default(TTo));
        await AssertInvalidCast(() => _ = fromVariant(variant));
    }

    private static async Task AssertVariantsEqual(Variant left, Variant right)
    {
        await Assert.That(left.Equals(right)).IsTrue();
        await Assert.That(right.Equals(left)).IsTrue();
        await Assert.That(left == right).IsTrue();
        await Assert.That(left != right).IsFalse();
        await Assert.That(left.GetHashCode()).IsEqualTo(right.GetHashCode());
    }

    private static async Task AssertVariantsNotEqual(Variant left, Variant right)
    {
        await Assert.That(left.Equals(right)).IsFalse();
        await Assert.That(right.Equals(left)).IsFalse();
        await Assert.That(left == right).IsFalse();
        await Assert.That(left != right).IsTrue();
    }

    private static async Task AssertInvalidCast(Action cast)
    {
        var threw = false;

        try
        {
            cast();
        }
        catch (InvalidCastException)
        {
            threw = true;
        }

        await Assert.That(threw).IsTrue();
    }
}
