# NonBoxingVariant

`Variant` is a small discriminated union for a fixed set of unmanaged value types. It stores the payload inline in a 16-byte buffer plus a discriminator, so constructing and carrying a `Variant` does **not** box the underlying value.

Use `TryGetValue(...)` to read the typed payload without boxing. Accessing `Value` returns `object`, so that path boxes the stored value.

## Supported types

| Category | Types |
| --- | --- |
| Boolean and character | `bool`, `char` |
| Signed integers | `sbyte`, `short`, `int`, `long`, `nint` + `Int128` on .NET 8+ |
| Unsigned integers | `byte`, `ushort`, `uint`, `ulong`, `nuint` + `UInt128` on .NET 8+ |
| Floating point | `float`, `double` + `Half` on .NET 8+ |
| Other numerics | `decimal` |
| Date and time | `TimeSpan`, `DateTime`, `DateTimeOffset` + `DateOnly`/`TimeOnly` on .NET 8+ |
| Identifier | `Guid` |

## Creating a `Variant`

Create a `Variant` with the constructor for the type you want to store:

```csharp
Variant count = new(42);
Variant price = new(19.95m);
Variant when = new(DateTimeOffset.UtcNow);
Variant id = new(Guid.Parse("e63bcfc0-c8a3-4d11-9958-7f042e6cbef0"));
```

## Reading values without boxing

`TryGetValue` is the zero-allocation read path. It succeeds when the requested type matches the stored type exactly, or when the stored value is an integer type that can be widened safely to the requested integer type.

```csharp
Variant value = new(42);

if (value.TryGetValue(out int number))
{
    Console.WriteLine(number); // 42
}

var matched = value.TryGetValue(out Guid id);
Console.WriteLine(matched); // False
Console.WriteLine(id);      // 00000000-0000-0000-0000-000000000000
```

### Integer widening

Integer reads are based on type range, not on the specific stored value.

- Wider same-sign reads are allowed, such as `byte -> ushort -> uint -> ulong -> UInt128` and `sbyte -> short -> int -> long -> Int128`.
- Signed/unsigned changes are allowed only when the destination type can represent the full range of the source type, such as `byte -> short` and `uint -> long`.
- Non-widening conversions are rejected even if a particular value would fit, so `int -> uint` and `ulong -> long` do not work.

```csharp
Variant small = new((byte)250);
small.TryGetValue(out ushort asUInt16); // True

Variant wideUnsigned = new(3_456_789_012U);
wideUnsigned.TryGetValue(out long asInt64); // True

Variant signed = new(42);
signed.TryGetValue(out uint asUInt32); // False
```

Because there is no public discriminator property, the typical pattern is to probe the types you expect:

```csharp
static string Describe(Variant value)
{
    if (value.TryGetValue(out int i))
    {
        return $"int: {i}";
    }

    if (value.TryGetValue(out decimal d))
    {
        return $"decimal: {d}";
    }

    if (value.TryGetValue(out DateTimeOffset dto))
    {
        return $"date/time: {dto:O}";
    }

    if (value.TryGetValue(out Guid guid))
    {
        return $"guid: {guid}";
    }

    throw new InvalidOperationException("Unsupported variant value.");
}
```

## Reading through `Value`

`Value` exposes the current payload as `object`:

```csharp
Variant value = new(TimeSpan.FromMinutes(5));
object boxed = value.Value;

Console.WriteLine((TimeSpan)boxed); // 00:05:00
```

This is convenient, but it boxes the underlying value type. Prefer `TryGetValue` when you want to preserve the non-boxing behavior.

## Default value behavior

A default-initialized `Variant` has no stored value.

```csharp
Variant empty = default;

Console.WriteLine(empty.TryGetValue(out int number)); // False
Console.WriteLine(number);                             // 0

// empty.Value throws InvalidOperationException
```

## Notes

- `TryGetValue` and the explicit cast operators allow safe integer widening conversions based on type range, not on the specific runtime value.
- Signed/unsigned changes are allowed only when the destination type can represent the full range of the stored type.
- Equality compares integer payloads by numeric value, so integer variants with the same value compare equal even when their widths differ (for example `byte(42)` equals `long(42)`).
- `DateTime.Kind` and `DateTimeOffset.Offset` are preserved.
- The inline storage is 16 bytes, which is large enough for the currently supported payloads, including `Guid`.
