using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

#if NET11_0_OR_GREATER

[assembly: TypeForwardedTo(typeof(IUnion))]

#else

namespace System.Runtime.CompilerServices;

/// <summary>
/// Implemented by union types to expose the currently held value.
/// </summary>
internal interface IUnion
{
    /// <summary>
    /// Gets the value currently held by the union.
    /// </summary>
    object Value { get; }
}

/// <summary>
/// Marks a struct emitted by the compiler as a union type.
/// </summary>
[AttributeUsage(AttributeTargets.Struct)]
[Embedded]
internal sealed class UnionAttribute : Attribute
{
}

#endif
